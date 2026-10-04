using UnityEngine;

public class LaserClaymore : MonoBehaviour
{
    private Explosivo explosivo;

    private void Awake()
    {
        explosivo = GetComponentInParent<Explosivo>();
    }

    private void OnTriggerEnter(Collider other)
    {
        BallGravityController bola = other.GetComponent<BallGravityController>();

        if (bola == null)
            return;

        explosivo.DetonarClaymore(bola);
    }
}