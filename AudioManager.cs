using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource audioMusica;
    public AudioSource audioSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (audioMusica != null)
        {
            audioMusica.Play();
        }
    }

    public void ReproducirSFX(AudioClip sonido)
    {
        if (audioSFX == null)
            return;

        if (sonido == null)
            return;

        audioSFX.PlayOneShot(sonido);
    }

    public void ReproducirSFX(AudioClip sonido, float pitch)
    {
        if (sonido == null)
            return;

        GameObject objetoAudio = new GameObject("SFX_Temporal");

        AudioSource fuente = objetoAudio.AddComponent<AudioSource>();

        fuente.clip = sonido;
        fuente.pitch = pitch;
        fuente.Play();

        Destroy(objetoAudio, sonido.length / pitch);
    }

    public void ReproducirMusica(AudioClip musica)
    {
        if (audioMusica == null)
            return;

        if (musica == null)
            return;

        if (audioMusica.clip == musica && audioMusica.isPlaying)
            return;

        audioMusica.clip = musica;
        audioMusica.Play();
    }
}