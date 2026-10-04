using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/If/Damage Based On Defense")]
public class If_DamageBasedOnDefense : IfEffect
{
    public int baseDamage = 6;
    public int bonusDamage = 6;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null) return;

        CombatManager attacker = source.owner;
        int totalDamage = baseDamage;

        if (attacker.TotalShield >= target.TotalShield * 2)
        {
            totalDamage += bonusDamage;
            Debug.Log($"{attacker.characterName} tiene el doble de defensa que {target.characterName}. Inflige {totalDamage} daño total!");
        }
        else
        {
            Debug.Log($"{attacker.characterName} inflige {baseDamage} de daño base.");
        }

        target.TakeDamage(totalDamage, attacker);
    }
}