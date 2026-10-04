using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/AddShieldEffect")]
public class AddShieldEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        target.AddShield(value, duration);
    }
}
