using UnityEngine;
using UnityEngine.UI;

public class PotionSlotUI : MonoBehaviour
{
    public Image icon;

    private PotionData currentPotion;
    public PotionData CurrentPotion => currentPotion;

    private int slotIndex;

    public void Setup(int index)
    {
        slotIndex = index;
    }

    public void SetPotion(PotionData potion)
    {
        currentPotion = potion;

        if (potion == null)
        {
            icon.enabled = false;

            Button btn = GetComponent<Button>();
            if (btn != null)
                btn.interactable = false;

            return;
        }

        icon.enabled = true;
        icon.sprite = potion.icon;

        Button button = GetComponent<Button>();
        if (button != null)
            button.interactable = true;
    }

    public void Click()
    {
        if (CombatManager.Instance == null)
        {
            Debug.Log("Las pociones solo pueden usarse en combate");
            return;
        }
        if(slotIndex < 0)
        {
            Debug.Log("El slot de poción está vacio.");
            return;
        }

        PotionManager.Instance.UsePotion(slotIndex, CombatManager.Instance);

        GetComponentInParent<PotionInventoryUI>().RefreshUI();
    }
    public string GetDescription()
    {
        return currentPotion != null ? currentPotion.description : "";
    }
}