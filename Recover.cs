using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/HealOverTime")]
public class Recover : Effect
{
    public StatusEffect regenerationStatus;
    public override void Apply(CardRuntime source, CombatManager target)
    {
        Debug.LogWarning("EnemyHealOverTimeEffect no debe usarse con cartas");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Debug.LogWarning("Usa Execute en vez de ApplyToEnemy");
    }

    public void Execute(Enemy caster, CombatManager combatManager)
    {
        if (caster == null || combatManager == null)
        {
            Debug.LogError("Caster o CombatManager NULL");
            return;
        }

        int healPerTurn = value + caster.Magia;

        // 🔥 AQUÍ ESTÁ LA CLAVE
        combatManager.AddStatus(regenerationStatus, healPerTurn, duration);

        Debug.Log($"{caster.name} obtiene regeneración");
    }
}