using UnityEngine;
using UnityEngine.UI;

public class SelectorNivel : MonoBehaviour
{
    public int numeroNivel;

    [Header("Referencia")]
    public MainMenu mainMenu;

    [Header("Boton")]
    public Image imagenBoton;

    [Header("Sonido")]
    public AudioClip sonido_SeleccionarNivel;

    [HideInInspector]
    public bool jugable = false;

    public void SeleccionarNivel()
    {
        // Si el nivel está bloqueado, no hacer nada
        if (!jugable)
            return;

        NivelSeleccionado.indiceNivel = numeroNivel - 1;
        NivelSeleccionado.tipoPartida = TipoPartida.NivelNormal;
        AudioManager.Instance.ReproducirSFX(sonido_SeleccionarNivel);
        mainMenu.MostrarInformacionNivel(numeroNivel);
    }

    public void ActualizarEstado(bool desbloqueado)
    {
        jugable = desbloqueado;

        Color color = imagenBoton.color;

        if (jugable)
        {
            color.r = 1f;
            color.g = 1f;
            color.b = 1f;
        }
        else
        {
            color.r = 0.7f;
            color.g = 0.7f;
            color.b = 0.7f;
        }

        // El Alpha siempre queda en 1
        color.a = 1f;

        imagenBoton.color = color;
    }
}