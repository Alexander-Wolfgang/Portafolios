using UnityEngine;

public class CameraRootController : MonoBehaviour
{
    public Transform bola;

    public float velocidadSeguimiento = 8f;

    void LateUpdate()
    {
        if (bola == null)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            bola.position,
            velocidadSeguimiento * Time.deltaTime
        );
    }
}