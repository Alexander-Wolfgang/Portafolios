using UnityEngine;

public class Meta : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BallGravityController bola = other.GetComponent<BallGravityController>();

        if (bola == null)
            return;

        if (!bola.vivo)
            return;

        GameManager.Instance.DetenerCronometro();
        GameManager.Instance.OcultarCronometro();
        GameManager.Instance.MostrarVictoria();
    }
}