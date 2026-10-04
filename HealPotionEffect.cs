using UnityEngine;

[CreateAssetMenu(menuName = "Potions/Effects/Heal")]
public class HealPotionEffect : PotionEffect
{
    public int healAmount = 10;

    public override void Apply(CombatManager target)
    {
        target.Heal(healAmount);

        Debug.Log($"Poción cura {healAmount}");
    }
}