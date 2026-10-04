using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarraSalud : MonoBehaviour
{
    public static BarraSalud Instance;
    private CombatManager combatCharacter;

    [Header("HP UI")]
    public Image currentHealthBar;
    public Image currentHealthGlobe;
    public TextMeshProUGUI healthText;

    [Header("Mana UI (lo conectaremos después)")]
    public Image currentManaBar;
    public Image currentManaGlobe;
    public TextMeshProUGUI manaText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Suscribirse al evento de PlayerRunData
        if (PlayerRunData.Instance != null)
        {
            PlayerRunData.Instance.OnHealthChanged += UpdateHealthUI;

            // Inicializar visualmente con valores actuales
            UpdateHealthUI(
                PlayerRunData.Instance.current_HP,
                PlayerRunData.Instance.Max_HP
            );
        }
        combatCharacter = FindFirstObjectByType<CombatManager>();

        if (combatCharacter != null)
        {
            combatCharacter.OnHealthChanged += UpdateHealthUI;
            combatCharacter.OnManaChanged += UpdateManaUI;

            UpdateHealthUI(combatCharacter.currentHealth, combatCharacter.maxHealth);
        }
        else if (PlayerRunData.Instance != null)
        {
            PlayerRunData.Instance.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(
                PlayerRunData.Instance.current_HP,
                PlayerRunData.Instance.Max_HP
            );
        }
    }

    private void OnDestroy()
    {
        if (PlayerRunData.Instance != null)
        {
            PlayerRunData.Instance.OnHealthChanged -= UpdateHealthUI;
        }
    }

    // ==========================================================
    // ACTUALIZA TODA LA UI DE VIDA
    // ==========================================================
    private void UpdateHealthUI(int current, int max)
    {
        if (max <= 0) return;

        float ratio = (float)current / max;

        // Barra horizontal (Filled)
        if (currentHealthBar != null)
        {
            currentHealthBar.fillAmount = ratio;
        }

        // Globo vertical (si también lo quieres como Filled)
        if (currentHealthGlobe != null)
        {
            currentHealthGlobe.fillAmount = ratio;
        }

        // Texto
        if (healthText != null)
        {
            healthText.text = current + "/" + max;
        }
    }

    private void UpdateManaUI(int current, int max)
    {
        if (max <= 0) return;

        float ratio = (float)current / max;

        if (currentManaBar != null)
        {
            currentManaBar.fillAmount = ratio;
        }

        if (currentManaGlobe != null)
        {
            currentManaGlobe.fillAmount = ratio;
        }

        if (manaText != null)
        {
            manaText.text = current + "/" + max;
        }
    }
}
