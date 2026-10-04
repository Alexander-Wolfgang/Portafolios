using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Deal Damage")]
public class EnemyDealDamageEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        target.TakeDamage(GetFinalValue(null), null);
    }

    public void Execute(Enemy enemy, CombatManager player)
    {
        int dmg = GetFinalValue(enemy);
        player.TakeDamage(dmg, null);

        Debug.Log($"{enemy.name} hace {dmg} de daño");
    }

    // Añadir la fuerza del enemigo al daño final
    public override int GetFinalValue(Enemy enemy)
    {
        int baseValue = value;
        int fuerza = (enemy != null) ? enemy.Fuerza : 0;
        return Mathf.Max(0, baseValue + fuerza);
    }
}