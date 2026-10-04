using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/If/Heal Based On Shield")]
public class If_HealBasedOnShield : IfEffect
{
    public int baseHeal = 5;
    public int bonusHeal = 7;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        int totalHeal = baseHeal + target.Magia;

        if (target.currentHealth > target.TotalShield)
        {
            totalHeal += bonusHeal;
            Debug.Log($"{target.characterName} tiene más salud que escudo. Se cura {totalHeal} de salud total!");
        }
        else
        {
            Debug.Log($"{target.characterName} se cura {baseHeal} de salud base.");
        }

        target.Heal(totalHeal);
    }
}
