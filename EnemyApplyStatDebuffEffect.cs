using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Stat Debuff")]
public class EnemyApplyStatDebuffEffect : Effect
{
    public StatusEffect statusEffect;

    // 🔥 Obligatorio por herencia
    public override void Apply(CardRuntime source, CombatManager target)
    {
        Debug.LogWarning("EnemyApplyStatDebuffEffect no debería usar Apply()");
    }

    // 🔥 Este es el que usa el enemigo
    public void Execute(Enemy enemy, CombatManager player)
    {
        if (player == null) return;

        player.AddStatus(statusEffect, value, duration);

        Debug.Log($"{enemy.name} aplica debuff al jugador: {statusEffect.effectName}");
    }
}