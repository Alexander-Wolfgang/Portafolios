using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image statusIcon;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI durationText;
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetStatus(ActiveStatus status)
    {
        if (status?.effect == null)
            return;

        // 🔥 Mostrar icono del efecto
        if (statusIcon != null && status.effect.icon != null)
        {
            statusIcon.sprite = status.effect.icon;
            statusIcon.enabled = true;
        }

        // 🔥 Color de fondo según categoría (Buff/Debuff)
        if (background != null)
        {
            if (status.effect.category == StatusEffect.StatusCategory.Buff)
                background.color = new Color(0.2f, 0.6f, 0.2f, 0.8f); // Verde para buffs
            else
                background.color = new Color(0.6f, 0.2f, 0.2f, 0.8f); // Rojo para debuffs
        }
        //Mostrar daño por turno o valor del estado
        if (valueText != null)
        {
            if (status.value != 0)
            {
                valueText.text = status.value.ToString();
                valueText.enabled = true;
            }
            else
            {
                valueText.enabled = false;
            }
        }
        // 🔥 Mostrar duración
        if (durationText != null)
        {
            if (status.duration > 0)
            {
                durationText.text = "-" + status.duration.ToString();
                durationText.enabled = true;
            }
            else
            {
                durationText.enabled = false;
            }
        }

        // 🔥 Mostrar/ocultar suavemente
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void Clear()
    {
        if (statusIcon != null)
        {
            statusIcon.sprite = null;
            statusIcon.enabled = false;
        }

        if (durationText != null)
        {
            durationText.text = "";
            durationText.enabled = false;
        }

        if (valueText != null)
        {
            valueText.text = "";
            valueText.enabled = false;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }
    public void SetStat(Sprite icon, string nombre, int value)
    {
        Debug.Log($"SetStat -> {nombre} = {value}");
        // Icono del Prefab
        if (statusIcon != null && icon != null)
        {
            Debug.Log("StatusIcon o Icon es null");
            statusIcon.sprite = icon;
        }

        // Fondo azul (o el color que prefieras)
        if (background != null)
            background.color = new Color(0.25f, 0.35f, 0.75f, 0.8f);

        // Valor (+3, -2, etc.)
        if (valueText != null)
        {
            if(value > 0)
            {
                valueText.text = "+" + value.ToString();
                valueText.enabled = true;
            }
            if(value < 0)
            {
                Debug.Log("Value es negativo");
                valueText.text = "-" + value.ToString();
                valueText.enabled = true;
            }

        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        Debug.Log("Llego al final de SetStat");
    }
    public void SetValueText(string text)
    {
        if (valueText != null)
        {
            valueText.text = text;
            valueText.enabled = true;
        }
    }
    public void ShowTooltip(string description)
    {
        // 🔥 Opcional: mostrar tooltip al hacer hover
        Debug.Log($"Estado: {description}");
    }
    //public void OnPointerEnter()
    //{
    //    ShowTooltip(currentStatus.effect.description);
    //}
}