using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonHoverTorch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Referencia al componente de imagen")]
    public Image targetImage;

    [Header("Color y comportamiento del brillo")]
    public Color baseGlowColor = new Color(1f, 0.55f, 0.2f); // Naranja cálido
    public float flickerSpeed = 6f;      // Velocidad del parpadeo
    public float flickerIntensity = 0.3f; // Variación máxima del brillo

    private bool hovering;
    private bool isActive;
    private Color baseColor;

    // Lista estática con todos los botones registrados
    private static List<ButtonHoverTorch> allButtons = new List<ButtonHoverTorch>();

    void Awake()
    {
        if (!targetImage)
            targetImage = GetComponent<Image>();

        if (targetImage != null)
            baseColor = targetImage.color;

        if (!allButtons.Contains(this))
            allButtons.Add(this);
    }

    void OnDestroy()
    {
        if (allButtons.Contains(this))
            allButtons.Remove(this);
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (hovering || isActive)
        {
            // Parpadeo tipo antorcha
            float flicker = (Mathf.Sin(Time.time * flickerSpeed) + Random.Range(-0.2f, 0.2f)) * flickerIntensity;
            float intensity = 1f + flicker;

            Color finalColor = new Color(
                Mathf.Clamp01(baseGlowColor.r * intensity),
                Mathf.Clamp01(baseGlowColor.g * intensity),
                Mathf.Clamp01(baseGlowColor.b * intensity),
                1f
            );

            targetImage.color = finalColor;
        }
        else
        {
            // Regresa al color base (transparente)
            targetImage.color = Color.Lerp(targetImage.color, baseColor, Time.deltaTime * 6f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Al entrar, apagar todos los demás botones (hover o activos)
        foreach (var btn in allButtons)
        {
            if (btn != this)
            {
                btn.hovering = false;
                btn.isActive = false;
                btn.ResetColor();
            }
        }

        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Al hacer clic, apagar todos los demás botones
        foreach (var btn in allButtons)
        {
            if (btn != this)
            {
                btn.hovering = false;
                btn.isActive = false;
                btn.ResetColor();
            }
        }

        // Este botón pasa a ser el activo
        hovering = false;
        isActive = true;
    }

    public void ResetColor()
    {
        targetImage.color = baseColor;
    }
}
