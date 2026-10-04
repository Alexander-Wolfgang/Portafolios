using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static class SceneNames
    {
        //public const string MainMenu = "1-MainMenu";
        public const string Gameplay = "Gameplay";
    }


    [Header("Definir paneles de la escena")]
    public RectTransform Panel_PressStart;
    public RectTransform Panel_MainMenu;
    public RectTransform Panel_Niveles;
    public RectTransform Panel_SpeedRun;
    public RectTransform Panel_Tienda;
    public RectTransform Panel_Salir;


    [Header("Sonidos")]
    public AudioClip sonido_PressStart;
    public AudioClip sonido_Confirmacion;
    public AudioClip sonido_Denegacion;
    public AudioClip sonido_Play;

    [Header("Música")]
    public AudioClip musicaMenu;
    public AudioClip musicaTienda;

    [Header("Información del nivel")]
    // TMP que muestra "Nivel 01"
    public TextMeshProUGUI textoNivelSeleccionado;

    // TMP que muestra el mejor tiempo
    public TextMeshProUGUI textoMejorTiempo;

    // Medallas
    public Image medallaBronce;
    public Image medallaPlata;
    public Image medallaOro;

    [Header("Seleccion de niveles")]
    public SelectorNivel[] selectoresNivel;

    // =========================================================
    // MENÚ PRINCIPAL
    // =========================================================

    private void Start()
    {
        AudioManager.Instance.ReproducirMusica(musicaMenu);
        // Comprobar si venimos desde el botón "Select Level" del menú de pausa
        if (NivelSeleccionado.irASeleccionNivel)
        {
            // Consumimos la bandera para evitar que vuelva a abrirse al reiniciar
            NivelSeleccionado.irASeleccionNivel = false;

            // Ocultamos las pantallas de inicio
            OcultarPanel(Panel_PressStart);
            OcultarPanel(Panel_MainMenu);

            // Abrimos directamente el panel de selección de niveles
            MostrarPanel(Panel_Niveles);
            ActualizarNivelesDesbloqueados();
            MostrarInformacionNivel(1);
        }
        else
        {
            // Estado por defecto al iniciar el juego o desde el botón Salir/Exit
            MostrarPanel(Panel_PressStart);
            OcultarPanel(Panel_MainMenu);
            OcultarPanel(Panel_Niveles);
        }
    }
    public void PressStart() // Clic
    {
        AudioManager.Instance.ReproducirSFX(sonido_PressStart);

        OcultarPanel(Panel_PressStart);
        MostrarPanel(Panel_MainMenu);
    }
    public void Jugar() // Clic
    {
        AudioManager.Instance.ReproducirSFX(sonido_Play);

        int nivelAContinuar = 0;

        // Buscar el primer nivel que no tenga ninguna medalla
        for (int i = 0; i < 24; i++)
        {
            string claveMedalla = "RollingMaze_Nivel_" + i;

            int medalla = PlayerPrefs.GetInt(claveMedalla, 0);

            if (medalla == 0)
            {
                nivelAContinuar = i;
                break;
            }

            // Si llegamos al último y también está completado,
            // volveremos al nivel 1.
            if (i == 23)
            {
                nivelAContinuar = 0;
            }
        }

        NivelSeleccionado.indiceNivel = nivelAContinuar;
        NivelSeleccionado.tipoPartida = TipoPartida.NivelNormal;

        OcultarPanel(Panel_MainMenu);

        SceneManager.LoadScene(SceneNames.Gameplay);
    }
    public void Seleccionar_Escena()
    {
        AudioManager.Instance.ReproducirSFX(sonido_Confirmacion);

        OcultarPanel(Panel_MainMenu);
        MostrarPanel(Panel_Niveles);

        ActualizarNivelesDesbloqueados();

        MostrarInformacionNivel(1);
    }
    public void Salir_Seleccionar_Escena() // Clic
    {
        AudioManager.Instance.ReproducirSFX(sonido_Denegacion);

        OcultarPanel(Panel_Niveles);
        MostrarPanel(Panel_MainMenu);
    }
    public void Seleccionar_SpeedRun() // Clic
    {
        AudioManager.Instance.ReproducirSFX(sonido_Confirmacion);

        OcultarPanel(Panel_MainMenu);
        MostrarPanel(Panel_SpeedRun);
    }
    public void Salir_Seleccionar_SpeedRun() // Clic
    {
        AudioManager.Instance.ReproducirSFX(sonido_Denegacion);

        OcultarPanel(Panel_SpeedRun);
        MostrarPanel(Panel_MainMenu);
    }

    public void Tienda()
    {
        AudioManager.Instance.ReproducirSFX(sonido_Confirmacion);

        AudioManager.Instance.ReproducirMusica(musicaTienda);

        OcultarPanel(Panel_MainMenu);
        MostrarPanel(Panel_Tienda);
    }
    public void Salir_Tienda()
    {
        AudioManager.Instance.ReproducirSFX(sonido_Denegacion);

        AudioManager.Instance.ReproducirMusica(musicaMenu);

        OcultarPanel(Panel_Tienda);
        MostrarPanel(Panel_MainMenu);
    }
    public void Salir() // Clic
    {
        AudioManager.Instance.ReproducirSFX(sonido_Denegacion);

        OcultarPanel(Panel_MainMenu);
        MostrarPanel(Panel_Salir);
    }
    public void Cerrar() // Clic
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


    public void Cancelar_salir() // Clic
    {
        AudioManager.Instance.ReproducirSFX(sonido_Denegacion);

        OcultarPanel(Panel_Salir);
        MostrarPanel(Panel_MainMenu);
    }


    // =========================================================
    // SELECCIONAR NIVEL
    // =========================================================

    public void MostrarInformacionNivel(int numeroNivel)
    {
        // El numero visual es 1, 2, 3...
        // El indice interno es 0, 1, 2...
        int indice = numeroNivel - 1;


        // =====================================================
        // NOMBRE DEL NIVEL
        // =====================================================

        textoNivelSeleccionado.text =
            "Nivel " + numeroNivel.ToString("00");


        // =====================================================
        // MEJOR TIEMPO
        // =====================================================

        string claveTiempo = "RollingMaze_Tiempo_" + indice;

        if (PlayerPrefs.HasKey(claveTiempo))
        {
            float mejorTiempo =
                PlayerPrefs.GetFloat(claveTiempo);

            textoMejorTiempo.text =
                FormatearTiempo(mejorTiempo);
        }
        else
        {
            textoMejorTiempo.text = "--:--:--";
        }

        // =====================================================
        // MEDALLA GUARDADA
        // =====================================================

        string claveMedalla =
            "RollingMaze_Nivel_" + indice;

        int medallaGuardada =
            PlayerPrefs.GetInt(claveMedalla, 0);


        ActualizarMedallas(medallaGuardada);
    }
    private void ActualizarNivelesDesbloqueados()
    {
        int nivelesPasados = 0;

        // Buscar cuántos niveles consecutivos han sido completados
        for (int i = 0; i < 24; i++)
        {
            string claveMedalla = "RollingMaze_Nivel_" + i;

            int medalla = PlayerPrefs.GetInt(claveMedalla, 0);

            if (medalla > 0)
            {
                nivelesPasados++;
            }
            else
            {
                break;
            }
        }

        // El siguiente nivel también queda desbloqueado
        int ultimoNivelDesbloqueado = nivelesPasados + 1;

        // Nunca superar el nivel 24
        if (ultimoNivelDesbloqueado > 24)
        {
            ultimoNivelDesbloqueado = 24;
        }

        // Actualizar visualmente los 24 botones
        for (int i = 0; i < 24; i++)
        {
            int numeroNivel = i + 1;

            bool desbloqueado =
                numeroNivel <= ultimoNivelDesbloqueado;

            selectoresNivel[i].ActualizarEstado(desbloqueado);
        }
    }

    // =========================================================
    // FORMATO DEL TIEMPO
    // =========================================================

    private string FormatearTiempo(float tiempo)
    {
        int minutos =
            Mathf.FloorToInt(tiempo / 60);

        int segundos =
            Mathf.FloorToInt(tiempo % 60);

        int centesimas =
            Mathf.FloorToInt((tiempo * 100) % 100);


        return minutos.ToString("00") + ":" +
               segundos.ToString("00") + ":" +
               centesimas.ToString("00");
    }


    // =========================================================
    // MEDALLAS
    // =========================================================

    private void ActualizarMedallas(int medalla)
    {
        // Primero apagar todas
        CambiarAlpha(medallaBronce, 0.4f);
        CambiarAlpha(medallaPlata, 0.4f);
        CambiarAlpha(medallaOro, 0.4f);


        // Bronce o superior
        if (medalla >= 1)
        {
            CambiarAlpha(medallaBronce, 1f);
        }


        // Plata o superior
        if (medalla >= 2)
        {
            CambiarAlpha(medallaPlata, 1f);
        }


        // Oro
        if (medalla >= 3)
        {
            CambiarAlpha(medallaOro, 1f);
        }
    }


    private void CambiarAlpha(Image imagen, float alpha)
    {
        Color color = imagen.color;

        color.a = alpha;

        imagen.color = color;
    }


    // =========================================================
    // BOTÓN VERDE - JUGAR NIVEL SELECCIONADO
    // =========================================================
    public void JugarSeleccionado()     //Clic
    {
        if (NivelSeleccionado.indiceNivel < 0)
        {
            Debug.Log("No se ha seleccionado ningún nivel.");
            return;
        }

        if (NivelSeleccionado.tipoPartida == TipoPartida.Ninguno)
        {
            Debug.Log("No se ha seleccionado ningún tipo de partida.");
            return;
        }
        AudioManager.Instance.ReproducirSFX(sonido_Play);
        SceneManager.LoadScene(SceneNames.Gameplay);
    }

    // =========================================================
    // MOSTRAR / OCULTAR PANELES
    // =========================================================

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
            Debug.LogWarning(
                "Se pasó un panel nulo a MostrarPanel."
            );
        }
    }
    public void OcultarPanel(RectTransform panel)
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(false);
        }
    }
}