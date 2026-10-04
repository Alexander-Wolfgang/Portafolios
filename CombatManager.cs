using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using static StatusEffect;

public class CombatManager : MonoBehaviour
{
    public event System.Action<CombatManager, CombatManager> OnShieldBrokenThisTurn;
    public bool IsWaitingForTarget => selectedCard != null;
    private bool isSelectingCardToSacrifice = false;
    public bool cancelCardExecution = false;
    public string characterName = "Personaje";
    public static CombatManager Instance;
    private CardRuntime sacrificeSourceCard;
    public CharacterStats baseStats;
    public PauseManager2 Pause;
    bool esta_pausado = false;
    bool Muerto = false;
    [SerializeField] private AudioSource Musica;
    [SerializeField] private AudioSource SFX_Pausar;
    [SerializeField] private AudioSource SFX_DesPausar;
    [SerializeField] private AudioSource SFX_Potion;
    [SerializeField] private AudioSource Victoria_SFX;
    [SerializeField] private AudioSource Derrota_SFX;
    [SerializeField] private AudioSource LevelUP_SFX;
    private const float NORMAL_VOLUME = 0.2f;
    private const float PAUSE_VOLUME = 0.08f;
    public Enemy Enemy;
    [SerializeField] private CombatReward CR;
    [SerializeField] private GameObject HUD_Combate;
    [SerializeField] private XP xpBar;
    public TMP_Text Oro_Display;
    [SerializeField] private AudioSource Sonido_Carta;
    [SerializeField] private GameObject fuegoArtificial1;
    [SerializeField] private GameObject fuegoArtificial2;
    [SerializeField] private GameObject Confetti1;
    [SerializeField] private GameObject Confetti2;
    [SerializeField] private ParticleSystem Brillito;
    [SerializeField] private DeckInitializer deckInitializer;
    public PlayerStatusUIManager statusManager;
    public List<Enemy> enemiesInCombat;
    public System.Action<int, int> OnHealthChanged;
    public System.Action<int, int> OnManaChanged;
    //public TurnManager turnManager;
    [SerializeField] private Button reviveButton;
    [SerializeField] private Button endTurnButton; // referencia desde inspector (botón de terminar turno)
    public GameObject Mano;

    //Textbox Victory premios
    [SerializeField] private TextMeshProUGUI Recompensa_Carta;
    [SerializeField] private TextMeshProUGUI Recompensa_Oro;
    [SerializeField] private TextMeshProUGUI Recompensa_Objeto1;
    [SerializeField] private TextMeshProUGUI Recompensa_Objeto2;
    [SerializeField] private TextMeshProUGUI Recompensa_Reliquia;

    //Victory/Defeat Panels
    public GameObject CartelVictoria;
    public GameObject CartelLevelUP;
    public GameObject PanelRecompensas;
    public GameObject CartelDerrota;

    public Deck deck;
    public Hand hand;
    public int Fuerza;              // Bonus de daño que se suma a los ataques
    public int Magia;              // Bonus de daño mágico que se suma a los ataques mágicos
    public int Robustez;           // Bonus de escudo que te aplicas
    public bool Hay_Espacio;

    [Header("Stats de Combate")]
    public int maxHealth;
    public int currentHealth;
    public int LimitHP;

    public bool usaEnergia;
    public int energiaBase;
    public int energia_Actual;

    public bool usaMana;
    public int manaMaxima;
    public int manaLimit;
    public int manaCurrent;
    public int manaRegenPorTurno;

    public int roboBase;
    private int roboTemporal; // buffs que duran mientras el estado exista, de normal es 0
    public int manoMaxima;
    public int energiaConservadaPorTurno;
    public int cost_reshuffle;
    private bool Barrera;

    private int energiaInicialBonus;
    private int danoInicialBonus;
    private int roboInicialBonus;
    private int escudoInicial;
    private int CuracionBonus;
    private int FuerzaBonus;
    private int RobustezBonus;
    private int MagiaBonus;
    private int RegenManaBonus;

    public int Potenciar_Duracion_Estado;   //+X turno la duracion de los estados beneficiosos del jugador
    public int Resistir_Estado;             //-X turno la duracion de los estados perjudiciales del jugador

    [Header("Las 10 rayos de energias")]
    public Image[] Rayos; // tamaño 10 en el inspector
    
    [Header("Las 5 Barras de escudo que puedes ponerte")]
    public GameObject Padre_Barras;             // Prefab de la barra de escudo
    public Image[] Barras_Escudo;           // tamaño 5 en el inspector
    public Color color1Turno = Color.red;
    public Color color2Turnos = Color.yellow;
    public Color color3Turnos = Color.green;
    public Color colorVacio = Color.gray;

    [Header("Referencia a las Barras de HP, MP y Energia")]
    public GameObject HPBar;
    public GameObject ManaBar;
    public RectTransform EnergyBar;

    [Header("Botones")]
    public GameObject BotonTomarCarta;
    public GameObject BotonTomarOro;
    public GameObject BotonTomarObjeto1;
    public GameObject BotonTomarObjeto2;
    public GameObject BotonTomarReliquia;

    [Header("Datos de Combate")]
    public int Oro_Enemigos_Acum;
    public int Oro_Actual;
    public int Oro_Moai;
    public int XP_Enemigos_Acum;
    public int Exp_Actual;

    [Header("UI")]
    public Image marcoBarrera;          // el marco se vuelve celeste
    public Image RecubrimientoMagico;   // el marco se vuelve verde
    public AudioSource audioSource;     //Para reproducir los efectos de sonido relacionados con el escudo

    [Header("Shield")]
    public Image Shield;                // La imagen del escudo
    public TMP_Text shieldValue;        // El texto que muestra el valor actual del escudo
    public AudioClip shieldGainSFX;     // Sonido cuando ganas escudo
    public AudioClip shieldHitSFX;      // Sonido cuando el Sonido recibe daño
    public AudioClip shieldBreakSFX;    // Sonido cuando se rompe el escudo

    [Header("Booleanos de estados especiales")]
    public bool barrera;                //Tienes una barrera activa, te protegera de un ataque
    public bool Magia_Infinita;         //Tienes magia infinita
    public bool Escudando;              //Tienes escudo activo, tu escudo recibira el daño por ti

    [Header("Efecto Visual")]
    private float velocidadPulso = 1.5f;
    private Color colorBase;
    private Color colorBase2;
    private Color playerOriginalColor;
    private Coroutine currentFlashCoroutine;

