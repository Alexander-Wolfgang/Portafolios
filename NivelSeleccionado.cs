using UnityEngine;

public static class NivelSeleccionado
{
    public static int indiceNivel = -1;
    public static TipoPartida tipoPartida = TipoPartida.Ninguno;
    public static bool irASeleccionNivel = false;
}

public enum TipoPartida
{
    Ninguno,
    NivelNormal,
    Speedrun
}