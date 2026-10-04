using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class HoleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 1. Obtenemos el componente directamente en 'bola'
        BallGravityController bola = other.GetComponent<BallGravityController>();

        // Agregamos una validación por seguridad en caso de que el objeto no tenga el componente
        if (bola == null) return;

        if (!bola.vivo)
        {
            Debug.Log("La bola no esta viva");
            return;
        }

        Level nivel = GameManager.Instance.NivelActual;

        if (nivel == null)
        {
            Debug.LogError("NivelActual es NULL");
            return;
        }

        if (nivel.pisoCollider == null)
        {
            Debug.LogError("pisoCollider es NULL");
            return;
        }

        // --- IMPULSO HACIA EL CENTRO ---
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculamos la dirección desde la bola hacia el centro del trigger (ejes X y Z)
            Vector3 direccionCentro = transform.position - other.transform.position;
            direccionCentro.y = 0; // Mantenemos el impulso puramente horizontal

            // Aplicamos un impulso instantáneo hacia el centro
            float fuerzaImpulso = 2f; // Puedes subir o bajar este valor desde el código si lo notas lento
            rb.AddForce(direccionCentro.normalized * fuerzaImpulso, ForceMode.VelocityChange);
        }
        if(bola.stun == false)
            nivel.pisoCollider.enabled = false;

        bola.ActivarStun();
    }
}