    [Header("Combat Flow")]
    public CombatState state;
    public TMP_Text NumTurn;            //El texto que muestra el numero de turno actual
    public int turnCounter = 0;
    private int sacrificeAmountRemaining;

    [Header("FeedBack")]
    [SerializeField] private SpriteRenderer playerSprite;

    [SerializeField]
    public List<Enemy> enemies = new List<Enemy>();

    // Pila LIFO de escudos
    private Stack<ShieldInstance> shieldStack = new Stack<ShieldInstance>();
    public List<ActiveStatus> activeStatuses = new List<ActiveStatus>();

    // UI root y prefabs para notificaciones de turno
    [Header("Turn Notifications")]
    [SerializeField] private RectTransform uiRoot;                    // Canvas / panel donde instanciar
    [SerializeField] private GameObject turnNotificationPrefabPlayer; // prefab para Player Turn
    [SerializeField] private GameObject turnNotificationPrefabEnemy;  // prefab para Enemy Turn
    [SerializeField] private float notificationDuration = 1.2f;
    [SerializeField] private float notificationFade = 0.25f;

    //private string NombreEscena;
    public enum CombatState
    {
        PlayerTurn,
        EnemyTurn,
        Busy,
        Victory,
        Defeat
    }

    private void Awake()
    {
        Instance = this;
        xpBar.Barra_XP_Inicio_Combate(Exp_Actual, 25);
        Mano.SetActive(true);                       //Activar mano de cartas
        Fuerza = 0;
        Magia = 0;
        Robustez = 0;
    }
    public void Start()
    {
        Debug.Log(
        "Estoy en start. CombatManager: " +
        gameObject.scene.name
        );

        if (DeckManager.Instance != null)
        {
            Debug.Log("HasDeck: " + DeckManager.Instance.HasDeck());
        }

        Padre_Barras.SetActive(false);
        playerOriginalColor = playerSprite.color;
        if (!PlayerRunData.Instance.IsInitialized)
        {
            PlayerRunData.Instance.InitializeFromBaseStats(baseStats);
        }
        InicializarStats();
        InicializarBarrasSecundarias();
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();
        ReposicionHUD();
        imageShield();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        foreach (var enemy in enemies)
        {
            enemy.Initialize(this);
        }
        roboTemporal = 0;
        deck.Initialize(this);

        if (statusManager == null)
            statusManager = GetComponentInChildren<PlayerStatusUIManager>();

        Debug.Log("XP de combate: " + Exp_Actual);
        Debug.Log("XP antes de actualizar: " + PlayerRunData.Instance.experience);
        if(PlayerRunData.Instance.experience > 0)
            Exp_Actual = PlayerRunData.Instance.experience;
        xpBar.Barra_XP_Inicio_Combate(Exp_Actual, 25);
        Debug.Log("XP de combate: " + Exp_Actual);
        if(PlayerRunData.Instance.gold > 0)
        {
            Oro_Actual = PlayerRunData.Instance.gold;
            Oro_Display.text = Oro_Actual.ToString();
        }
        deck.Draw(5);
        StartCombat();
    }
    void Update()
    {
        // =========================
        // CLICK DERECHO → CANCELAR
        // =========================
        if (Input.GetMouseButtonDown(1))
        {
            CancelCardSelection();
            return;
        }
        //Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PressEscape();
        }

        // =========================
        // SI NO HAY CARTA SELECCIONADA → NO HACER NADA
        // =========================
        if (!IsWaitingForTarget)
            return;

        // =========================
        // EVITAR CLICK SI ESTAMOS SOBRE UI (CARTAS)
        // =========================
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        // =========================
        // CLICK IZQUIERDO → SELECCIONAR ENEMIGO
        // =========================
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos = new Vector2(mouseWorld.x, mouseWorld.y);

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

            foreach (var h in hits)
            {
                Debug.Log("CLICK en collider: " + h.name + " | Layer: " + LayerMask.LayerToName(h.gameObject.layer));
            }

