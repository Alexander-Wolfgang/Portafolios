using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/ApplyStatusEffect")]
public class ApplyStatusEffect : Effect
{
    public StatusEffect status;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        target.AddStatus(status, value, duration);
    }
}