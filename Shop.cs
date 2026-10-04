using UnityEngine;

public class Shop : MonoBehaviour
{
    void Start()
    {
        
    }
    public void BuyCard(CardData card, int cost)
    {
        if (PlayerRunData.Instance.gold >= cost)
        {
            PlayerRunData.Instance.SubtractGold(cost);
            DeckManager.Instance.AddCardToDeck(card);
            Debug.Log($"💳 Compraste: {card.cardName}");
        }
    }
}
