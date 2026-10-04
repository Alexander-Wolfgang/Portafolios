using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Shield All")]
public class EnemyShieldAllEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        // No aplica al jugador
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null) return;
        if (enemy.PCM == null) return;

        Execute(enemy.PCM);
    }

    public void Execute(CombatManager pcm)
    {
        foreach (Enemy e in pcm.enemiesInCombat)
        {
            if (e == null || e.IsDead) continue;

            e.AddArmor(value, duration);

            Debug.Log($"{e.name} gana {value} de escudo por {duration} turnos");
        }
    }
}