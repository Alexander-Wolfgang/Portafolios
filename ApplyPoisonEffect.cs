using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Apply Poison")]
public class ApplyPoisonEffect : Effect
{
    public StatusEffect poisonStatus;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null) return;

        target.AddStatus(poisonStatus, value, duration);

        Debug.Log($"{target.characterName} recibe veneno ({value}) por {duration} turnos");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null) return;

        enemy.activeStatuses.Add(new ActiveStatus(poisonStatus, value, duration));

        Debug.Log($"{enemy.name} recibe veneno ({value}) por {duration} turnos");
    }
}