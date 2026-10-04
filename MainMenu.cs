using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // 👈 necesario para IEnumerator
using DG.Tweening; // 👈 necesario para DOTween


public class MainMenu : MonoBehaviour
{
    public static class SceneNames
    {
        public const string MAIN_MENU = "1-MainMenu";
        public const string HUB = "2-Hub Principal";
        public const string Ex = "3-Exploracion";
    }
    private bool Prototipo = true;
    private bool entrandoADungeon = false;

    [Header("Animación de entrada a la cueva")]

    public float zoomOutSize = 5.5f;
    public float zoomOutDuration = 0.3f;

    public float zoomInSize = 0.3f;
    public float zoomInDuration = 1.3f;

    public Vector3 targetPosition; // centro de la puerta


    public Vector2 puertaOffset = new Vector2(0f, 750f);

    [Header("Importar otros scripts")]
    //public GameManager MG;

    [Header("Importar variables ajenas")]
    public bool partida_En_Curso = false;
    //partida_En_Curso = true;

    public int Partidas_Jugadas = 0;

    [Header("Definir paneles de la escena")]
    public RectTransform Panel_PressStart;
    public RectTransform Panel_Slots;
    public RectTransform Panel_Botones;
    public RectTransform Panel_Estadisticas;
    public RectTransform Panel_Novedades;
    public RectTransform Panel_Opciones;

    public RectTransform Panel_TextoBloqueado;
    public CanvasGroup CanvasGroup_TextoBloqueado;
    private Sequence secuenciaMensaje;

    public RectTransform Panel_Continuar_Run;
    public RectTransform Panel_Elegir_Slot;

    [Header("Fondo del menú")]
    public RectTransform fondo;

    [Header("Sonidos")]
    public AudioClip sonidoPressStart;
    public AudioClip sonidoConfirmacion;
    public AudioClip sonidoConfirmacion_Jugar;
    public AudioClip sonido_Entrar_Cueva;

    [Header("sonido Personajes")]
    public AudioClip Sonido_Capyguardian;

    [Header("Fuente de audio principal")]
    public AudioSource audioSource;

    public void Awake()
    {
        Panel_TextoBloqueado.gameObject.SetActive(false);
    }
    public void PressStart() //Clic
    {
        audioSource.PlayOneShot(sonidoPressStart);
        OcultarPanel(Panel_PressStart);
        MostrarPanel(Panel_Botones);
    }
    public void Jugar() //Clic
    {
        if (partida_En_Curso == false)
        {
            if (Partidas_Jugadas == 0)
            {
                //Cargar Escena 3
                StartCoroutine(ZoomBackground(SceneNames.Ex));
            }
            else
            {
                //Cargar Escena 2
                StartCoroutine(ZoomBackground(SceneNames.HUB));
            }
        }
        else
        {
            OcultarPanel(Panel_Botones);
            MostrarPanel(Panel_Continuar_Run);
        }
    }
    public void Reanudar()
    {
        //Cargar escena 3 en la misma zona y lugar que estabas antes, con los datos que se tenian
        //Cargar_Datos()
        StartCoroutine(ZoomBackground(SceneNames.Ex));
    }
    public void Iniciar_nueva_run()
    {
        //Cargar escena 2, borrar datos de la partida en curso
        //Borrar_Datos()
        StartCoroutine(ZoomBackground(SceneNames.HUB));
    }
    public void Cancelar()
    {
        OcultarPanel(Panel_Continuar_Run);
        MostrarPanel(Panel_Botones);
    }

    public void Estadisticas()  //Clic
    {
        MostrarMensajeBloqueado();

        if (Prototipo)
            return;

        OcultarPanel(Panel_Botones);
        MostrarPanel(Panel_Estadisticas);
    }

    public void Salir_Estadisticas() //Clic al boton salir en el panel de Estadisticas
    {
        OcultarPanel(Panel_Estadisticas);
        MostrarPanel(Panel_Botones);
    }

    public void Opciones()  //Clic
    {
        MostrarMensajeBloqueado();
        if (Prototipo)
            return;
        OcultarPanel(Panel_Botones);
        MostrarPanel(Panel_Opciones);
    }
    public void Salir_Opciones() //Clic al boton salir en el panel de Opciones
    {
        OcultarPanel(Panel_Opciones);
        MostrarPanel(Panel_Botones);
    }

