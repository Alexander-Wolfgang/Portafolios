using System.Collections;
using UnityEngine;

public class Explosivo : MonoBehaviour
{
    public enum Tipo_Explosivo
    {
        Claymore,
        Dinamita
    }

    [Header("Tipo")]
    public Tipo_Explosivo tipoExplosivo;

    [Header("Explosion")]
    private float fuerzaExplosion = 110f;
    private float radioExplosion = 10f;
    private float duracionStun = 2.5f;

    [Header("Dinamita")]
    private float tiempoDetonacion = 3f; //Tiempo que tarda en detonar la dinamita
    private float tiempoCooldown = 3f;   //Tiempo que tarda en volver a estar disponible la dinamita

    [Header("Sonidos")]
    public AudioClip pitidoLeve;
    public AudioClip pitidoFuerte;
    public AudioClip sonidoExplosion;

    [Header("Animacion Explosion")]
    public GameObject animacionExplosion;

    [Header("Audio")]
    public AudioSource audioSource;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        BallGravityController bola = other.GetComponent<BallGravityController>();

        if (bola == null)
            return;

        if (!bola.vivo)
            return;

        if (activado)
            return;

        activado = true;

        if (tipoExplosivo == Tipo_Explosivo.Claymore)
        {
            Explode(bola);
        }
        else if (tipoExplosivo == Tipo_Explosivo.Dinamita)
        {
            StartCoroutine(DetonarDinamita(bola));
        }
    }
    public void DetonarClaymore(BallGravityController bola)
    {
        if (tipoExplosivo != Tipo_Explosivo.Claymore)
            return;

        if (!bola.vivo)
            return;

        if (activado)
            return;

        activado = true;

        Explode(bola);
    }
    private IEnumerator DetonarDinamita(BallGravityController bola)
    {
        // Primer pitido
        ReproducirSonido(pitidoLeve);

        yield return new WaitForSeconds(1f);

        // Segundo pitido
        ReproducirSonido(pitidoLeve);

        yield return new WaitForSeconds(1f);

        // Pitido fuerte
        ReproducirSonido(pitidoFuerte);

        yield return new WaitForSeconds(1f);

        // Explosion
        Explode(bola);

        // Cooldown de 5 segundos
        yield return new WaitForSeconds(tiempoCooldown);

        // La dinamita vuelve a estar disponible
        activado = false;
    }

    private void Explode(BallGravityController bola)
    {
        Transform punto = transform;

        // Distancia entre la dinamita y la bola
        float distancia = Vector3.Distance(
            punto.position,
            bola.transform.position
        );

        // Calcula cuánto efecto tiene la explosión según la distancia
        float porcentajeFuerza = 1f - (distancia / radioExplosion);

        // Si está fuera del radio, el efecto físico es 0
        porcentajeFuerza = Mathf.Clamp01(porcentajeFuerza);

        Vector3 direccion = bola.transform.position - punto.position;

        if (direccion.sqrMagnitude < 0.001f)
        {
            direccion = Vector3.up;
        }
        else
        {
            direccion.Normalize();
        }

        // Aplicar fuerza solamente si está dentro del radio
        if (porcentajeFuerza > 0f)
        {
            Rigidbody rb = bola.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.AddForce(
                    direccion * fuerzaExplosion * porcentajeFuerza,
                    ForceMode.Impulse
                );
            }

            // Stun también disminuye con la distancia
            float stunReal = duracionStun * porcentajeFuerza;

            if (stunReal > 0.05f)
            {
                bola.stun = true;

                StartCoroutine(
                    QuitarStun(
                        bola,
                        stunReal
                    )
                );
            }
        }

        // La explosión visual y el sonido ocurren SIEMPRE
        ReproducirSonido(sonidoExplosion);

        if (animacionExplosion != null)
        {
            Instantiate(
                animacionExplosion,
                transform.position + Vector3.up * 1.0f,
                Quaternion.identity
            );
        }
    }

    private IEnumerator QuitarStun(BallGravityController bola,float tiempoStun)
    {
        yield return new WaitForSeconds(tiempoStun);

        if (bola != null)
        {
            bola.stun = false;
        }
    }

    private void ReproducirSonido(AudioClip sonido)
    {
        if (audioSource == null)
            return;

        if (sonido == null)
            return;

        audioSource.PlayOneShot(sonido);
    }
}