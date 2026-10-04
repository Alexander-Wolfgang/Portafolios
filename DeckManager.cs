using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    [SerializeField] private List<CardData> extraCards = new List<CardData>();

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

    // ========================
    // AGREGAR CARTAS GANADAS
    // ========================
    public void AddCardToDeck(CardData cardData)
    {
        if (cardData == null)
        {
            Debug.LogError("❌ CardData NULL");
            return;
        }

        extraCards.Add(cardData);

        Debug.Log($"✅ Carta agregada: {cardData.cardName}");
    }

    // ========================
    // OBTENER CARTAS EXTRA
    // ========================
    public List<CardData> GetAllCards()
    {
        return new List<CardData>(extraCards);
    }

    // ========================
    // DEBUG
    // ========================
    public int GetDeckSize()
    {
        return extraCards.Count;
    }

    public bool HasDeck()
    {
        return extraCards.Count > 0;
    }

    public void PrintDeckContents()
    {
        Debug.Log("=== CARTAS EXTRA ===");

        foreach (var card in extraCards)
        {
            if (card != null)
                Debug.Log(card.cardName);
        }
    }

    // ========================
    // LIMPIAR RUN
    // ========================
    public void ClearDeck()
    {
        extraCards.Clear();
    }
}