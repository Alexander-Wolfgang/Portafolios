using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/DealDamageEffect")]
public class DealDamageEffect : Effect
{
    // OBLIGATORIO porque Effect lo pide
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null)
        {
            Debug.LogError("Target NULL en DealDamageEffect.Apply");
            return;
        }

        int damage = value;

        CombatManager attacker = source != null ? source.owner : null;

        Debug.Log($"Owner: {attacker.characterName}");
        Debug.Log($"Fuerza del owner: {attacker.Fuerza}");
        Debug.Log($"Daño base: {value}");

        damage += attacker.Fuerza;

        string attackerName = attacker != null ? attacker.characterName : "Unidad";
        string actionName = (source != null && source.cardData != null)
            ? source.cardData.cardName
            : effectName;

        target.TakeDamage(damage, attacker);

        Debug.Log($"{actionName} -> {attackerName} causa {damage} daño a {target.characterName}");
    }

    // Opcional: puedes dejar esto o eliminarlo
    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy NULL en DealDamageEffect");
            return;
        }

        int damage = value;

        CombatManager attacker = source != null ? source.owner : null;

        if (attacker != null)
            damage += attacker.Fuerza;

        enemy.TakeDamage(damage);

        Debug.Log($"{effectName} hace {damage} daño a {enemy.name}");
    }
}