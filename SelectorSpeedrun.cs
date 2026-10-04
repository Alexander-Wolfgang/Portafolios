using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectorSpeedrun : MonoBehaviour
{
    public int numeroSpeedrun;

    public void SeleccionarSpeedrun()
    {
        NivelSeleccionado.indiceNivel = numeroSpeedrun - 1;
        NivelSeleccionado.tipoPartida = TipoPartida.Speedrun;
    }
}