    public void Novedades() //Clic
    {
        MostrarMensajeBloqueado();
        if (Prototipo)
            return;
        OcultarPanel(Panel_Botones);
        MostrarPanel(Panel_Novedades);
    }
    public void Salir_Novedades() //Clic al boton salir en el panel de Novedades
    {
        OcultarPanel(Panel_Novedades);
        MostrarPanel(Panel_Botones);
    }

    public void Cambiar_partida()   //Clic
    {
        MostrarMensajeBloqueado();
        if (Prototipo)
            return;
        OcultarPanel(Panel_Botones);
        MostrarPanel(Panel_Slots);
    }

    public void Salir_Slots()
    {
        OcultarPanel(Panel_Slots);
        MostrarPanel(Panel_Botones);
    }

    public void Salir() //Clic
    {
        Debug.Log("Saliendo del juego...");

        // 🧠 Si hay un GameManager asignado, guardar antes de salir
        //GM.GuardarPartida();

        #if UNITY_EDITOR
                // Cierra el modo play en el editor
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Cierra el juego en build
            Application.Quit();
        #endif
    }

    // Función genérica que centra cualquier panel
    public void MostrarPanel(RectTransform panel)
    {
        if (panel != null)
        {
            // Centra el panel en el Canvas
            panel.anchoredPosition = Vector2.zero;
            panel.localScale = Vector3.one;
            panel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Se pasó un panel nulo a MostrarPanel.");
        }
    }

    // Función opcional para ocultar cualquier panel
    public void OcultarPanel(RectTransform panel)
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(false);
        }
    }
    private IEnumerator ZoomBackground(string sceneToLoad)
    {
        if (entrandoADungeon) yield break;
        entrandoADungeon = true;
        OcultarPanel(Panel_Botones);
        audioSource.PlayOneShot(sonido_Entrar_Cueva);

        Vector3 startScale = fondo.localScale;
        Vector2 startPos = fondo.anchoredPosition;

        // 1️⃣ Pequeño zoom OUT (anticipación)
        yield return StartCoroutine(AnimarFondo(
            startScale,
            Vector3.one * 0.95f,
            startPos,
            startPos,
            0.2f
        ));

        yield return new WaitForSeconds(0.03f);

        // 2️⃣ ZOOM IN BRUTAL (entrar a la cueva)
        yield return StartCoroutine(AnimarFondo(
            Vector3.one * 0.95f,
            Vector3.one * 9f,              // 🔥 este valor SÍ importa
            startPos,
            startPos + new Vector2(0, 400), // subir hacia la puerta
            1.4f
        ));
        //Cargar escena
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
    private IEnumerator AnimarFondo(
    Vector3 scaleFrom,
    Vector3 scaleTo,
    Vector2 posFrom,
    Vector2 posTo,
    float duration
)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smooth = Mathf.SmoothStep(0, 1, t);

            fondo.localScale = Vector3.Lerp(scaleFrom, scaleTo, smooth);
            fondo.anchoredPosition = Vector2.Lerp(posFrom, posTo, smooth);

            yield return null;
        }

        fondo.localScale = scaleTo;
        fondo.anchoredPosition = posTo;
    }
    private void MostrarMensajeBloqueado()
    {
        // Si ya había una animación ejecutándose, la cancelamos
        secuenciaMensaje?.Kill();

        Panel_TextoBloqueado.gameObject.SetActive(true);

        CanvasGroup_TextoBloqueado.alpha = 0f;

        // 👇 El panel empieza un poco más pequeño
        Panel_TextoBloqueado.localScale = Vector3.one * 0.9f;

        secuenciaMensaje = DOTween.Sequence();

        // Fade In
        secuenciaMensaje.Append(
            CanvasGroup_TextoBloqueado.DOFade(1f, 0.2f)
        );

        // 👇 Al mismo tiempo que hace el Fade In, crece hasta su tamaño normal
        secuenciaMensaje.Join(
            Panel_TextoBloqueado.DOScale(1f, 0.2f)
        );

        // Espera
        secuenciaMensaje.AppendInterval(2.5f);

        // Fade Out
        secuenciaMensaje.Append(
            CanvasGroup_TextoBloqueado.DOFade(0f, 0.2f)
        );

        secuenciaMensaje.OnComplete(() =>
        {
            Panel_TextoBloqueado.gameObject.SetActive(false);
        });
    }
}
