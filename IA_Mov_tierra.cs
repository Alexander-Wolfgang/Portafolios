using UnityEngine;
using UnityEngine.SceneManagement;
using static Base_Stat_Enemy;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class IA_Mov_tierra : MonoBehaviour
{
    public Transform jugador;

    public Player_Control PC;

    [Header("Stats Asset")]
    public Base_Stat_Enemy statsAsset;

    [Header("Atacar al Jugador")]
    public GameObject ataquePrefab;
    public Transform puntoAtaque;
    public float rangoAtaque = 1.5f;
    private float cooldownAtaque = 1.0f;
    private float ultimoAtaque = -999f;

    [SerializeField]
    float distanciaTeleport = 5f;

    [Header("Hechizo")]
    public GameObject hechizoPrefab;
    public Transform puntoHechizo;
    public float rangoHechizo = 8f;

    [Header("Exploration")]
    public float Velocidad;
    public float Fuerza_Salto;
    public float Acelerar_Tierra;
    public float Acelerar_Aire;
    public float Freno_Tierra;
    public float Freno_Aire;
    public float Gravedad_base;
    public float MultiplicadorCaida;
    public int Num_Saltos;
    public int Limite_Saltos;
    public float Vision;
    public bool TeVio;
    public Comportamiento QueHace;
    public int ID_GrupoEnemigos;

    [Header("Escape")]
    public float DistanciaPerderJugador = 20f;

    [Header("Raycast")]
    public float Distancia_Raycast_Frente = 0.2f;
    public float Distancia_Raycast_Abajo = 0.3f;
    public float DistanciaDeteccionBorde = 0.8f;
    public LayerMask Capa_Colisiones;

    [Header("Salto de susto")]
    bool yaReacciono = false;
    public float FuerzaSaltoSusto = 3f;

    [Header("Debug")]
    public bool DibujarRaycasts = true;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    private int direccion = 1;
    private bool enTierra;

    private float tiempoUltimoCambio = 0f;
    private const float COOLDOWN_CAMBIO = 0.05f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (statsAsset != null)
        {
            Velocidad = statsAsset.Velocidad;
            Fuerza_Salto = statsAsset.Fuerza_Salto;
            Acelerar_Tierra = statsAsset.Acelerar_Tierra;
            Acelerar_Aire = statsAsset.Acelerar_Aire;
            Freno_Tierra = statsAsset.Freno_Tierra;
            Freno_Aire = statsAsset.Freno_Aire;
            Gravedad_base = statsAsset.Gravedad_base;
            MultiplicadorCaida = statsAsset.MultiplicadorCaida;
            Num_Saltos = statsAsset.Num_Saltos;
            Limite_Saltos = statsAsset.Limite_Saltos;
            Vision = statsAsset.Vision;
            TeVio = statsAsset.TeVio;
            QueHace = statsAsset.QueHace;
            DistanciaDeteccionBorde = statsAsset.DistanciaDeteccionBorde;
        }
    }

    void OnDrawGizmos()
    {
        if (jugador == null) return;

        Gizmos.color = PuedeVerJugador() ? Color.green : Color.red;

        Vector3 dir = Vector3.right * direccion;
        Gizmos.DrawLine(transform.position, transform.position + dir * Vision);
    }

    void Update()
    {
        if (!TeVio && PuedeVerJugador())
        {
            TeVio = true;
            yaReacciono = false;

            if (QueHace == Comportamiento.Boss)
            {
                IniciarCombateBoss();
            }
        }
    }

    void FixedUpdate()
    {
        Bounds b = col.bounds;

        enTierra = DetectarSuelo(b);
        bool obstaculo = DetectarObstaculoFrente(b);
        bool haySueloAdelante = DetectarSueloAlFrente(b);

        Vector2 v = rb.linearVelocity;

        if ((obstaculo || !haySueloAdelante) && enTierra)
        {
            if (Time.time - tiempoUltimoCambio > COOLDOWN_CAMBIO)
            {
                direccion *= -1;

                if (sr != null)
                    sr.flipX = direccion == -1;

                tiempoUltimoCambio = Time.time;
            }
        }

        float objetivo;

        if (!TeVio)
        {
            objetivo = (haySueloAdelante && !obstaculo) ? Velocidad * direccion : 0f;
        }
        else
        {
            objetivo = ObtenerMovimientoSegunComportamiento(haySueloAdelante, obstaculo);
        }

        bool cambiandoDireccion =
        Mathf.Sign(v.x) != Mathf.Sign(objetivo)
        && Mathf.Abs(v.x) > 0.1f
        && Mathf.Abs(objetivo) > 0.1f;

        float control;

        if (Mathf.Abs(objetivo) < 0.01f)
        {
            control = enTierra ? Freno_Tierra : Freno_Aire;
        }
        else if (cambiandoDireccion)
        {
            control = enTierra ? Freno_Tierra : Freno_Aire;
        }
        else
        {
            control = enTierra ? Acelerar_Tierra : Acelerar_Aire;
        }

        v.x = Mathf.MoveTowards(
            v.x,
            objetivo,
            control * Time.fixedDeltaTime
        );

        if (!enTierra)
        {
            float gravedad = v.y < 0 ? Gravedad_base * MultiplicadorCaida : Gravedad_base;
            v.y -= gravedad * Time.fixedDeltaTime;
        }
        else if (v.y < 0)
        {
            v.y = 0;
        }

        rb.linearVelocity = v;
    }

    // ================= DETECCIONES =================

    bool DetectarSuelo(Bounds b)
    {
        Vector2 p1 = new Vector2(b.min.x + 0.05f, b.min.y);
        Vector2 p2 = new Vector2(b.max.x - 0.05f, b.min.y);

        bool hit1 = Physics2D.Raycast(p1, Vector2.down, Distancia_Raycast_Abajo, Capa_Colisiones);
        bool hit2 = Physics2D.Raycast(p2, Vector2.down, Distancia_Raycast_Abajo, Capa_Colisiones);

        if (DibujarRaycasts)
        {
            Debug.DrawRay(p1, Vector2.down * Distancia_Raycast_Abajo, Color.blue);
            Debug.DrawRay(p2, Vector2.down * Distancia_Raycast_Abajo, Color.blue);
        }

        return hit1 || hit2;
    }

    bool DetectarObstaculoFrente(Bounds b)
    {
        Vector2 origen = new Vector2(
            direccion == 1 ? b.max.x : b.min.x,
            b.center.y
        );

        Vector2 dir = Vector2.right * direccion;

        RaycastHit2D hit = Physics2D.Raycast(origen, dir, Distancia_Raycast_Frente, Capa_Colisiones);

        if (DibujarRaycasts)
            Debug.DrawRay(origen, dir * Distancia_Raycast_Frente, Color.red);

        return hit.collider != null;
    }

    bool DetectarSueloAlFrente(Bounds b)
    {
        Vector2 origen = new Vector2(
            direccion == 1
                ? b.max.x + DistanciaDeteccionBorde
                : b.min.x - DistanciaDeteccionBorde,
            b.min.y + 0.05f
        );

        RaycastHit2D hit = Physics2D.Raycast(
            origen,
            Vector2.down,
            1f,
            Capa_Colisiones
        );

        if (DibujarRaycasts)
        {
            Debug.DrawRay(origen, Vector2.down * 1f, Color.green);
        }

        return hit.collider != null;
    }

    float ObtenerMovimientoSegunComportamiento(bool haySueloAdelante, bool obstaculo)
    {
        float dist = DistanciaAlJugador();

        switch (QueHace)
        {
            case Comportamiento.Nada:
                return (haySueloAdelante && !obstaculo) ? Velocidad * direccion : 0f;

            case Comportamiento.Esconderse:
                {
                    float velocidadActual = Velocidad + 2.5f;

                    if (!yaReacciono && enTierra)
                    {
                        rb.linearVelocity = new Vector2(
                            DireccionLejosJugador() * 2f,
                            FuerzaSaltoSusto
                        );

                        yaReacciono = true;
                        return 0f;
                    }

                    if (dist > DistanciaPerderJugador)
                    {
                        TeVio = false;
                        return 0f;
                    }

                    SetDireccion(DireccionLejosJugador());
                    return velocidadActual * direccion;
                }

            case Comportamiento.Seguir:

                SetDireccion(DireccionHaciaJugador());

                if (!haySueloAdelante || obstaculo)
                    return 0f;

                return Velocidad * direccion;

            case Comportamiento.Atacar:
                if (dist > rangoAtaque)
                {
                    SetDireccion(DireccionHaciaJugador());
                    return Velocidad * direccion;
                }
                else
                {
                    AtacarJugador();
                    return 0f;
                }

            case Comportamiento.Teletransportar:
                Teletransportarse();
                return 0f;

            case Comportamiento.Hechizo:
                LanzarHechizo();
                return 0f;
        }

        return 0f;
    }

    int DireccionHaciaJugador() => CalcularDireccionJugador(true);
    int DireccionLejosJugador() => CalcularDireccionJugador(false);

    float DistanciaAlJugador()
    {
        if (jugador == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, jugador.position);
    }

    void EjecutarAtaque()
    {
        Debug.Log("Atacando jugador");
    }

    void Teletransportarse()
    {
        if (jugador == null)
            return;

        Vector3 destino =
            jugador.position +
            new Vector3(Random.Range(-distanciaTeleport, distanciaTeleport), 0, 0);

        transform.position = destino;

        TeVio = false;
    }

    void HuirDelJugador()
    {
        if (jugador == null) return;
        SetDireccion(DireccionLejosJugador());
    }

    void IrHaciaJugador()
    {
        if (jugador == null) return;
        SetDireccion(DireccionHaciaJugador());
    }

    void SetDireccion(int nuevaDir)
    {
        direccion = nuevaDir;

        if (sr != null)
            sr.flipX = direccion == -1;
    }

    void AtacarJugador()
    {
        if (jugador == null)
            return;

        float distancia = DistanciaAlJugador();

        if (distancia > rangoAtaque)
        {
            SetDireccion(DireccionHaciaJugador());
        }
        else if (Time.time >= ultimoAtaque + cooldownAtaque)
        {
            Instantiate(
                ataquePrefab,
                puntoAtaque.position,
                Quaternion.identity
            );

            ultimoAtaque = Time.time;
        }
    }
    void LanzarHechizo()
    {
        if (jugador == null)
            return;

        float distancia = DistanciaAlJugador();

        if (distancia > rangoHechizo)
        {
            SetDireccion(DireccionHaciaJugador());

            return;
        }

        if (Time.time >= ultimoAtaque + cooldownAtaque)
        {
            Instantiate(
                hechizoPrefab,
                puntoHechizo.position,
                Quaternion.identity
            );

            ultimoAtaque = Time.time;
        }
    }
    int CalcularDireccionJugador(bool haciaJugador)
    {
        if (jugador == null) return direccion;

        int dir = jugador.position.x > transform.position.x ? 1 : -1;
        return haciaJugador ? dir : -dir;
    }

    bool PuedeVerJugador()
    {
        if (jugador == null)
            return false;

        float distancia = DistanciaAlJugador();

        if (distancia > Vision)
            return false;

        if (statsAsset.tipo == Tipo_Enemigo.Jefe)
            return true;

        Vector2 toPlayer = jugador.position - transform.position;
        Vector2 forward = Vector2.right * direccion;

        float dot = Vector2.Dot(
            toPlayer.normalized,
            forward
        );

        return dot > 0.3f;
    }
    bool combateBossIniciado = false;

    void IniciarCombateBoss()
    {
        if (combateBossIniciado)
            return;

        //Animacion del Boss, puede ser un ataque o algo asi.


        combateBossIniciado = true;
        Debug.Log("Iniciando combate Boss");

        Destroy(gameObject);

        // aquí llamas tu sistema actual de combate
        PC.CombateBOSS(7);
    }
}