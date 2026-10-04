using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Referencias")]
    public Transform spawn;
    public Transform goal;
    public MeshCollider pisoCollider;

    [Header("Información")]
    public string nombreNivel;

    [Header("Tiempos")]
    public float tiempoOro;
    public float tiempoPlata;
    public float tiempoBronce;

    [Header("Caída")]
    public float CaidaMuerte;
}