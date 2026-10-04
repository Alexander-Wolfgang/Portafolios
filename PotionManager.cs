using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{
    public static PotionManager Instance;

    [Header("Slots")]
    [SerializeField] private List<PotionData> potions = new();

    public List<PotionData> GetPotions()
    {
        return potions;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    [SerializeField] private PotionData testPotion;
    private void Start()
    {
        //AddPotion(testPotion);
    }
    public int AddPotion(PotionData potion)
    {
        if (potions.Count >= 10)
        {
            Debug.Log("Inventario de pociones lleno");
            return 0;
        }

        potions.Add(potion);
        return 1;
        //RefreshUI();
    }

    public void UsePotion(int index, CombatManager owner)
    {
        if (index < 0 || index >= potions.Count)
            return;

        PotionData potion = potions[index];

        if (potion == null)
            return;

        foreach (var effect in potion.effects)
        {
            effect.Apply(owner);
        }

        Debug.Log($"Usaste poción: {potion.potionName}");

        potions.RemoveAt(index);

        //RefreshUI();
    }
}