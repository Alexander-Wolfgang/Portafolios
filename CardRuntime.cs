using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardRuntime : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler, 
    IPointerExitHandler
{
    [Header("Referencias")]
    [SerializeField] private RectTransform visual; // 🔥 SOLO ESTE SE MUEVE

    [SerializeField] private float hoverGraceTime = 0.1f;
    private float hoverTimer;

    private RectTransform rt;
    public bool exhaustOnUse = false; //Exilio, la carta no vuelve al reiniciar el maso, solo puede con efectos especiales
    public bool purgeOnUse = false; // desaparece completamen, NO VOLVERA. Ideal para cartas genericas o MUY PODEROSAS

    [Header("Hover")]
    public bool isHovered;
    public bool isSelected;
    private Vector2 basePos;
    private Quaternion baseRotation;
    private Vector3 baseScale = Vector3.one;
    float maxHoverY = 40f; // límite absoluto en pantalla

    [SerializeField] private Outline selectionOutline;
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float hoverHeight = 2f;

    [Header("Animación")]
    private Vector3 targetScale;
    private Vector2 targetPos;
    private Quaternion targetRotation;

    [SerializeField] private float lerpSpeed = 8f;

    [Header("Data")]
    public CardData cardData;
    public CombatManager owner;

    [Header("UI")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image Gema;
    [SerializeField] private Image Circulo_Gema;
    [SerializeField] private Image Num_Gema;
    [SerializeField] private TextMeshProUGUI nameText;
    //[SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        if (visual == null)
        {
            Debug.LogError("Visual no asignado en " + gameObject.name);
        }

        targetScale = Vector3.one;
        targetPos = Vector2.zero;
        targetRotation = Quaternion.identity;
    }

    void Update()
    {
        if (isHovered)
        {
            hoverTimer = hoverGraceTime;
        }
        else
        {
            hoverTimer -= Time.deltaTime;
        }

        bool hoverActive = hoverTimer > 0.08f || isSelected;

        if (visual == null) return;

        Vector2 finalPos = basePos;
        Quaternion finalRot = baseRotation;
        Vector3 finalScale = baseScale;

        if (hoverActive)
        {
            float extraLift = Mathf.Abs(basePos.x) * 0.01f; //0.03 originalmente, para las cartas curvas

            float targetY = basePos.y + hoverHeight + extraLift;

            // 🔥 CLAMP CLAVE
            targetY = Mathf.Min(targetY, maxHoverY);

            finalPos = new Vector2(basePos.x, targetY);
        }

        visual.anchoredPosition = Vector2.Lerp(visual.anchoredPosition, finalPos, Time.deltaTime * lerpSpeed);
        visual.localRotation = visual.localRotation = Quaternion.identity;
        //visual.localRotation = Quaternion.Lerp(visual.localRotation, finalRot, Time.deltaTime * lerpSpeed);
        visual.localScale = Vector3.Lerp(visual.localScale, finalScale, Time.deltaTime * lerpSpeed);
    }

    // =========================
    // INPUT
    // =========================

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Ignorar input si NO es el turno del jugador
        if (owner == null || owner.state != CombatManager.CombatState.PlayerTurn)
            return;

        if (owner.deck.currentHovered != null &&
            owner.deck.currentHovered != this)
            return;

        owner.deck.currentHovered = this;
        isHovered = true;

        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Ignorar si NO es el turno del jugador
        if (owner == null || owner.state != CombatManager.CombatState.PlayerTurn)
            return;

        isHovered = false;

        if (owner.deck.currentHovered == this)
            owner.deck.currentHovered = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Ignorar clicks fuera del turno del jugador
        if (owner == null || owner.state != CombatManager.CombatState.PlayerTurn)
            return;

        if (owner.IsSelectingCardToSacrifice())
        {
            // highlight, sonido, etc
        }
        if (owner != null && owner.IsSelectingCardToSacrifice())        //Modo Sacrificio (CORREGIDO)
        {
            owner.SelectCardToSacrifice(this);
            return;
        }

        // VALIDACIÓN CLAVE
        if (!owner.deck.IsInHand(this))
            return;

        owner.PlayCard(this, owner);
    }

    // =========================
    // LAYOUT (LO USA EL DECK)
    // =========================

    public void SetBaseTransform(Vector2 pos, float rotationZ)
    {
        basePos = pos;
        baseRotation = Quaternion.Euler(0, 0, rotationZ);
    }

    // =========================
    // INIT / UI
    // =========================

    public void Initialize(CardData data, CombatManager ownerCharacter)
    {
        cardData = data;
        owner = ownerCharacter;

        UpdateVisual();
        gameObject.SetActive(false);
    }

    public void Mostrar()
    {
        gameObject.SetActive(true);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (cardData == null) return;

        // COSTO / NÚMERO DE GEMA

        int Costo = cardData.cost;
        Sprite spriteCosto = Resources.Load<Sprite>($"Imagenes/{Costo}");

        if (spriteCosto != null)
        {
            Num_Gema.sprite = spriteCosto;
        }
        else
        {
            Debug.LogWarning($"No se encontró la imagen del costo: Resources/Imagenes/{Costo}");
        }

        // DATOS DE LA CARTA

        descriptionText.text = cardData.description;

        artworkImage.sprite = cardData.artwork;
        borderImage.sprite = cardData.border;
    }
    public void OnDiscard()
    {
        // Bloquear input
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        isHovered = false;
        gameObject.SetActive(false);
    }
    void OnDisable()
    {
        if (owner != null && owner.deck.currentHovered == this)
            owner.deck.currentHovered = null;
    }
    public void SetSelected(bool value)
    {
        isSelected = value;

        Debug.Log($"SetSelected {cardData.cardName}: {value}");
        Debug.Log("Activar brillo");

        if (selectionOutline != null)
            selectionOutline.enabled = value;
    }
}