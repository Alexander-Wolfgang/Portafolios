using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Paneles")]
    public GameObject panelVictory;
    public GameObject panelPause;

    [Header("Referencias")]
    public BallGravityController bola;
    public GameObject PanelCronometro;
    public TextMeshProUGUI TuTiempo;
    public TextMeshProUGUI textoTiempo;

    [Header("Skybox")]
    public GameManagerSkybox gameManagerSkybox;

    [Header("Skins")]
    public Material[] materialesSkins;

    [Header("Niveles")]
    public GameObject[] niveles;

    [Header("Cronómetro")]
    public float tiempoNivel = 0f;
    public bool cronometroActivo = false;

    private GameObject nivelActual;
    private Level levelActual;
    public Level NivelActual => levelActual;

    private int indiceNivel = 0;

    private bool juegoPausado = false;
    private bool Pausable = true;

    public enum Medalla
    {
        Ninguna,
        Bronce,
        Plata,
        Oro
    }

    // Medalla conseguida en el intento ACTUAL
    public Medalla medallaActual = Medalla.Ninguna;

    [Header("Objetos Medallas")]
    public GameObject MedallaBronce;
    public GameObject MedallaPlata;
    public GameObject MedallaOro;

    [Header("Tiempos Medallas")]
    public TextMeshProUGUI TiempoMedallaBronce;
    public TextMeshProUGUI TiempoMedallaPlata;
    public TextMeshProUGUI TiempoMedallaOro;

    [Header("Sonido Medallas")]
    public AudioClip sonidoMedalla;
    public AudioClip Celebracion_ORO;

    [Header("Musica niveles")]
    public AudioClip Musica1;
    public AudioClip Musica2;
    public AudioClip MusicaFinal;

    [Header("Sonido interfaz")]
    public AudioClip Sonido_Click;
    public AudioClip Sonido_NextLevel;
    public AudioClip Sonido_Salir;
    public AudioClip Sonido_Pausar;

    [Header("Botones Victoria")]
    public GameObject BotonSalir;
    public GameObject BotonRepetirNivel;
    public GameObject BotonSiguienteNivel;
    public GameObject ThanksForPlaying;

    void Awake()
    {
        //#if UNITY_EDITOR
        //        PlayerPrefs.DeleteAll();
        //        PlayerPrefs.Save();
        //#endif

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    void Start()
    {
        Pausable = true;
        indiceNivel = NivelSeleccionado.indiceNivel;

        // Cargar material de la skin seleccionada
        int skinSeleccionada = PlayerPrefs.GetInt("RollingMaze_SkinSeleccionada", 1);

        Renderer rendererBola = bola.GetComponent<Renderer>();

        if (rendererBola != null && skinSeleccionada >= 1 && skinSeleccionada <= materialesSkins.Length)
        {
            rendererBola.material = materialesSkins[skinSeleccionada - 1];
        }

        CargarNivel(indiceNivel);
    }
    private void Update()
    {
        if (cronometroActivo)
        {
            tiempoNivel += Time.deltaTime;
            ActualizarTextoTiempo();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (juegoPausado)
                Reanudar();
            else
                Pausar();
        }
    }


    public void Pausar()
    {
        if (!Pausable)
            return;

        juegoPausado = true;
        Time.timeScale = 0f;
        panelPause.SetActive(true);
        AudioManager.Instance.ReproducirSFX(Sonido_Pausar);
    }


    public void Reanudar()
    {
        if (!Pausable)
            return;

        juegoPausado = false;
        Time.timeScale = 1f;
        panelPause.SetActive(false);
        AudioManager.Instance.ReproducirSFX(Sonido_Click);
    }

    public void Reintentar()
    {
        // La medalla del intento anterior deja de importar
        medallaActual = Medalla.Ninguna;

        // El jugador vuelve a poder pausar
        Pausable = true;

        // Quitar el estado de pausa de la victoria
        juegoPausado = false;
        panelVictory.SetActive(false);
        AudioManager.Instance.ReproducirSFX(Sonido_Click);
        Reanudar();
        ReiniciarCronometro();
        ReiniciarBola();
    }

    // Método para el botón EXIT / SALIR (Va al inicio del Menú Principal)
    public void Salir()
    {
        AudioManager.Instance.ReproducirSFX(Sonido_Salir);
        Time.timeScale = 1f;
        NivelSeleccionado.irASeleccionNivel = false; // No abre el selector directamente
        SceneManager.LoadScene("1-Menu Principal");
    }

    // Método para el botón SELECT LEVEL (Va directamente al selector)
    public void BotonSeleccionarNivel()
    {
        AudioManager.Instance.ReproducirSFX(Sonido_Salir);
        Time.timeScale = 1f;
        NivelSeleccionado.irASeleccionNivel = true; // Activa la bandera
        SceneManager.LoadScene("1-Menu Principal");
    }
    public void MostrarVictoria()
    {
        Pausable = false;
        panelVictory.SetActive(true);

        bola.vivo = false;

        // Guardar el mejor tiempo inmediatamente
        GuardarMejorTiempo(indiceNivel, tiempoNivel);

        Time.timeScale = 0f;

        StartCoroutine(CalcularMedalla());
    }
    public void CargarNivel(int indice)
    {
        Pausable = true;
        if (nivelActual != null)
            Destroy(nivelActual);

        AudioManager.Instance.ReproducirSFX(Sonido_NextLevel);
        nivelActual = Instantiate(niveles[indice]);
        levelActual = nivelActual.GetComponent<Level>();
        gameManagerSkybox.AplicarSkyboxPorNivel(indice + 1);

        if (indice <= 11)
        {
            AudioManager.Instance.ReproducirMusica(Musica1);
        }
        else if (indice <= 22)
        {
            AudioManager.Instance.ReproducirMusica(Musica2);
        }
        else
        {
            AudioManager.Instance.ReproducirMusica(MusicaFinal);
        }

        medallaActual = Medalla.Ninguna;
        panelVictory.SetActive(false);
        Time.timeScale = 1f;
        ActualizarTiempoMedallas();
        Desactivar_Botones_Victoria();
        ReiniciarCronometro();
        ReiniciarBola();
    }
    public void ReiniciarBola()
    {
        Pausable = true;
        // Asegurarnos de que el piso esté activo
        if (levelActual.pisoCollider.enabled == false)
            levelActual.pisoCollider.enabled = true;

        Rigidbody rb = bola.GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = levelActual.spawn.position;
        rb.rotation = levelActual.spawn.rotation;

        bola.vivo = true;
        bola.cayendoEnHoyo = false;

        bola.inclinacionX = 0f;
        bola.inclinacionZ = 0f;

        panelVictory.SetActive(false);
        Desactivar_Botones_Victoria();
        MostrarCronometro();

        rb.WakeUp();
    }
    public void CompletarNivel()
    {
        indiceNivel++;

        if (indiceNivel >= niveles.Length)
        {
            Debug.Log("Juego completado");
            return;
        }

        CargarNivel(indiceNivel);
    }


    private void ActualizarTextoTiempo()
    {
        int minutos = Mathf.FloorToInt(tiempoNivel / 60);

        int segundos = Mathf.FloorToInt(tiempoNivel % 60);

        int centesimas = Mathf.FloorToInt((tiempoNivel * 100) % 100);

        textoTiempo.text =
            minutos.ToString("00") + ":" +
            segundos.ToString("00") + ":" +
            centesimas.ToString("00");
    }


    public void ReiniciarCronometro()
    {
        tiempoNivel = 0f;

        cronometroActivo = true;
    }


    public void DetenerCronometro()
    {
        cronometroActivo = false;
    }


    public void MostrarCronometro()
    {
        PanelCronometro.SetActive(true);
    }


    public void OcultarCronometro()
    {
        PanelCronometro.SetActive(false);
    }


    // =========================================================
    // GUARDADO DE MEDALLAS
    // =========================================================

    private void GuardarMedalla(int nivel, Medalla nuevaMedalla)
    {
        string clave = "RollingMaze_Nivel_" + nivel;

        // Obtener la mejor medalla conseguida anteriormente
        Medalla mejorMedalla = CargarMedalla(nivel);

        // Solo entregar puntos si esta medalla es nueva
        if ((int)nuevaMedalla > (int)mejorMedalla)
        {
            PlayerPrefs.SetInt(clave, (int)nuevaMedalla);

            // La nueva medalla entrega su valor completo
            int puntosGanados = (int)nuevaMedalla;

            int puntosActuales = PlayerPrefs.GetInt("RollingMaze_Puntos", 0);

            puntosActuales += puntosGanados;

            PlayerPrefs.SetInt("RollingMaze_Puntos",puntosActuales);

            PlayerPrefs.Save();

            Debug.Log(
                "Nueva mejor medalla en nivel " +
                nivel +
                ": " +
                nuevaMedalla +
                " | Puntos ganados: " +
                puntosGanados +
                " | Puntos totales: " +
                puntosActuales
            );
        }
    }
    private Medalla CargarMedalla(int nivel)
    {
        string clave = "RollingMaze_Nivel_" + nivel;

        return (Medalla)PlayerPrefs.GetInt(
            clave,
            (int)Medalla.Ninguna
        );
    }


    // =========================================================
    // CALCULAR MEDALLA DEL INTENTO ACTUAL
    // =========================================================

    private IEnumerator CalcularMedalla()
    {
        // Empezar este intento sin ninguna medalla
        medallaActual = Medalla.Ninguna;

        TuTiempo.text = textoTiempo.text;


        // BRONCE
        yield return new WaitForSecondsRealtime(0.4f);

        if (tiempoNivel <= levelActual.tiempoBronce)
        {
            medallaActual = Medalla.Bronce;

            ActivarMedalla(MedallaBronce);

            AudioManager.Instance.ReproducirSFX(sonidoMedalla, 1.0f);
        }


        // PLATA
        yield return new WaitForSecondsRealtime(0.4f);

        if (tiempoNivel <= levelActual.tiempoPlata)
        {
            medallaActual = Medalla.Plata;

            ActivarMedalla(MedallaPlata);

            AudioManager.Instance.ReproducirSFX(sonidoMedalla, 1.5f);
        }


        // ORO
        yield return new WaitForSecondsRealtime(0.4f);

        if (tiempoNivel <= levelActual.tiempoOro)
        {
            medallaActual = Medalla.Oro;

            ActivarMedalla(MedallaOro);
            AudioManager.Instance.ReproducirSFX(Celebracion_ORO);
        }


        // Guardar solamente si es la mejor medalla histórica
        GuardarMedalla(indiceNivel, medallaActual);


        yield return new WaitForSecondsRealtime(0.2f);

        Activar_Botones_Victoria();
    }

    // =========================================================
    // MEDALLAS VISUALES
    // =========================================================

    private void ActivarMedalla(GameObject medalla)
    {
        Image imagen = medalla.GetComponent<Image>();

        Color color = imagen.color;

        color.a = 1f;

        imagen.color = color;
    }
    private void DesactivarMedalla(GameObject medalla)
    {
        Image imagen = medalla.GetComponent<Image>();

        Color color = imagen.color;

        color.a = 0.4f;

        imagen.color = color;
    }


    // =========================================================
    // MOSTRAR MEJORES MEDALLAS DEL NIVEL
    // =========================================================

    private void ActualizarTiempoMedallas()
    {
        // Primero apagar todas
        DesactivarMedalla(MedallaBronce);
        DesactivarMedalla(MedallaPlata);
        DesactivarMedalla(MedallaOro);


        // Mostrar tiempos necesarios
        TiempoMedallaBronce.text =
            levelActual.tiempoBronce.ToString("F2");

        TiempoMedallaPlata.text =
            levelActual.tiempoPlata.ToString("F2");

        TiempoMedallaOro.text =
            levelActual.tiempoOro.ToString("F2");


        // Cargar mejor medalla histórica
        Medalla medallaGuardada = CargarMedalla(indiceNivel);


        // Si tiene Bronce o superior
        if (medallaGuardada >= Medalla.Bronce)
            ActivarMedalla(MedallaBronce);


        // Si tiene Plata o superior
        if (medallaGuardada >= Medalla.Plata)
            ActivarMedalla(MedallaPlata);


        // Si tiene Oro
        if (medallaGuardada >= Medalla.Oro)
            ActivarMedalla(MedallaOro);
    }
    private void GuardarMejorTiempo(int nivel, float nuevoTiempo)
    {
        string clave = "RollingMaze_Tiempo_" + nivel;

        // Si nunca se ha guardado un tiempo, guardar directamente
        if (!PlayerPrefs.HasKey(clave))
        {
            PlayerPrefs.SetFloat(clave, nuevoTiempo);
            PlayerPrefs.Save();

            Debug.Log(
                "Primer tiempo guardado en nivel " +
                nivel +
                ": " +
                nuevoTiempo
            );

            return;
        }

        // Cargar el mejor tiempo anterior
        float mejorTiempo = PlayerPrefs.GetFloat(clave);

        // Solo reemplazarlo si el nuevo tiempo es mejor
        if (nuevoTiempo < mejorTiempo)
        {
            PlayerPrefs.SetFloat(clave, nuevoTiempo);
            PlayerPrefs.Save();

            Debug.Log(
                "Nuevo mejor tiempo en nivel " +
                nivel +
                ": " +
                nuevoTiempo
            );
        }
    }

    // =========================================================
    // BOTONES DE VICTORIA
    // =========================================================

    private void Activar_Botones_Victoria()
    {
        PanelCronometro.SetActive(false);

        BotonSalir.SetActive(true);
        BotonRepetirNivel.SetActive(true);

        if (indiceNivel < niveles.Length - 1)
        {
            BotonSiguienteNivel.SetActive(true);
            ThanksForPlaying.SetActive(false);
        }
        else
        {
            BotonSiguienteNivel.SetActive(false);
            ThanksForPlaying.SetActive(true);
        }
    }
    private void Desactivar_Botones_Victoria()
    {
        PanelCronometro.SetActive(true);
        BotonSalir.SetActive(false);
        ThanksForPlaying.SetActive(false);
        BotonRepetirNivel.SetActive(false);
        BotonSiguienteNivel.SetActive(false);
    }
}