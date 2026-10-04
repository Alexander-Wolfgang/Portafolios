using UnityEngine;
using System.Collections;
public class ChestReward : MonoBehaviour
{
    [Header("Estado")]
    public bool opened = false;

    [Header("Sprites")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openedSprite;

    private SpriteRenderer sr;

    [Header("Rewards")]
    [SerializeField] private CardData rewardCard;
    [SerializeField] private PotionData rewardPotion;
    //[SerializeField] private PotionData rewardRelics;

    [Header("SFX")]
    [SerializeField] private AudioSource SFX_Chest;
    private Rigidbody2D rb;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (closedSprite != null)
            sr.sprite = closedSprite;
    }
    public void Hit()
    {
        if (opened)
            return;

        // Para el prototipo sera 10, pero tiene que ser ajustable en el futuro, usaremos Tamaño_Inventario del RPD
        if (PotionManager.Instance.GetPotions().Count >= 10)
        {
            Panel_Caracteristicas_Objetos.Instance.MensajeDirecto("Inventario lleno");
            return;
        }

        opened = true;

        // Salto
        rb.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);

        StartCoroutine(AbrirCofre());
    }
    IEnumerator AbrirCofre()
    {
        // Esperar a que el cofre alcance un poco de altura
        yield return new WaitForSeconds(0.15f);

        SFX_Chest.Play();
        Debug.Log("🧰 Cofre abierto");

        // Cambiar sprite
        if (openedSprite != null)
            sr.sprite = openedSprite;

        // Entregar carta
        if (rewardCard != null)
        {
            DeckManager.Instance.AddCardToDeck(rewardCard);
            Debug.Log($"🎁 Carta obtenida: {rewardCard.cardName}");
            Panel_Caracteristicas_Objetos.Instance.MostrarCofre(1,rewardCard.cardName,rewardCard.description);
        }

        // Entregar poción
        if (rewardPotion != null)
        {
            int i;
            i = PotionManager.Instance.AddPotion(rewardPotion);
            if (i == 0)
                Panel_Caracteristicas_Objetos.Instance.MensajeDirecto("Bolso lleno, no puedes llevar más objetos");
            else
            {
                Debug.Log("Instance = " + Panel_Caracteristicas_Objetos.Instance);

                if (Panel_Caracteristicas_Objetos.Instance == null)
                {
                    Debug.LogError("INSTANCE ES NULL");
                }
                else
                {
                    Panel_Caracteristicas_Objetos.Instance.MostrarCofre(2,rewardPotion.potionName,rewardPotion.description);
                }
            }
        }
        // Entregar Reliquia
        
        //Copiar script anterior

        StartCoroutine(Destruir());
    }
    IEnumerator Destruir()
    {
        yield return new WaitForSeconds(1.5f);
        Panel_Caracteristicas_Objetos.Instance.Apagar();
        Destroy(gameObject);
    }
}