using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LaunchPad : MonoBehaviour
{
    [Header("Lanzamiento")]
    public Vector3 direccion = Vector3.forward;
    public float velocidad = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        // Normalizamos para que la dirección no afecte la velocidad.
        Vector3 direccionNormalizada = direccion.normalized;

        // Aplicar velocidad instantánea.
        rb.linearVelocity = direccionNormalizada * velocidad;
    }
}