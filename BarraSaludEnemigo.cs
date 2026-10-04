using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarraSaludEnemigo : MonoBehaviour
{
    private Enemy enemy;

    public TMP_Text healthText;
    public Image currentHealthBar;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("Enemy no encontrado");
            return;
        }

        if (healthText == null)
        {
            Debug.LogError("healthText no asignado");
        }

        if (currentHealthBar == null)
        {
            Debug.LogError("currentHealthBar no asignado");
        }

        enemy.OnHealthChanged += UpdateHealthUI;

        UpdateHealthUI(enemy.GetCurrentHP(), enemy.GetMaxHP());
    }

    void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnHealthChanged -= UpdateHealthUI;
        }
    }

    void UpdateHealthUI(int current, int max)
    {
        if (max <= 0) return;

        float ratio = (float)current / max;

        currentHealthBar.fillAmount = ratio;

        healthText.text = current + "/" + max;
    }
}