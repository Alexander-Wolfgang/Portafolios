using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<CardData> initialDeck = new List<CardData>();

    [Header("Mazo (ScriptableObject)")]
    [SerializeField] private DeckInitializer Mazo;

    private List<CardRuntime> drawPile = new List<CardRuntime>();
    private List<CardRuntime> hand = new List<CardRuntime>();
    private List<CardRuntime> discardPile = new List<CardRuntime>();
    private List<CardRuntime> exhaustPile = new List<CardRuntime>();

    [SerializeField] private Transform handParent;
    [SerializeField] private CardRuntime cardPrefab;
    [SerializeField] private Transform cardPool;

    public int HandCount => hand.Count;
    public int DrawPileCount => drawPile.Count;
    public int DiscardCount => discardPile.Count;

    private CombatManager owner;
    public CardRuntime currentHovered;

    [Header("TMP Mazo, cementerio y Exilio")]
    public TMPro.TextMeshProUGUI drawCountText;
    public TMPro.TextMeshProUGUI CementerioCountText;
    public TMPro.TextMeshProUGUI ExilioCountText;

    [SerializeField] private Color normalDeckColor = Color.white;

    // 5-3 cartas
    [SerializeField] private Color dangerDeckColor = new Color(1f, 0.85f, 0.15f);

    //  o menos
    [SerializeField] private Color warningDeckColor = new Color(1f, 0.15f, 0.15f);

    [SerializeField] private float normalFontSize = 18;
    [SerializeField] private float DangerFontSize = 24;
    [SerializeField] private float warningFontSize = 30;
    public void Initialize(CombatManager owner)
    {
        this.owner = owner;

        drawPile.Clear();
        hand.Clear();
        discardPile.Clear();
        exhaustPile.Clear();

        List<CardData> finalDeck = new List<CardData>();

        // =========================
        // 1. MAZO BASE (Capy-Deck)
        // =========================
        if (Mazo != null)
        {
            List<CardData> baseDeck = Mazo.GetStartingCards();

            finalDeck.AddRange(baseDeck);

            Debug.Log($"✅ Mazo base cargado: {baseDeck.Count} cartas");
        }
        else
        {
            Debug.LogError("❌ Mazo (DeckInitializer) es NULL");
        }

        // =========================
        // 2. CARTAS GANADAS DURANTE LA RUN
        // =========================
        if (DeckManager.Instance != null && DeckManager.Instance.HasDeck())
        {
            List<CardData> extraCards = DeckManager.Instance.GetAllCards();

            finalDeck.AddRange(extraCards);

            Debug.Log($"✅ Cartas extra agregadas: {extraCards.Count}");
        }

        // =========================
        // 3. INICIALIZAR MAZO FINAL
        // =========================
        InitializeFromCardDataList(finalDeck);

        Debug.Log($"✅ Mazo final: {finalDeck.Count} cartas");
        ContMazo();
        ContCementerio();
        ContExilio();
    }

    // 🔥 NUEVO: Inicializar desde lista de CardData
    private void InitializeFromCardDataList(List<CardData> cardDataList)
    {
        foreach (var cardData in cardDataList)
        {
            CardRuntime runtime = Instantiate(cardPrefab, cardPool);
            runtime.Initialize(cardData, owner);
            drawPile.Add(runtime);
        }

        Shuffle(drawPile);

        ContMazo();
        ContCementerio();
        ContExilio();

        // 🔥 DEBUG: Saber qué mazo se está usando
        if (DeckManager.Instance != null && DeckManager.Instance.HasDeck())
        {
            Debug.Log($"✅ USANDO DECKMANAGER: {drawPile.Count} cartas");
        }
        else
        {
            Debug.Log($"⚠️ USANDO initialDeck (FALLBACK): {drawPile.Count} cartas");
        }
    }

    public void Draw(int amount)
    {
        int cartasARobar = Mathf.Min(amount, drawPile.Count);

        for (int i = 0; i < cartasARobar; i++)
        {
            CardRuntime card = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(card);

            card.transform.SetParent(handParent, false);

            RectTransform rt = card.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            card.Mostrar();
        }

        ContMazo();
        UpdateHandLayout();
    }

    public void Discard(CardRuntime card)
    {
        Debug.Log("entra en discard");
        hand.Remove(card);
        discardPile.Add(card);

        card.OnDiscard();

        owner.UpdateReviveButton();

        ContCementerio();
        UpdateHandLayout();
    }

    public void Exhaust(CardRuntime card)
    {
        hand.Remove(card);
        exhaustPile.Add(card);

        card.gameObject.SetActive(false);

        ContExilio();
        UpdateHandLayout();
    }

    public void SacrificeRandom()
    {
        if (hand.Count == 0)
            return;

        int index = Random.Range(0, hand.Count);

        CardRuntime card = hand[index];

        hand.RemoveAt(index);
        discardPile.Add(card);

        card.transform.SetParent(cardPool, false);

        ContCementerio();
        UpdateHandLayout();
    }

    private void Shuffle(List<CardRuntime> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void RevivirCartas()
    {
        if (discardPile.Count == 0)
            return;

        drawPile.AddRange(discardPile);
        discardPile.Clear();

        Shuffle(drawPile);

        owner.UpdateReviveButton();

        ContMazo();
        ContCementerio();
    }

    public void UpdateHandLayout()
    {
        int count = hand.Count;
        if (count == 0) return;

        float spacing = 75f;
        float curveHeight = 8f;
        float maxRotation = -5f;
        //float curveHeight = 0f;
        //float maxRotation = 0f;

        for (int i = 0; i < count; i++)
        {
            if (hand[i].isHovered)
                continue;

            RectTransform rt = hand[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            float x = (i - (count - 1) / 2f) * spacing;

            float t = count == 1 ? 0.5f : (float)i / (count - 1);

            float rot = Mathf.Lerp(-maxRotation, maxRotation, t);

            float y = Mathf.Sin(t * Mathf.PI) * curveHeight;

            // compensación por rotación
            y += Mathf.Abs(rot) * 1.2f;

            hand[i].SetBaseTransform(new Vector2(x, y), rot);
        }
    }

    public void ClearHoverState()
    {
        currentHovered = null;
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].isHovered = false;
        }
        UpdateHandLayout();
    }

    public void ContMazo()
    {
        Debug.Log($"Cartas: {drawPile.Count} - FontSize: {drawCountText.fontSize}");
        if (drawCountText == null)
        {
            Debug.LogWarning("drawCountText es null");
            return;
        }

        drawCountText.text = drawPile.Count.ToString();

        if (drawPile.Count < 6)
        {
            if (drawPile.Count < 3)
            {
                drawCountText.color = warningDeckColor;
                drawCountText.fontSize = warningFontSize;
                drawCountText.fontStyle = FontStyles.Bold;
            }
            else
            {
                drawCountText.color = dangerDeckColor;
                drawCountText.fontSize = DangerFontSize;
                drawCountText.fontStyle = FontStyles.Normal;
            }
        }
        else
        {
            drawCountText.color = normalDeckColor;
            drawCountText.fontSize = normalFontSize;
            drawCountText.fontStyle = FontStyles.Normal;
        }
        Debug.Log($"Cartas: {drawPile.Count} - FontSize: {drawCountText.fontSize}");
    }

    public void ContCementerio()
    {
        if (CementerioCountText != null)
            CementerioCountText.text = discardPile.Count.ToString();
    }

    public void ContExilio()
    {
        if (ExilioCountText != null)
            ExilioCountText.text = exhaustPile.Count.ToString();
    }

    public bool IsInHand(CardRuntime card)
    {
        return hand.Contains(card);
    }

    public void MoveRandomDiscardToDrawPile(int mode)
    {
        if (discardPile.Count == 0)
            return;

        int index = Random.Range(0, discardPile.Count);
        CardRuntime card = discardPile[index];

        discardPile.RemoveAt(index);

        switch (mode)
        {
            case 3:
                drawPile.Insert(0, card);
                break;

            case 2:
                int randomPos = Random.Range(0, drawPile.Count + 1);
                drawPile.Insert(randomPos, card);
                break;

            case 1:
            default:
                drawPile.Add(card);
                break;
        }

        Debug.Log($"Carta {card.cardData.cardName} movida al mazo (modo {mode})");

        ContMazo();
        ContCementerio();
    }

    public void DiscardAllFromHand(CardRuntime source)
    {
        List<CardRuntime> cardsToDiscard = new List<CardRuntime>(hand);

        foreach (var card in cardsToDiscard)
        {
            if (card == source)
                continue;

            Discard(card);
        }
    }

    public void Purge(CardRuntime card)
    {
        hand.Remove(card);
        drawPile.Remove(card);
        discardPile.Remove(card);
        exhaustPile.Remove(card);

        Destroy(card.gameObject);

        UpdateHandLayout();
    }

    // 🔥 GETTERS para acceso externo
    public List<CardRuntime> GetDrawPile()
    {
        return new List<CardRuntime>(drawPile);
    }

    public List<CardRuntime> GetDiscardPile()
    {
        return new List<CardRuntime>(discardPile);
    }

    public List<CardRuntime> GetExhaustPile()
    {
        return new List<CardRuntime>(exhaustPile);
    }
}