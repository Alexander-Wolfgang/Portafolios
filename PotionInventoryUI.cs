using System.Collections.Generic;
using UnityEngine;

public class PotionInventoryUI : MonoBehaviour
{
    public static PotionInventoryUI Instance;

    public List<PotionSlotUI> slots;
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        var potions = PotionManager.Instance.GetPotions();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Setup(i);

            if (i < potions.Count)
                slots[i].SetPotion(potions[i]);
            else
                slots[i].SetPotion(null);
        }
    }
}