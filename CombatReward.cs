using UnityEngine;

public class CombatReward : MonoBehaviour
{
    [SerializeField] private CardData Card;
    [SerializeField] private PotionData Object1;
    [SerializeField] private PotionData Object2;
    [SerializeField] private CardData Relic;

    public void TakeCard()
    {
        if (Card == null)
        {
            Debug.LogError("❌ Card es NULL");
            return;
        }
        DeckManager.Instance.AddCardToDeck(Card);
        Debug.Log($"✅ Carta entregada: {Card.cardName}");
    }
    public void TakeObject1()
    {
        if (Object1 == null)
        {
            Debug.LogError("❌ Object1 es NULL");
            return;
        }
        //Agregar el objeto al inventario del jugador
        PotionManager.Instance.AddPotion(Object1);
        //Panel_Caracteristicas_Objetos.Instance.Mostrar(Object1.description);
        if (PotionInventoryUI.Instance != null)
        {
            Debug.Log("✅ Actualizando UI de pociones");
            PotionInventoryUI.Instance.RefreshUI();
        }
    }
    public void TakeObject2()
    {
        if (Object2 == null)
        {
            Debug.LogError("❌ Object2 es NULL");
            return;
        }
        //Agregar el objeto al inventario del jugador
        PotionManager.Instance.AddPotion(Object2);
        //Panel_Caracteristicas_Objetos.Instance.MostrarCofre(Object2.description);
    }
    public void TakeRelic()
    {
        if (Relic == null)
        {
            Debug.LogError("❌ Relic es NULL");
            return;
        }
        //Agregar el objeto al inventario del jugador

        //Debug.Log($"✅ Reliquia entregada: {Card.cardName}");
        //Panel_Caracteristicas_Objetos.Instance.MostrarCofre(Relic.description);
    }
}