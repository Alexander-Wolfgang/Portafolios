using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class Player_Control : MonoBehaviour
{
    public Camera CamaraExploracion;
    public GameObject slashPrefab;
    public Transform attackPoint;
    public bool atacando = false;
    public BarraSalud healthUI;
    public GameObject BarraSalud;
    public GameObject Escudo;
    public GameObject BarraMana;
    private IA_Mov_tierra Enemigo_mapa;
    [SerializeField] private XP xpBar;
    public PauseManager Pause;
    bool esta_pausado = false;
    [SerializeField] private GameObject HUD_Exploracion;
    [SerializeField] private AudioSource Musica;
    [SerializeField] private AudioSource SFX_Pausar;
    [SerializeField] private AudioSource SFX_DesPausar;
    [SerializeField] private AudioSource SFX_puas;
    private const float NORMAL_VOLUME = 0.1f;
    private const float PAUSE_VOLUME = 0.03f;

    //Desplegar Oro en el HUD
    public TMPro.TextMeshProUGUI OroText;

    //es para simplificar codigo
    GameObject currentSlash;

    public static Player_Control Instance;
    public int totalCards;
    public int Anime_Estado;
    public int Vida_Extra;
    public Rigidbody2D rb;
    private bool jumpRequest;
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    public System.Action<int, int> OnHealthChanged;
    public System.Action<int, int> OnManaChanged;

    private bool isGrounded;
    private SpriteRenderer SR;

    //[ANIMAR]
    private Animator animator;

    public CharacterStats baseStats;
    // Establecer Stats
    //[MAIN_STATS]
    int HP_Current;
    int HP_Max;
    int HP_Limit;
    bool UsaEnergia;
    int Energia;
    bool UsaMana;
    int Mana_Current;
    int Mana_Max;
    int Mana_Limit;
    int manaRegenPorTurno;

    //[EXPLORATION]
    float VelocidadMax;
    int Num_Saltos;
    int saltosRestantes;
    int LimiteSaltos;
    float Fuerza_Salto;
    float Acelerar_Tierra;
    float Acelerar_Aire;
    float Freno_Tierra;
    float Freno_Aire;
    float AirControl;
    float Vel_Ataque;
    private float nextAttackTime = 0f;
    int Bloqueos_Restantes;
    int Bloqueos_Max;
    int Bloqueos_Limit;
    bool nadar;
    bool bucear;
    bool escalar;

    [SerializeField] private float CoyoteTime = 0.15f;
    private float coyoteTimer;

    //[COMBAT]
    int RoboBase;
    int ManoMaxima;

    //[STATUS MODIFIERS]
    int Potenciar_Duracion_Estado;
    int Resistir_Estado;

    //[Monedas]
    public int gold;                        //Oro Normal, se usa en las tiendas de la RUN
    public int gold_Moai;                   //Oro Rapanui, se usa con el moai en el HUB
    public int experience;                  //Experiencia para tu personaje
    public enum EstadoAnimacion
    {
        Idle,
        Walk,
        Jump,
        Fall,
        Attack
    }
    float moveInput;
    private hazard currentHazard;
    private float hazardTimer = 0f;
    void Awake()
    {
        Debug.Log("Player_Control Awake -> " + GetInstanceID());

        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();

        // Asegurar que exista PlayerRunData
        if (PlayerRunData.Instance == null)
        {
            GameObject go = new GameObject("PlayerRunData");
            go.AddComponent<PlayerRunData>();
        }

        if (!PlayerRunData.Instance.IsInitialized)
        {
            PlayerRunData.Instance.InitializeFromBaseStats(baseStats);
        }
        PlayerRunData.Instance.Estas_Combatiendo(false);
    }
    void Start()
    {
        Anime_Estado = 0; //Idle
        //OnHealthChanged?.Invoke(HP_Current, HP_Max);
        if (baseStats == null)
        {
            enabled = false;
            return;
        }
        animator = GetComponent<Animator>();
        //Coyote Time
        CoyoteTime = 0.15f;
        coyoteTimer = CoyoteTime;
        //Tamaño mazo
        
        //Rigidbody
        rb = GetComponent<Rigidbody2D>();

        ApplyRunStats();
        //Health UI
        PlayerRunData.Instance.OnHealthChanged?.Invoke(HP_Current, HP_Max);
        Debug.Log("Salud: " + HP_Current);
        Debug.Log("Energia: " + Energia);
        Debug.Log("¿Usa energia? " + UsaEnergia);
    }
    void ApplyRunStats()
    {
        var run = PlayerRunData.Instance;

        // MAIN
        HP_Current = run.current_HP;
        HP_Max = run.Max_HP;
        HP_Limit = run.Limit_HP;

        Mana_Max = run.Max_MP;
        Mana_Current = run.current_MP;
        Mana_Limit = run.Limit_MP;

        UsaEnergia = run.UsaEnergia;
        Energia = run.Energia;

        manaRegenPorTurno = run.manaRegenPorTurno;
        UsaMana = run.UsaMana;

        // EXPLORATION
        VelocidadMax = run.Velocidad;
        Fuerza_Salto = run.Fuerza_Salto;
        Num_Saltos = run.Num_Saltos;
        LimiteSaltos = run.Limite_Saltos;
        Acelerar_Tierra = run.Acelerar_Tierra;
        Acelerar_Aire = run.Acelerar_Aire;
        Freno_Tierra = run.Freno_Tierra;
        Freno_Aire = run.Freno_Aire;
        AirControl = run.AirControl;
        Vel_Ataque = run.Vel_Ataque;

        Bloqueos_Max = run.Num_Bloqueo;
        Bloqueos_Limit = run.Limite_Bloqueos;

        nadar = run.Nadar;
        bucear = run.Bucear;
        escalar = run.Escalar;

        // COMBAT
        RoboBase = run.roboBase;
        ManoMaxima = run.manoMaxima;
        experience = run.experience;
        gold = run.gold;
        gold_Moai = run.gold_Moai;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PressEscape();

        if (Keyboard.current == null) return;
            CheckGround();
        if (isGrounded)
        {
            coyoteTimer = CoyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();

                // Calcula el próximo momento en que puedes atacar
                nextAttackTime = Time.time + Vel_Ataque;
            }
        }

        float left = Keyboard.current.leftArrowKey.isPressed ? -1f : 0f;
        float right = Keyboard.current.rightArrowKey.isPressed ? 1f : 0f;

        moveInput = left + right;

        if (!atacando)
        {
            if (moveInput < 0)
            {
                attackPoint.localPosition = new Vector3(-0.8f, -0.05f, 0f);
                SR.flipX = false;
            }
            else if (moveInput > 0)
            {
                attackPoint.localPosition = new Vector3(0.8f, -0.05f, 0f);
                SR.flipX = true;
            }
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Caso Coyote Time
            if (!isGrounded)
            {
                if (coyoteTimer < 0f)
                {
                    saltosRestantes = Num_Saltos - 1;
                }
                else if (saltosRestantes > 0)
                {
                    jumpRequest = true;
                    coyoteTimer = 0f;
                }
            }
            else if (saltosRestantes > 0)
            {
                saltosRestantes--;
                jumpRequest = true;
            }
        }
        if (currentHazard != null)
        {
            hazardTimer -= Time.deltaTime;

            if (hazardTimer <= 0f && HP_Current > currentHazard.DPS)    //no puede morir por hazard, por ahora
            {
                TakeDamage(currentHazard.DPS);
                hazardTimer = 1f;
            }
        }
        ActualizarAnimacion();
    }
    public void PressEscape()
    {
        if (esta_pausado)
        {
            //Vamos a despausar el juego
            SFX_DesPausar.Play();
            Musica.volume = NORMAL_VOLUME;
            esta_pausado = false;
            //Encender UI de salud, mana y escudo o moverla, ahi vere que hago
            BarraSalud.SetActive(true);
            //Escudo.SetActive(true);
            //BarraMana.SetActive(true);

        }
        else
        {
            //Vamos a pausar el juego
            SFX_Pausar.Play();
            Musica.volume = PAUSE_VOLUME;
            esta_pausado = true;
            //Apagar UI de salud, mana y escudo o moverla, ahi vere que hago
            BarraSalud.SetActive(false);
            //Escudo.SetActive(false);
            //BarraMana.SetActive(false);
            //No deberia ser asi, sino que llame a la funcion para 
        }

        //Activar pause
        Pause.PresionaEscape();
    }
    public void TakeDamage(int damage)
    {
        HP_Current = PlayerRunData.Instance.current_HP;

        HP_Current -= damage;

        HP_Current = Mathf.Clamp(HP_Current, 0, HP_Max);

        // 🔥 sincronizar con PlayerRunData
        PlayerRunData.Instance.current_HP = HP_Current;

        // 🔥 actualizar UI mediante evento
        PlayerRunData.Instance.OnHealthChanged?.Invoke(HP_Current, HP_Max);

        //Actualizar UI salud
        healthUI.healthText.text = HP_Current + "/" + HP_Max;
        OnHealthChanged?.Invoke(HP_Current, HP_Max);

        if (HP_Current <= 0)
        {
            Debug.Log("Jugador muerto");
        }
    }
    [SerializeField] private float groundCheckRadius = 0.15f;

    void CheckGround()
    {
        bool estabaEnSuelo = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (!estabaEnSuelo && isGrounded)
        {
            saltosRestantes = Num_Saltos;
        }
    }
    void FixedUpdate()
    {
        float aceleracionActual;
        float frenoActual;

        if (isGrounded)
        {
            aceleracionActual = Acelerar_Tierra;
            frenoActual = Freno_Tierra;
        }
        else
        {
            aceleracionActual = Acelerar_Aire * AirControl;
            frenoActual = Freno_Aire * AirControl;
        }

        float velocidadX = rb.linearVelocity.x;

        if (moveInput != 0)
        {
            float velocidadObjetivo = moveInput * VelocidadMax;

            velocidadX = Mathf.MoveTowards(
                velocidadX,
                velocidadObjetivo,
                aceleracionActual * Time.fixedDeltaTime
            );
        }
        else
        {
            velocidadX = Mathf.MoveTowards(
                velocidadX,
                0,
                frenoActual * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = new Vector2(
            velocidadX,
            rb.linearVelocity.y
        );

        if (jumpRequest)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Fuerza_Salto
            );

            jumpRequest = false;
        }
    }
    void ActualizarAnimacion()
    {
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                Anime_Estado = 2;
            }
            else
            {
                Anime_Estado = 3;
            }
        }
        else
        {
            if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                Anime_Estado = 1;
            }
            else
            {
                Anime_Estado = 0;
            }
        }
        animator.SetInteger("Estado", Anime_Estado);
    }
    public void Attack()
    {
        atacando = true;

        currentSlash = Instantiate(
            slashPrefab,
            attackPoint.position,
            Quaternion.identity,
            attackPoint
        );

        MeleeSlash slash = currentSlash.GetComponent<MeleeSlash>();

        if (slash != null)
        {
            slash.player = this;

            if (SR.flipX)
            {
                slash.startAngle = 80f;
                slash.endAngle = -80f;
            }
            else
            {
                slash.startAngle = -80f;
                slash.endAngle = 80f;
            }
        }
    }
    IEnumerator SlashRoutine(GameObject slash)
    {
        if (slash == null) yield break;

        float currentAngle = 0f;
        float maxAngle = 110f;
        float speed = 300f; // grados por segundo

        int direction = SR.flipX ? -1 : 1;

        while (currentAngle < maxAngle)
        {
            // Si el objeto fue destruido por otro lado, salimos sin intentar acceder a su transform.
            if (slash == null)
                yield break;

            float step = speed * Time.deltaTime;

            slash.transform.Rotate(0f, 0f, step * direction);
            currentAngle += step;

            yield return null;
        }

        if (slash != null)
            Destroy(slash);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("💥 Colisionaste con un enemigo");

            // 🔥 Obtener el script del enemigo
            IA_Mov_tierra enemigo = collision.gameObject.GetComponent<IA_Mov_tierra>();

            // 🔥 Si el collider está en un hijo (MUY común)
            if (enemigo == null)
                enemigo = collision.gameObject.GetComponentInParent<IA_Mov_tierra>();

            if (enemigo != null)
            {
                PlayerRunData.Instance.BonusActual = (PlayerRunData.Bonus)0;
                int idGrupo = enemigo.ID_GrupoEnemigos;

                Debug.Log("Grupo enemigo: " + idGrupo);

                // 🔥 GUARDAR EN PRD
                PlayerRunData.Instance.ID_GrupoEnemigo = idGrupo;
                // 🔥 destruir enemigo del mapa
                //Destroy(enemigo.gameObject);
                // 🔥 Guardar y cargar combate
                Combatir(idGrupo, enemigo);
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró IA_Mov_tierra en el enemigo");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("hazard"))
        {
            currentHazard = other.GetComponent<hazard>();

            if (currentHazard != null && HP_Current > currentHazard.DPS) //no puede morir por hazard, por ahora
            {
                // daño instantáneo al tocar
                TakeDamage(currentHazard.DPS);

                // esperar 1 segundo para el siguiente tick
                hazardTimer = 1f;
                SFX_puas.Play();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("hazard"))
        {
            currentHazard = null;
        }
    }
    public void GuardarDatos()
    {
        // ======================
        // STATS
        // ======================
        PlayerRunData.Instance.current_HP = HP_Current;
        PlayerRunData.Instance.Max_HP = HP_Max;
        PlayerRunData.Instance.Limit_HP = HP_Limit;
        PlayerRunData.Instance.UsaEnergia = UsaEnergia;
        PlayerRunData.Instance.Energia = Energia;
        PlayerRunData.Instance.UsaMana = UsaMana;
        PlayerRunData.Instance.current_MP = Mana_Current;
        PlayerRunData.Instance.Max_MP = Mana_Max;
        PlayerRunData.Instance.Limit_MP = Mana_Limit;
        PlayerRunData.Instance.manaRegenPorTurno = manaRegenPorTurno;
        // ======================
        // EXPLORATION
        // ======================
        PlayerRunData.Instance.Velocidad = VelocidadMax;
        PlayerRunData.Instance.Num_Saltos = Num_Saltos;
        PlayerRunData.Instance.Limite_Saltos = LimiteSaltos;
        PlayerRunData.Instance.Fuerza_Salto = Fuerza_Salto;
        PlayerRunData.Instance.Acelerar_Tierra = Acelerar_Tierra;
        PlayerRunData.Instance.Acelerar_Aire = Acelerar_Aire;
        PlayerRunData.Instance.Freno_Tierra = Freno_Tierra;
        PlayerRunData.Instance.Freno_Aire = Freno_Aire;
        PlayerRunData.Instance.AirControl = AirControl;
        PlayerRunData.Instance.Vel_Ataque = Vel_Ataque;
        PlayerRunData.Instance.Num_Bloqueo = Bloqueos_Restantes;
        PlayerRunData.Instance.Limite_Bloqueos = Bloqueos_Max;
        // ======================
        // HABILIDADES
        // ======================
        PlayerRunData.Instance.Nadar = nadar;
        PlayerRunData.Instance.Bucear = bucear;
        PlayerRunData.Instance.Escalar = escalar;
        // ======================
        // POSICIÓN
        // ======================
        PlayerRunData.Instance.posicion = transform.position;
        // ======================
        // STATUS
        // ======================
        PlayerRunData.Instance.Potenciar_Duracion_Estado = Potenciar_Duracion_Estado;
        PlayerRunData.Instance.Resistir_Estado = Resistir_Estado;
        // ======================
        // RECURSOS
        // ======================
        PlayerRunData.Instance.gold = gold;
        PlayerRunData.Instance.gold_Moai = gold_Moai;
        PlayerRunData.Instance.experience = experience;
        // ======================
        // COMBAT
        // ======================
        PlayerRunData.Instance.roboBase = RoboBase;
        PlayerRunData.Instance.manoMaxima = ManoMaxima;
    }
    public void CargarDatos()
    {
        var run = PlayerRunData.Instance;

        // ======================
        // STATS
        // ======================
        HP_Current = run.current_HP;
        HP_Max = run.Max_HP;
        HP_Limit = run.Limit_HP;

        UsaEnergia = run.UsaEnergia;
        Energia = run.Energia;

        UsaMana = run.UsaMana;
        Mana_Current = run.current_MP;
        Mana_Max = run.Max_MP;
        Mana_Limit = run.Limit_MP;
        manaRegenPorTurno = run.manaRegenPorTurno;

        // ======================
        // EXPLORATION
        // ======================
        VelocidadMax = run.Velocidad;
        Num_Saltos = run.Num_Saltos;
        LimiteSaltos = run.Limite_Saltos;
        Fuerza_Salto = run.Fuerza_Salto;

        Acelerar_Tierra = run.Acelerar_Tierra;
        Acelerar_Aire = run.Acelerar_Aire;
        Freno_Tierra = run.Freno_Tierra;
        Freno_Aire = run.Freno_Aire;

        AirControl = run.AirControl;
        Vel_Ataque = run.Vel_Ataque;

        Bloqueos_Restantes = run.Num_Bloqueo;
        Bloqueos_Max = run.Limite_Bloqueos;

        // ======================
        // HABILIDADES
        // ======================
        nadar = run.Nadar;
        bucear = run.Bucear;
        escalar = run.Escalar;

        // ======================
        // POSICIÓN
        // ======================
        transform.position = run.posicion;

        // ======================
        // STATUS
        // ======================
        Potenciar_Duracion_Estado = run.Potenciar_Duracion_Estado;
        Resistir_Estado = run.Resistir_Estado;

        // ======================
        // RECURSOS
        // ======================
        gold = run.gold;
        gold_Moai = run.gold_Moai;
        experience = run.experience;

        // ======================
        // COMBAT
        // ======================
        RoboBase = run.roboBase;
        ManoMaxima = run.manoMaxima;
    }
    public void Combatir(int ID, IA_Mov_tierra enemigo)
    {
        GuardarDatos();
        Pausar_EX();
        // 🔥 destruir enemigo del mapa
        Destroy(enemigo.gameObject);
        string escena = "9-Combate-" + ID;
        SceneManager.LoadScene(escena, LoadSceneMode.Additive);
    }
    public void CombateBOSS(int ID)
    {
        PlayerRunData.Instance.BonusActual = 0;
        GuardarDatos();
        Pausar_EX();
        string escena = "9-Combate-" + ID;
        SceneManager.LoadScene(escena, LoadSceneMode.Additive);
    }
    public void Pausar_EX()
    {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        enabled = false;
        HUD_Exploracion.SetActive(false);
        CamaraExploracion.enabled = false;
        Musica.Pause();
        PlayerRunData.Instance.Estas_Combatiendo(true);
        //Panel_Caracteristicas_Objetos.Instance = null;
    }
    public void Reanudar_EX()
    {
        PlayerRunData.Instance.Estas_Combatiendo(false);
        Time.timeScale = 1;
        rb.simulated = true;
        enabled = true;
        HUD_Exploracion.SetActive(true);
        var panel = HUD_Exploracion.GetComponentInChildren<Panel_Caracteristicas_Objetos>(true);
        if (panel != null)
            panel.Registrar();

        CamaraExploracion.enabled = true;
        Musica.UnPause();
        ApplyRunStats();
        Actualizar_Oro();
        Actualizar_XP();
    }
    public int GetDeckSize()
    {
        if (DeckManager.Instance == null)
        {
            Debug.LogWarning("⚠️ DeckManager no inicializado");
            return 0;
        }

        return DeckManager.Instance.GetDeckSize();
    }

    public void PrintDeckInfo()
    {
        if (DeckManager.Instance == null)
        {
            Debug.LogWarning("⚠️ DeckManager no inicializado");
            return;
        }

        totalCards = DeckManager.Instance.GetDeckSize();
        Debug.Log($"📊 Total de cartas en el deck: {totalCards}");
        DeckManager.Instance.PrintDeckContents();
    }
    private void Actualizar_Oro()
    {
        OroText.text = gold.ToString();
    }
    private void Actualizar_XP()
    {
        xpBar.Barra_XP_Inicio_Combate(experience, 25);
    }
    
}