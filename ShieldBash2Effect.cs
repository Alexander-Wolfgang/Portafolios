using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/ShieldBash2Effect")]
public class ShieldBash2Effect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null)
        {
            Debug.LogError("Target NULL en ShieldBashEffect.Apply");
            return;
        }

        CombatManager attacker = source != null ? source.owner : null;

        if (attacker == null)
        {
            Debug.LogError("Attacker NULL en ShieldBashEffect.Apply");
            return;
        }

        // 🔹 Obtener TODO el escudo
        int shieldAmount = attacker.TotalShield;

        if (shieldAmount <= 0)
        {
            Debug.Log("No tienes escudo para usar Shield Bash");
            return;
        }

        string attackerName = attacker.characterName;
        string actionName = (source != null && source.cardData != null)
            ? source.cardData.cardName
            : effectName;

        attacker.BreakOneShield();      // 🔹 consumir 1 escudo

        // 🔹 aplicar daño
        target.TakeDamage(shieldAmount, attacker);

        Debug.Log($"{actionName} -> {attackerName} consume {shieldAmount} de escudo e inflige {shieldAmount} daño a {target.characterName}");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy NULL en ShieldBashEffect");
            return;
        }

        CombatManager attacker = source != null ? source.owner : null;

        if (attacker == null)
        {
            Debug.LogError("Attacker NULL en ShieldBashEffect.ApplyToEnemy");
            return;
        }

        int shieldAmount = attacker.TotalShield;

        if (shieldAmount <= 0)
        {
            Debug.Log("No hay escudo para Shield Bash");
            return;
        }

        attacker.BreakOneShield();      // 🔹 consumir 1 escudo

        enemy.TakeDamage(shieldAmount);

        Debug.Log($"{effectName} consume {shieldAmount} de escudo y hace {shieldAmount} daño a {enemy.name}");
    }
}