            if (hit != null)
            {
                Debug.Log("CLICK detectado en: " + hit.name);

                Enemy enemy = hit.GetComponentInParent<Enemy>();

                Debug.Log("No se rompe en hit.GetComponentInParent<Enemy>();");

                if (enemy == null || enemy.IsDead)
                {
                    Debug.Log("Enemy es null o esta muerto");
                    return;
                }

                SelectEnemyTarget(enemy);
            }
            else
            {
                Debug.Log("Click en vacío");
            }
        }
    }
    public void PressEscape()
    {
        if (esta_pausado)
        {
            SFX_Pausar.Play();
            Musica.volume = NORMAL_VOLUME;
            esta_pausado = false;
            //Encender UI de salud, mana y escudo o moverla, ahi vere que hago

        }
        else
        {
            SFX_DesPausar.Play();
            Musica.volume = PAUSE_VOLUME;
            esta_pausado = true;
            //Apagar UI de salud, mana y escudo o moverla, ahi vere que hago

        }

        Pause.PresionaEscape();
    }
    private void aplicarVentaja()
    {
        //¿Hay que aplicar Bonus?
        if (PlayerRunData.Instance.BonusActual == PlayerRunData.Bonus.Player)   // El jugador tiene ventaja
        {
            energia_Actual += energiaInicialBonus;
            AplicarDanioInicial();
            if (roboInicialBonus > 0)
                deck.Draw(roboInicialBonus);
            if (escudoInicial > 0)
                AddShield(escudoInicial, 1); // ejemplo: escudo inicial de 1 turno
            if (CuracionBonus > 0)
                Heal(CuracionBonus);
            if (FuerzaBonus != 0)
                AddStat(1,FuerzaBonus);
                //Fuerza += FuerzaBonus;
            if (RobustezBonus != 0)
                AddStat(2,RobustezBonus);
                //Robustez += RobustezBonus;
            if (MagiaBonus != 0)
                AddStat(3,MagiaBonus);
                //Magia += MagiaBonus;
            if (RegenManaBonus > 0)
                manaCurrent = Mathf.Min(manaCurrent + RegenManaBonus, manaMaxima);
        }
        else if (PlayerRunData.Instance.BonusActual == PlayerRunData.Bonus.Enemy)
        {
            // El enemigo tiene ventaja

        }
    }
    private void AplicarDanioInicial()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            enemy.TakeDamage(danoInicialBonus);
        }
    }
    // =====================
    // FLUJO DE COMBATE
    // =====================
    private CardRuntime selectedCard;
    public void PlayCard(CardRuntime card, CombatManager target)
    {
        if (state != CombatState.PlayerTurn)
            return;

        if (!CanPayCost(card))
            return;

        var tipo = card.cardData.TipoCarta;
        var objetivo = card.cardData.ObjetivoCarta;

        //Debug
        if (tipo == TipoCarta.Attack && objetivo == ObjetivoCarta.Self)
        {
            Debug.LogWarning("Carta Attack no debería usar Self");
        }

        // =========================
        // 🟥 ATTACK → SIEMPRE ENEMY
        // =========================
        if (tipo == TipoCarta.Attack)
        {
            if (objetivo == ObjetivoCarta.Enemy)
            {
                if (selectedCard != null)
                    selectedCard.SetSelected(false);

                selectedCard = card;
                selectedCard.SetSelected(true);

                return;
            }

            if (objetivo == ObjetivoCarta.AllEnemies)
            {
                ExecuteCardOnAllEnemies(card);
                return;
            }
        }

        // =========================
        // 🟦 SKILL / POWER → SELF
        // =========================
        if (tipo == TipoCarta.Skill || tipo == TipoCarta.Power)
        {
            ExecuteCard(card, this);
            return;
        }

        // ===========================
        // 🟦 HYBRID → ENEMY OR SELF
        // ===========================

        if (tipo == TipoCarta.Hybrid)
        {
            if (selectedCard != null)
                selectedCard.SetSelected(false);

            selectedCard = card;
            selectedCard.SetSelected(true);

            return;
        }

        // fallback (por si algo raro pasa)
        ExecuteCard(card, this);
        CheckCombatEnd();
    }
    public void ExecuteCard(CardRuntime card, CombatManager target)
    {
        cancelCardExecution = false;
        PayCost(card);
        Debug.Log("Transaccion completa (Self)");
        if (card.cardData.audioClip != null)
        {
            audioSource.PlayOneShot(card.cardData.audioClip);
        }

        foreach (var effect in card.cardData.effects)
        {
            effect.Apply(card, target);
            if (cancelCardExecution)
            {
                Debug.Log("Ejecución de carta cancelada");
                break;
            }
        }

        if (card.purgeOnUse)
        {
            deck.Purge(card);
            card.purgeOnUse = false;
        }
        else if (card.exhaustOnUse)
        {
            deck.Exhaust(card);
            card.exhaustOnUse = false;
        }
        else
        {
            deck.Discard(card);
        }
    }
    public void ExecuteCardOnEnemy(CardRuntime card, Enemy enemy)
    {
        cancelCardExecution = false;
        PayCost(card);
        if (card.cardData.audioClip != null)
        {
            audioSource.PlayOneShot(card.cardData.audioClip);
        }

        if (card == null)
        {
            Debug.LogError("card es NULL");
            return;
        }

        if (card.cardData == null)
        {
            Debug.LogError("cardData es NULL");
            return;
        }

        if (card.cardData.effects == null)
        {
            Debug.LogError("effects es NULL");
            return;
        }

        Debug.Log("Cantidad de efectos: " + card.cardData.effects.Count);

        foreach (var effect in card.cardData.effects)
        {
            if (effect == null)
            {
                Debug.LogError("Hay un effect NULL en la lista");
                continue;
            }

            Debug.Log("Aplicando efecto: " + effect.name);

            if (effect.target == Effect.EffectTarget.Self)
            {
                effect.Apply(card, this);
            }
            else
            {
                effect.ApplyToEnemy(card, enemy);
            }

            if (cancelCardExecution)
            {
                Debug.Log("Ejecución de carta cancelada");
                break;
            }
        }

        if (card.purgeOnUse)
        {
            deck.Purge(card);
            card.purgeOnUse = false;
        }
        else if (card.exhaustOnUse)
        {
            deck.Exhaust(card);
            card.exhaustOnUse = false;
        }
        else
        {
            deck.Discard(card);
        }
    }
    void ExecuteCardOnAllEnemies(CardRuntime card)
    {
        Debug.Log("Ejecución en TODOS los enemigos");

        cancelCardExecution = false;
        PayCost(card);
        if (card.cardData.audioClip != null)
        {
            audioSource.PlayOneShot(card.cardData.audioClip);
        }

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            foreach (var effect in card.cardData.effects)
            {
                if (effect == null)
                    continue;

                // 🔥 CLAVE: usar ApplyToEnemy
                effect.ApplyToEnemy(card, enemy);

                if (cancelCardExecution)
                {
                    Debug.Log("Ejecución cancelada");
                    break;
                }
            }
        }

        // 🔥 manejo de la carta (igual que en single target)
        if (card.purgeOnUse)
        {
            deck.Purge(card);
            card.purgeOnUse = false;
        }
        else if (card.exhaustOnUse)
        {
            deck.Exhaust(card);
            card.exhaustOnUse = false;
        }
        else
        {
            deck.Discard(card);
        }
    }
    public void SelectEnemyTarget(Enemy enemy)
    {
        if (selectedCard == null)
            return;

        ExecuteCardOnEnemy(selectedCard, enemy);

        selectedCard.SetSelected(false);
        selectedCard = null;
    }
    bool CanPayCost(CardRuntime card)
    {
        int cost = card.cardData.cost;

        switch (card.cardData.costType)
        {
            case CostType.Energy:
                return energia_Actual >= cost;

            case CostType.Mana:
                return manaCurrent >= cost;

            case CostType.Health:
                return currentHealth > cost;

            case CostType.Sacrifice:
                return currentHealth > cost;
        }

        return false;
    }
    void PayCost(CardRuntime card)
    {
        int cost = card.cardData.cost;

        switch (card.cardData.costType)
        {
            case CostType.Energy:
                energia_Actual -= cost;                         //Pagar energia
                energia_Actual = Mathf.Max(0, energia_Actual);  //Evitar energia negativa
                ActualizarRayos();                              //Actualizar UI

                break;

            case CostType.Mana:
                UseMana(cost);
                break;

            case CostType.Health:
                TakeDamage(cost, null);
                break;

            case CostType.Sacrifice:
                TakeDamage(cost, null);
                break;
        }
    }
    public void IncreaseShieldDuration(int extraTurns)
    {
        if (shieldStack == null || shieldStack.Count == 0)
            return;

        Stack<ShieldInstance> newStack = new Stack<ShieldInstance>();

        foreach (var shield in shieldStack)
        {
            shield.remainingTurns += extraTurns;
            newStack.Push(shield);
        }

        shieldStack = newStack;

        Debug.Log($"{characterName} extiende la duración de sus escudos en +{extraTurns} turno(s).");
    }
    public void ReduceShieldDurations()
    {
        Stack<ShieldInstance> newStack = new Stack<ShieldInstance>();

        foreach (var shield in shieldStack)
        {
            shield.remainingTurns--;

            if (shield.remainingTurns > 0)
                newStack.Push(shield);
        }

        shieldStack = newStack;
    }
    // =====================
    // COMBAT FLOW
    // =====================

    public void StartCombat()
    {
        turnCounter = 0;
        StartCoroutine(StartPlayerTurn());
    }

    private IEnumerator StartPlayerTurn()
    {
        Debug.Log("Fuerza: " + Fuerza);
        var run = PlayerRunData.Instance;
        state = CombatState.PlayerTurn;
        turnCounter++;

        NumTurn.text = turnCounter.ToString();

        // Recuperar energia antes de mostrar la notificación visual
        Recuperar_Energia();

        // Mostrar notificación "Player Turn" y esperar a que desaparezca
        yield return StartCoroutine(ShowTurnNotificationCoroutine("Player Turn", true));

        // Reactivar UI jugador sólo después de la notificación
        if (endTurnButton != null)
            endTurnButton.interactable = true;

        UpdateReviveButton();

        // Limpiar hover residual y actualizar layout (evita solapes)
        if (deck != null)
            deck.ClearHoverState();

        // Ahora procesar inicio de turno
        ProcessStartTurnLogic();
        ActualizarBarrasEscudo();
        if (turnCounter == 1)
            aplicarVentaja();

        if (turnCounter > 1)
            deck.Draw(roboBase);

        foreach (var enemy in enemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                enemy.Agregar_Iconos();
            }
        }
        CheckCombatEnd();
    }
    public void EndTurnButton() // Llamado desde el botón UI
    {
        EndTurn();
    }
    public void EndTurn()
    {
        if (state != CombatState.PlayerTurn) return;

        state = CombatState.Busy;

        StartCoroutine(EnemyTurnCoroutine());
    }
    private IEnumerator EnemyTurnCoroutine()
    {
        state = CombatState.EnemyTurn;

        // Desactivar UI de jugador y limpiar hover inmediatamente
        if (endTurnButton != null)
            endTurnButton.interactable = false;
        if (reviveButton != null)
            reviveButton.interactable = false;
        if (deck != null)
            deck.ClearHoverState();

        // Mostrar notificación "Enemy Turn" y esperar a que desaparezca
        yield return StartCoroutine(ShowTurnNotificationCoroutine("Enemy Turn", false));

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            yield return enemy.PerformAction();
            enemy.ProcessStatuses();
            // 🔥 Reducir duración de buffs del enemigo
            enemy.OnTurnPassed();

            yield return new WaitForSeconds(0.2f);
        }

        CheckCombatEnd();

        if (state != CombatState.Victory && state != CombatState.Defeat)
        {
            // Inicia la rutina de StartPlayerTurn y espera (evita reentradas)
            yield return StartCoroutine(StartPlayerTurn());
        }
    }
    private IEnumerator ShowTurnNotificationCoroutine(string message, bool isPlayer)
    {
        GameObject prefab = isPlayer ? turnNotificationPrefabPlayer : turnNotificationPrefabEnemy;

        if (prefab == null || uiRoot == null)
        {
            Debug.Log($"Turn notification: {message} (prefab o uiRoot no asignado)");
            yield return new WaitForSeconds(notificationDuration);
            yield break;
        }

        // Desactivar LayoutGroups / ContentSizeFitter del uiRoot temporalmente
        var layoutGroups = uiRoot.GetComponents<UnityEngine.UI.LayoutGroup>();
        var fitters = uiRoot.GetComponents<UnityEngine.UI.ContentSizeFitter>();
        var layoutStates = new bool[layoutGroups.Length];
        var fitterStates = new UnityEngine.UI.ContentSizeFitter.FitMode[fitters.Length];

        for (int i = 0; i < layoutGroups.Length; i++)
        {
            layoutStates[i] = layoutGroups[i].enabled;
            layoutGroups[i].enabled = false;
        }
        for (int i = 0; i < fitters.Length; i++)
        {
            fitterStates[i] = fitters[i].horizontalFit;
            fitters[i].horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            fitters[i].verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
        }

        // Preservar rect-config del prefab
        RectTransform prefabRT = prefab.GetComponent<RectTransform>();
        Vector3 prefabLocalScale = prefabRT != null ? prefabRT.localScale : Vector3.one;
        Vector2 prefabSize = prefabRT != null ? prefabRT.sizeDelta : Vector2.zero;
        Vector2 prefabAnchorMin = prefabRT != null ? prefabRT.anchorMin : new Vector2(0.5f, 0.5f);
        Vector2 prefabAnchorMax = prefabRT != null ? prefabRT.anchorMax : new Vector2(0.5f, 0.5f);
        Vector2 prefabPivot = prefabRT != null ? prefabRT.pivot : new Vector2(0.5f, 0.5f);

        // Instanciar y forzar configuración del RectTransform de la instancia
        GameObject go = Instantiate(prefab, uiRoot, false);

        // Forzar actualizar el Canvas/layout antes de tocar posiciones
        Canvas.ForceUpdateCanvases();

        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = prefabLocalScale;
            if (prefabSize != Vector2.zero) rt.sizeDelta = prefabSize;
            rt.anchorMin = prefabAnchorMin;
            rt.anchorMax = prefabAnchorMax;
            rt.pivot = prefabPivot;
            rt.anchoredPosition = Vector2.zero;
            rt.localPosition = Vector3.zero;
        }

        var cg = go.GetComponent<CanvasGroup>() ?? go.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        var tn = go.GetComponent<TurnNotification>();
        if (tn == null)
        {
            Debug.LogWarning("turnNotificationPrefab no contiene TurnNotification component");
            yield return new WaitForSeconds(notificationDuration);
            Destroy(go);
        }
        else
        {
            yield return StartCoroutine(tn.Play(message, notificationDuration, notificationFade));
            Destroy(go);
        }

        // Restaurar LayoutGroups / Fitters
        for (int i = 0; i < layoutGroups.Length; i++)
            layoutGroups[i].enabled = layoutStates[i];
        for (int i = 0; i < fitters.Length; i++)
        {
            // no tenemos el estado vertical/horizontal original perfecto, pero restauramos a Unconstrained->original simple
            fitters[i].horizontalFit = fitterStates[i];
            fitters[i].verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
        }

        Canvas.ForceUpdateCanvases();
    }
    public void CheckCombatEnd()
    {
        if (state == CombatState.Defeat || state == CombatState.Victory)
            return;

        if (currentHealth <= 0 || Muerto == true)
        {
            state = CombatState.Defeat;
            StartCoroutine(Derrota());
            return;
        }

        bool allEnemiesDead = enemies.All(e => e == null || e.IsDead);

        if (allEnemiesDead)
        {
            state = CombatState.Victory;
            StartCoroutine(Victory());
        }
    }
    public void ProcessStartTurnLogic()
    {
        // 🔥 PROCESAR ESTADOS
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            var status = activeStatuses[i];

            // =========================
            // 🟢 APLICAR EFECTOS
            // =========================

            if (status.effect.effectName == "Regeneracion")
            {
                Heal(status.value);
                Debug.Log($"{characterName} se cura {status.value} por regeneración");
            }

            if (status.effect.effectName == "Veneno")
            {
                Debug.Log($"ANTES RemoveAt -> i={i}  Count={activeStatuses.Count}");
                TakeDamage(status.value, null);
                Debug.Log($"DESPUÉS TakeDamage_veneno -> Count={activeStatuses.Count}");
                Debug.Log($"{characterName} recibe {status.value} de daño por veneno");
            }

            if (status.effect.effectName == "Hemorragia")
            {
                // 🩸 daño por turno
                Debug.Log($"ANTES RemoveAt -> i={i}  Count={activeStatuses.Count}");
                TakeDamage(status.value, null);
                Debug.Log($"DESPUÉS TakeDamage_hemorragia -> Count={activeStatuses.Count}");
                Debug.Log($"{characterName} sangra {status.value}");
            }
            if (status.effect.effectName == "DrenajeEnergia")
            {
                energia_Actual -= status.value;
                energia_Actual = Mathf.Max(0, energia_Actual);

                ActualizarRayos();

                Debug.Log($"{characterName} pierde {status.value} de energía por drenaje");
            }

            status.duration--;

            // =========================
            // 💀 EXPIRACIÓN
            // =========================
            if (status.duration <= 0)
            {
                // 🔥 EFECTO ESPECIAL: HEMORRAGIA (burst)
                if (status.effect.effectName == "Hemorragia")
                {
                    Debug.Log($"ANTES RemoveAt -> i={i}  Count={activeStatuses.Count}");
                    TakeDamage(status.burstValue, null);
                    Debug.Log($"DESPUÉS TakeDamage_hemorragia_Burst -> Count={activeStatuses.Count}");
                    Debug.Log($"{characterName} sufre hemorragia final de {status.burstValue}");
                }

                // 🔥 REVERTIR BUFF TEMPORAL
                if (status.effect.revertOnExpire)
                {
                    AddStat(status.effect.statType, -status.value);
                    Debug.Log($"{characterName} pierde {status.value} de buff temporal");
                }

                activeStatuses.RemoveAt(i);
            }
        }

        // =========================
        // 🛡️ PROCESAR ESCUDOS
        // =========================
        if (shieldStack.Count > 0)
        {
            var tempList = shieldStack.ToList();
            tempList.Reverse();

            for (int i = 0; i < tempList.Count; i++)
            {
                tempList[i].remainingTurns--;

                if (tempList[i].remainingTurns <= 0)
                {
                    tempList.RemoveAt(i);
                    i--;
                }
            }

            shieldStack.Clear();

            foreach (var s in tempList.AsEnumerable().Reverse())
                shieldStack.Push(s);

            ActualizarEscudo();
        }

        OnShieldBrokenThisTurn = null;
        // 🔥 ACTUALIZAR UI DE ESTADOS
        if (statusManager != null)
            statusManager.UpdateStatuses();
    }
    //Buff y Debuff Stats
    public void AddStat(int type, int amount)
    {
        switch (type)
        {
            case 1: Fuerza += amount; break;
            case 2: Robustez += amount; break;
            case 3: Magia += amount; break;
            default:
                Debug.LogWarning("Tipo de stat desconocido");
                break;
        }
        // Actualizar iconos de estados
        if (statusManager != null)
            statusManager.UpdateStatuses();

        Debug.Log($"[{characterName}] Fuerza ahora = {Fuerza}");
    }
    public int GetFuerza()
    {
        return Fuerza;
    }
    public int GetRobustez()
    {
        return Robustez;
    }
    public int GetMagia()
    {
        return Magia;
    }
    // =====================
    // ESCUDOS (LIFO)
    // =====================

    public void AddShield(int value, int duration)
    {
        if(TotalShield == 0)
        {
            Padre_Barras.SetActive(true);
        }
        StartCoroutine(FlashPlayer(Color.gray));
        int finalValue = Mathf.Max(0, value + Robustez);
        shieldStack.Push(new ShieldInstance(finalValue, duration));
        ActualizarEscudo();
        audioSource.PlayOneShot(shieldGainSFX);
        Debug.Log($"{characterName} obtiene +{value} de escudo por {duration} turnos. Total actual: {TotalShield}");
    }

    public void TakeDamage(int amount, CombatManager attacker)
    {
        int damageLeft = amount;
        StartCoroutine(FlashPlayer(Color.red));

        while (shieldStack.Count > 0 && damageLeft > 0)
        {
            var top = shieldStack.Pop();

            if (damageLeft >= top.value)
            {
                damageLeft -= top.value;

                if (shieldStack.Count == 0)
                {
                    //Reproduce sonido de rotura de escudo
                    audioSource.PlayOneShot(shieldBreakSFX);
                }
                else
                {
                    //Cada vez que el escudo recibe daño y no se rompa, se reproduce un sonido de golpe
                    audioSource.PlayOneShot(shieldHitSFX);
                }
            }
            else
            {
                top.value -= damageLeft;
                damageLeft = 0;
                shieldStack.Push(top);
            }
        }

        ActualizarEscudo();

        if (damageLeft > 0)
        {
            currentHealth -= damageLeft;

            Debug.Log($"{characterName} recibe {damageLeft} de daño directo. Vida restante: {currentHealth}");

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
            Die();

        CheckCombatEnd();
    }
    public void Heal(int amount)
    {
        StartCoroutine(FlashPlayer(Color.green));
        if (amount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log($"{characterName} se cura {amount}. Salud actual: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void UseMana(int amount)
    {
        manaCurrent -= amount;
        manaCurrent = Mathf.Max(0, manaCurrent);

        OnManaChanged?.Invoke(manaCurrent, manaMaxima);
    }
    public void Recuperar_Energia()
    {
        if (!usaEnergia) return;

        Debug.Log("energiaBase = " + energiaBase);

        energia_Actual = energiaBase;

        Debug.Log("energiaActual = " + energia_Actual);
        ActualizarRayos();
    }
    public void ActualizarRayos()
    {
        for (int i = 0; i < Rayos.Length; i++)
        {
            Rayos[i].enabled = i < energia_Actual;
        }
    }
    public int TotalShield
    {
        get
        {
            int total = 0;
            foreach (var s in shieldStack)
                total += s.value;
            return total;
        }
    }

    // === Métodos públicos seguros para acceder a la pila ===
    public int ShieldCount => shieldStack.Count;

    public ShieldInstance PeekShield()
    {
        return shieldStack.Count > 0 ? shieldStack.Peek() : null;
    }

    public void PopShield()
    {
        if (shieldStack.Count > 0)
            shieldStack.Pop();
    }

    // =====================
    // ESTADOS
    // =====================
    public void AddStatus(StatusEffect effect, int value, int duration)
    {
        if (effect.effectName == "Escudo")
        {
            AddShield(value, duration);
            return;
        }

        int finalDuration = duration;

        // 🔻 SI ES DEBUFF → aplicar resistencia
        if (effect.category == StatusCategory.Debuff)
        {
            finalDuration -= Resistir_Estado;
        }
        // 🔺 SI ES BUFF → potenciar duración
        else
        {
            finalDuration += Potenciar_Duracion_Estado;
        }

        finalDuration = Mathf.Max(1, finalDuration);

        activeStatuses.Add(new ActiveStatus(effect, value, finalDuration));
        
        // 🔥 ACTUALIZAR UI
        if (statusManager != null)
            statusManager.UpdateStatuses();
    }

    private void Die()
    {
        //PlayerRunData.Instance.TryRevive();
        Muerto = true;
    }

    [System.Serializable]
    public class ShieldInstance
    {
        public int value;
        public int remainingTurns;

        public ShieldInstance(int value, int duration)
        {
            this.value = value;
            this.remainingTurns = duration;
        }
    }

    // =====================
    // CONSULTAS DE ESTADOS
    // =====================
    public bool HasStatus(string statusName)
    {
        foreach (var status in activeStatuses)
        {
            if (status.effect != null && status.effect.effectName == statusName)
                return true;
        }
        return false;
    }

    public int GetStatusValue(string statusName)
    {
        int total = 0;
        foreach (var status in activeStatuses)
        {
            if (status.effect != null && status.effect.effectName == statusName)
                total += status.value;
        }
        return total;
    }

    public void RemoveStatus(string statusName)
    {
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            if (activeStatuses[i].effect != null && activeStatuses[i].effect.effectName == statusName)
            {
                activeStatuses.RemoveAt(i);
            }
        }
        
        // 🔥 ACTUALIZAR UI
        if (statusManager != null)
            statusManager.UpdateStatuses();
    }

    public int GetStatusStacks(string statusName)
    {
        int count = 0;
        foreach (var status in activeStatuses)
        {
            if (status.effect != null && status.effect.effectName == statusName)
                count++;
        }
        return count;
    }

    public int GetSpecificBuffValue(string buffName)
    {
        StartCoroutine(FlashPlayer(Color.blue));
        foreach (var status in activeStatuses)
        {
            if (status.effect != null && status.effect.effectName == buffName)
                return status.value;
        }
        return 0;
    }
    void Titilar()
    {
        marcoBarrera.enabled = true;

        // Oscila entre 0 y 1
        float t = (Mathf.Sin(Time.time * velocidadPulso) + 1f) / 2f;

        // Verde entre 0.8 y 0.2 (baja y sube)
        float verde = Mathf.Lerp(0.8f, 0.2f, t);

        Color nuevoColor = colorBase;

        nuevoColor.g = verde; // ahora modificamos el verde

        marcoBarrera.color = nuevoColor;
    }
    void TitilarMagico()
    {
        RecubrimientoMagico.enabled = true;
        // Oscila entre 0 y 1
        float t = (Mathf.Sin(Time.time * velocidadPulso) + 1f) / 2f;

        // Convertimos ese 0-1 a 0.2 - 0.7
        float azul = Mathf.Lerp(0.5f, 1.0f, t);

        Color nuevoColor = colorBase2;
        nuevoColor.b = azul;

        RecubrimientoMagico.color = nuevoColor;
    }
    void InicializarStats()
    {
        var run = PlayerRunData.Instance;

        maxHealth = run.Max_HP;
        currentHealth = run.current_HP;
        LimitHP = run.Limit_HP;

        usaEnergia = run.UsaEnergia;
        energiaBase = run.Energia;

        usaMana = run.UsaMana;
        manaCurrent = run.current_MP;
        manaMaxima = run.Max_MP;
        manaLimit = run.Limit_MP;
        manaRegenPorTurno = run.manaRegenPorTurno;

        roboBase = run.roboBase;
        manoMaxima = run.manoMaxima;
        energiaConservadaPorTurno = run.energiaConservadaPorTurno;
        cost_reshuffle = run.Cost_Reshuffle;

        energiaInicialBonus = run.energiaInicialBonus;
        danoInicialBonus = run.danoInicialBonus;
        roboInicialBonus = run.roboInicialBonus;
        escudoInicial = run.escudoInicial;
        CuracionBonus = run.CuracionBonus;
        FuerzaBonus = run.FuerzaBonus;
        RobustezBonus = run.RobustezBonus;
        MagiaBonus = run.MagiaBonus;

        Potenciar_Duracion_Estado = run.Potenciar_Duracion_Estado;
        Resistir_Estado = run.Resistir_Estado;
    }
    void InicializarBarrasSecundarias()
    {
        colorBase = marcoBarrera.color;
        colorBase2 = RecubrimientoMagico.color;
        barrera = false;
        Magia_Infinita = false;
    }
    void ajustar_salud()
    {

    }
    void EndCombat()
    {
        var run = PlayerRunData.Instance;

        // Actualizar Valores a PRD
        run.SaveCombatResults(this);        
        activeStatuses.Clear();
        shieldStack.Clear();
        ActualizarEscudo();
        
        if (statusManager != null)
            statusManager.DestroyAllIcons();
    }
    public void ReposicionHUD()
    {
        if (!usaMana)
        {
            ManaBar.SetActive(false);

            if (usaEnergia)
            {
                Vector2 pos = EnergyBar.anchoredPosition;
                pos.y += 15;
                EnergyBar.anchoredPosition = pos;
            }
        }
        if (!usaEnergia)
        {
            EnergyBar.gameObject.SetActive(false);
            //foreach (var rayo in Rayos)
            //    rayo.gameObject.SetActive(false);
        }
    }
    public void ceguera()
    {
        HPBar.gameObject.SetActive(false);
        ManaBar.gameObject.SetActive(false);
        EnergyBar.gameObject.SetActive(false);
        imageShield();
    }
    public void revelar()
    {
        HPBar.gameObject.SetActive(true);
        if (usaMana)
            ManaBar.gameObject.SetActive(true);
        if (usaEnergia)
            EnergyBar.gameObject.SetActive(true);
        //if(shieldStack > 0)
        imageShield();
    }
    public void BotonRevivirCartas()
    {
        if (energia_Actual < cost_reshuffle)
        {
            //Mensaje en pantalla de que no tienes energia suficiente
            return;
        }

        energia_Actual -= cost_reshuffle;
        ActualizarRayos();
        deck.RevivirCartas();
    }
    public void UpdateReviveButton()
    {
        reviveButton.interactable = deck.DiscardCount > 0;
    }
    public void CancelCardSelection()
    {
        if (selectedCard == null)
            return;

        selectedCard.isHovered = false;
        selectedCard.SetSelected(false);
        selectedCard = null;
    }
    public void ActualizarEscudo()
    {
        int total = TotalShield;

        bool tieneEscudo = total > 0;

        Padre_Barras.SetActive(tieneEscudo);            //Desactiva las barras cuando no hay escudo
        ActualizarBarrasEscudo();
        // Activar / desactivar TODO
        imageShield();

        if (shieldValue != null)
        {
            shieldValue.gameObject.SetActive(tieneEscudo);

            if (tieneEscudo)
                shieldValue.text = total.ToString();
            else
                shieldValue.text = "";
        }
    }
    void ActualizarBarrasEscudo()
    {
        // 🔻 Resetear todo
        for (int i = 0; i < Barras_Escudo.Length; i++)
        {
            Barras_Escudo[i].color = colorVacio;
            Barras_Escudo[i].transform.localScale = Vector3.one; // 🔥 reset escala
        }

        if (shieldStack.Count == 0)
            return;

        // 🔻 Convertir stack a lista en orden visual (izq → der)
        var lista = shieldStack.ToList();
        lista.Reverse();

        // 🔻 Pintar barras
        for (int i = 0; i < lista.Count && i < Barras_Escudo.Length; i++)
        {
            int duracion = lista[i].remainingTurns;

            if (duracion <= 1)
                Barras_Escudo[i].color = color1Turno;
            else if (duracion == 2)
                Barras_Escudo[i].color = color2Turnos;
            else
                Barras_Escudo[i].color = color3Turnos;
        }

        // 🔥 DESTACAR EL ÚLTIMO (TOP DEL STACK)
        int ultimoIndex = Mathf.Min(lista.Count, Barras_Escudo.Length) - 1;

        if (ultimoIndex >= 0)
        {
            Barras_Escudo[ultimoIndex].transform.localScale = Vector3.one * 1.2f;
        }
    }
    public void ClearAllShield()
    {
        if (shieldStack.Count == 0)
            return;

        shieldStack.Clear();
        ActualizarEscudo();
        audioSource.PlayOneShot(shieldBreakSFX); // 🔥 importante
    }
    public void BreakOneShield()
    {
        if (shieldStack.Count == 0)
            return;

        shieldStack.Pop();

        if (shieldStack.Count == 0)
        {
            // Reproducir sonido cuando se rompe el último escudo
            audioSource.PlayOneShot(shieldBreakSFX);
        }
        else
        {
            // Se rompe un escudo pero aún quedan, reproducir sonido de golpe
            audioSource.PlayOneShot(shieldHitSFX);
        }

        ActualizarEscudo();
    }
    public void StartCardSelectionForSacrifice(CardRuntime source, int amount)
    {
        isSelectingCardToSacrifice = true;
        sacrificeSourceCard = source;
        sacrificeAmountRemaining = amount;

        Debug.Log($"Selecciona {amount} carta(s) para sacrificar");
    }
    public void SelectCardToSacrifice(CardRuntime card)
    {
        if (!isSelectingCardToSacrifice)
            return;

        if (card == null)
            return;

        if (card == sacrificeSourceCard)
            return;

        if (!deck.IsInHand(card))
            return;

        deck.Discard(card);
        sacrificeAmountRemaining--;
        Debug.Log($"{characterName} sacrifica {card.cardData.cardName}");

        if (sacrificeAmountRemaining <= 0)
        {
            isSelectingCardToSacrifice = false;
            Debug.Log("Sacrificio completado");
        }
    }
    public bool IsSelectingCardToSacrifice()
    {
        return isSelectingCardToSacrifice;
    }
    IEnumerator FlashPlayer(Color flashColor)
    {
        if (playerSprite == null) yield break;

        // 🔥 Si ya hay un flash activo → lo cancelamos
        if (currentFlashCoroutine != null)
        {
            StopCoroutine(currentFlashCoroutine);
            playerSprite.color = playerOriginalColor; // reset limpio
        }

        currentFlashCoroutine = StartCoroutine(FlashRoutine(flashColor));
    }

    IEnumerator FlashRoutine(Color flashColor)
    {
        playerSprite.color = flashColor;

        yield return new WaitForSeconds(0.1f);

        playerSprite.color = playerOriginalColor;

        currentFlashCoroutine = null;
    }
    public void Sumar_Oro_y_XP(int Oro_Enemy, int XP_Enemy)
    {
        Oro_Enemigos_Acum += Oro_Enemy;
        XP_Enemigos_Acum += XP_Enemy;
    }
    public void TomarCarta()        //Clic
    {
        //Posicionar brillo
        Brillito.gameObject.transform.position = BotonTomarCarta.transform.position;
        Destroy(BotonTomarCarta); // Destruir el panel de victoria
        StartCoroutine(Aplicar_Brillo());
        //Aqui pondre el panel que devela 3 cartas para que elijas

        //Sumar carta al mazo del jugador
        CR.TakeCard();
    }
    public void TomarDinero()       //Clic
    {
        //Posicionar y activar el brillito
        Brillito.transform.position = BotonTomarOro.transform.position; // Mover el brillito al botón
        Destroy(BotonTomarOro); // Destruir el panel de victoria
        Oro_Actual += Oro_Enemigos_Acum;
        //Sonido de Oro recogido

        StartCoroutine(Aplicar_Brillo());
        Oro_Display.text = Oro_Actual.ToString();
    }
    public void TomarObjeto1()      //Clic
    {
        if (PotionManager.Instance.GetPotions().Count >= 10)
        {
            Panel_Caracteristicas_Objetos.Instance.MensajeDirecto("Inventario lleno");
            return;
        }
        //Posicionar y activar el brillito
        Brillito.transform.position = BotonTomarObjeto1.transform.position; // Mover el brillito al botón
        Destroy(BotonTomarObjeto1); // Destruir el panel de victoria
        //Sonido de Objeto recogido

        StartCoroutine(Aplicar_Brillo());
        CR.TakeObject1();
    }
    public void TomarObjeto2()      //Clic
    {
        if (PotionManager.Instance.GetPotions().Count >= 10)
        {
            Panel_Caracteristicas_Objetos.Instance.MensajeDirecto("Inventario lleno");
            return;
        }
        //Posicionar y activar el brillito
        Brillito.transform.position = BotonTomarObjeto2.transform.position; // Mover el brillito al botón
        Destroy(BotonTomarObjeto2); // Destruir el panel de victoria
        //Sonido de Objeto recogido

        StartCoroutine(Aplicar_Brillo());
        CR.TakeObject2();
    }
    public void TomarReliquia()     //Clic
    {
        //Posicionar y activar el brillito
        Brillito.transform.position = BotonTomarReliquia.transform.position; // Mover el brillito al botón
        Destroy(BotonTomarReliquia); // Destruir el panel de victoria
        //Sonido de Reliquia recogido

        StartCoroutine(Aplicar_Brillo());
        CR.TakeRelic();
    }
    public void ClickSalir()
    {
        StartCoroutine(Salir());
    }
    private IEnumerator Salir()
    {
        EndCombat();

        yield return new WaitForSeconds(0.5f);
        Debug.Log("Escena activa: " + SceneManager.GetActiveScene().name);
        Scene escenaCombate = gameObject.scene;
        HUD_Combate.SetActive(false);
        Player_Control.Instance.Reanudar_EX();
        //Debug.Log("Voy a descargar: " + escenaCombate.name);
        yield return SceneManager.UnloadSceneAsync(escenaCombate);
        Debug.Log("Escena descargada");
    }
    public IEnumerator Victory()
    {
        ActualizarRecompensas();
        Mano.SetActive(false);                       //Desactivar mano de cartas
        //Activar animacion de victoria

        //Sonido de victoria
        Victoria_SFX.Play();
        CartelVictoria.SetActive(true);             // mostrar cartel victoria
        Confetti1.SetActive(true);
        Confetti2.SetActive(true);
        xpBar.Actualizar_XP(Exp_Actual, 25, XP_Enemigos_Acum);
        yield return new WaitUntil(() => !xpBar.AnimandoXP);
        PlayerRunData.Instance.experience = Exp_Actual;
        CartelVictoria.SetActive(false);            // ocultar cartel victoria
        Confetti1.SetActive(false);
        Confetti2.SetActive(false);
        PanelRecompensas.SetActive(true);              // activar panel de victoria

        //Lo ideal sería que Actualizar_XP() fuera una corrutina:
        //yield return StartCoroutine(xpBar.Actualizar_XP(...));
    }
    //panelVictoria.SetActive(false); // ocultar
    public IEnumerator Derrota()
    {
        //Activar animacion de victoria

        //Sonido de derrota
        Derrota_SFX.Play();
        CartelDerrota.SetActive(true);              // mostrar cartel victoria
        EndCombat();
        yield return new WaitForSeconds(1f);        // Esperar 1 segundo
        CartelDerrota.SetActive(false);             // ocultar cartel victoria
        yield return new WaitForSeconds(1f);        // Esperar 1 segundo
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void volverMenu()    //Clic panel derrota
    {

        SceneManager.LoadScene("2-Hub Principal");
    }
    public void SubirNivel()
    {
        StartCoroutine(Level_UP());
    }
    public IEnumerator Level_UP()
    {
        LevelUP_SFX.Play();
        fuegoArtificial1.SetActive(true);
        fuegoArtificial2.SetActive(true);        
        CartelLevelUP.SetActive(true);              // mostrar cartel LevelUP
        yield return new WaitForSeconds(1.5f);        // Esperar 1.5 segundos
        CartelLevelUP.SetActive(false);             // ocultar cartel LevelUP
        maxHealth += 5;
        currentHealth += 5;
        PlayerRunData.Instance.AddMaxHP(5);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void imageShield()
    {
        if (TotalShield > 0)
        {
            Shield.gameObject.SetActive(true);
            if (shieldValue != null)
            {
                shieldValue.gameObject.SetActive(true);
                shieldValue.text = TotalShield.ToString();
            }
        }
        else
        {
            Shield.gameObject.SetActive(false);
            if (shieldValue != null)
            {
                shieldValue.gameObject.SetActive(false);
                shieldValue.text = "";
            }
        }
    }
    private void ActualizarRecompensas()
    {
        //Recompensa_Carta.text = "Tomar: " CR.Card.cardName;
        Recompensa_Oro.text = "Tomar " + Oro_Enemigos_Acum.ToString() + " de Oro";
    }
    IEnumerator Aplicar_Brillo()
    {
        Brillito.gameObject.SetActive(true);   // Activar el brillo
        Brillito.Play();
        yield return new WaitForSeconds(2f);    //esperar a que termine la animacion
        Brillito.gameObject.SetActive(false);  // Desactivar el brillo
    }
    public void AddMaxHealthTemporary(int amount)   //Buff de vida temporal (no se guarda en PRD)
    {
        maxHealth += amount;
        currentHealth += amount;
    }
    public void AddMaxHealthPermanent(int amount)   //Buff de vida permanente (se guarda en PRD)
    {
        maxHealth += amount;
        currentHealth += amount;

        PlayerRunData.Instance.AddMaxHP(amount);
    }
}