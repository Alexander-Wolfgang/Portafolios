using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Increase Shield Duration")]
public class IncreaseShieldDurationEffect : Effect
{
    public int extraTurns = 1;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null)
        {
            Debug.LogWarning("No hay objetivo para IncreaseShieldDurationEffect.");
            return;
        }

        target.IncreaseShieldDuration(extraTurns);
    }
}