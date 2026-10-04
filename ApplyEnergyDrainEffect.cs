using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Apply Energy Drain")]
public class ApplyEnergyDrainEffect : Effect
{
    public StatusEffect energyDrainStatus;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null) return;

        ApplyOrExtend(target);

        Debug.Log($"{target.characterName} sufre drenaje de energía ({value}) por {duration} turnos");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null) return;

        ApplyOrExtendEnemy(enemy);

        Debug.Log($"{enemy.name} sufre drenaje de energía ({value}) por {duration} turnos");
    }

    void ApplyOrExtend(CombatManager target)
    {
        var existing = target.activeStatuses
            .Find(s => s.effect.effectName == energyDrainStatus.effectName);

        if (existing != null)
        {
            // 🔥 NO stackea → solo extiende duración
            existing.duration += duration;
        }
        else
        {
            target.activeStatuses.Add(
                new ActiveStatus(energyDrainStatus, value, duration)
            );
        }
    }

    void ApplyOrExtendEnemy(Enemy enemy)
    {
        var existing = enemy.activeStatuses
            .Find(s => s.effect.effectName == energyDrainStatus.effectName);

        if (existing != null)
        {
            existing.duration += duration;
        }
        else
        {
            enemy.activeStatuses.Add(
                new ActiveStatus(energyDrainStatus, value, duration)
            );
        }
    }
}