using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Cleanse")]
public class EnemyCleanseEffect : Effect
{
    public bool cleanseAllAllies = true; // true = equipo, false = solo caster

    public override void Apply(CardRuntime source, CombatManager target)
    {
        Debug.LogWarning("EnemyCleanseEffect no debe usarse con cartas");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Debug.LogWarning("Usa Execute en vez de ApplyToEnemy");
    }

    public void Execute(Enemy caster, CombatManager combatManager)
    {
        if (caster == null || combatManager == null)
        {
            Debug.LogError("Caster o CombatManager NULL en EnemyCleanseEffect");
            return;
        }

        var enemies = combatManager.enemies
            .Where(e => e != null && !e.IsDead)
            .ToList();

        if (enemies.Count == 0)
            return;

        if (cleanseAllAllies)
        {
            foreach (var e in enemies)
            {
                Cleanse(e);
            }

            Debug.Log($"{caster.name} limpia estados de TODO el equipo");
        }
        else
        {
            Cleanse(caster);
            Debug.Log($"{caster.name} se limpia sus estados");
        }
    }

    void Cleanse(Enemy enemy)
    {
        // 🔥 Aquí decides qué remover
        // Opción simple: limpiar TODO (si tienes lista de estados en Enemy)

        if (enemy.PCM != null)
        {
            enemy.PCM.activeStatuses.Clear();
        }
    }
}