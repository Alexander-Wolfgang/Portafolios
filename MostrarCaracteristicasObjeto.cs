using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MostrarCaracteristicasObjeto :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private PotionSlotUI slotUI;
    void Awake()
    {
        slotUI = GetComponent<PotionSlotUI>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotUI == null || slotUI.CurrentPotion == null)
            return;
        Panel_Caracteristicas_Objetos.Instance.Mostrar(transform as RectTransform,slotUI.CurrentPotion.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Panel_Caracteristicas_Objetos.Instance.Ocultar();
    }
}