using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Shield")]
public class EnemyShieldEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        // No aplica al jugador
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Execute(enemy);
    }

    public void Execute(Enemy enemy)
    {
        if (enemy == null) return;

        enemy.AddArmor(value, duration);

        Debug.Log($"{enemy.name} gana {value} de escudo por {duration} turnos");
    }
}