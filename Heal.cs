using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/HealEffect")]
public class Heal : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null)
        {
            Debug.LogError("Target NULL en HealEffect");
            return;
        }

        int amount = Mathf.Max(0, value);

        // La Magia solo aumenta la curación de cartas
        CombatManager caster = source != null ? source.owner : null;

        if (caster != null)
            amount += caster.Magia;

        if (amount <= 0)
        {
            Debug.Log("HealEffect con valor 0, no hace nada");
            return;
        }

        target.Heal(amount);

        Debug.Log($"{target.characterName} se cura {amount} HP");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy NULL en HealEffect");
            return;
        }

        int amount = Mathf.Max(0, value);

        // La Magia del lanzador también afecta si una carta cura enemigos
        CombatManager caster = source != null ? source.owner : null;

        if (caster != null)
            amount += caster.Magia;

        if (amount <= 0)
            return;

        enemy.Heal(amount);

        Debug.Log($"{enemy.name} se cura {amount} HP");
    }
}