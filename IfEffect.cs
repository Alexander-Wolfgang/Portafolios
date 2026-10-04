using UnityEngine;

public abstract class IfEffect : Effect
{
    // Todos los efectos if usan este método
    public abstract override void Apply(CardRuntime source, CombatManager target);
}
