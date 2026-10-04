using UnityEngine;

public class CameraTiltController : MonoBehaviour
{
    public BallGravityController controlador;

    public float inclinacionVisual = 18f;
    public float velocidadRotacion = 12f;

    Quaternion rotacionInicial;

    void Start()
    {
        rotacionInicial = transform.localRotation;
    }

    void LateUpdate()
    {
        if (controlador == null)
            return;

        float x = controlador.inclinacionX / controlador.inclinacionMaxima;
        float z = controlador.inclinacionZ / controlador.inclinacionMaxima;

        Quaternion inclinacion =
            Quaternion.Euler(
                -x * inclinacionVisual,
                0f,
                z * inclinacionVisual
            );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            rotacionInicial * inclinacion,
            velocidadRotacion * Time.deltaTime
        );
    }
}