using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Buff Temp")]
public class BuffTempEffect : Effect
{
    public StatusEffect statusToApply;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null) return;

        target.AddStat(type, value);

        if (statusToApply == null)
        {
            Debug.LogError($"BuffTempEffect '{name}' no tiene 'statusToApply' asignado.");
            return;
        }

        target.AddStatus(statusToApply, value, duration);
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null) return;

        if (statusToApply == null)
        {
            Debug.LogError($"BuffTempEffect '{name}' no tiene 'statusToApply' asignado (ApplyToEnemy).");
            return;
        }

        enemy.AddStatus(statusToApply, value, duration, type);
    }
}