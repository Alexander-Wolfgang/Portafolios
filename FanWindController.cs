using System.Collections;
using UnityEngine;

public class FanWindController : MonoBehaviour
{
    [Header("Wind")]
    [SerializeField] private BoxCollider windCollider;
    [SerializeField] private GameObject windParticles;

    [Header("Timing")]
    [SerializeField] private float delayBeforeFirstBurst = 3f;
    [SerializeField] private float windDuration = 1.5f;
    [SerializeField] private float interval = 3f;

    private void Start()
    {
        windCollider.enabled = false;

        if (windParticles != null)
            windParticles.SetActive(false);

        StartCoroutine(WindCycle());
    }

    private IEnumerator WindCycle()
    {
        yield return new WaitForSeconds(delayBeforeFirstBurst);

        while (true)
        {
            // ENCENDER VIENTO
            windCollider.enabled = true;

            if (windParticles != null)
                windParticles.SetActive(true);

            // MANTENER VIENTO
            yield return new WaitForSeconds(windDuration);

            // APAGAR VIENTO
            windCollider.enabled = false;

            if (windParticles != null)
                windParticles.SetActive(false);

            // ESPERAR HASTA LA SIGUIENTE RÁFAGA
            yield return new WaitForSeconds(interval);
        }
    }
}