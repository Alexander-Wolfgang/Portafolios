using System.Collections;
using UnityEngine;

public class HoleController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float tiempoEsperaReactivacion = 0.5f;

    // Este método lo llamará el objeto "Succion" mediante un script simple o directamente
    public void AplicarSuccion(Rigidbody rb)
    {
        Debug.Log("Entramos a aplicar succion");
        // Tu objeto "Hoyo Trigger" está en el centro, usamos su posición
        Vector3 centro = transform.position;
        Vector3 direccion = centro - rb.transform.position;
        direccion.y = 0; // Mantener la fuerza en el plano horizontal

        float fuerzaAtraccion = 150f; // Puedes ajustar este valor a gusto
        rb.AddForce(direccion.normalized * fuerzaAtraccion, ForceMode.Force);
    }

    // Este método se ejecutará cuando la bola toque el "Hoyo Trigger" central
    public void RegistrarCaida(BallGravityController bola)
    {
        Debug.Log("Entramos a registrar caida");
        if (bola != null && bola.vivo)
        {
            Rigidbody rb = bola.GetComponent<Rigidbody>();
            StartCoroutine(ProcesarCaida(bola, rb));
        }
    }

    private IEnumerator ProcesarCaida(BallGravityController bola, Rigidbody rb)
    {
        Debug.Log("Entramos a procesarCaida");
        if (rb != null)
        {
            // Frenamos la canica en seco para que caiga perfecto por el tubo (Pipe)
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Opcional: Hacerla un poco más pesada temporalmente para que caiga rápido por el "Pipe"
            rb.useGravity = true;
        }

        // Apagamos el colisionador de la BOLA. Al hacer esto, ignorará el plano "Piso" 
        // y caerá visualmente a través de tu cilindro/tubo.
        Collider bolaCollider = bola.GetComponent<Collider>();
        if (bolaCollider != null) bolaCollider.enabled = false;

        bola.ActivarStun();

        yield return new WaitForSeconds(tiempoEsperaReactivacion);

        // --- AQUÍ VA TU LÓGICA DE RESPRAWN ---
        // Ejemplo: reubicar la bola en la posición del objeto "Start" que se ve en tu Hierarchy

        if (bolaCollider != null) bolaCollider.enabled = true;
    }
}
