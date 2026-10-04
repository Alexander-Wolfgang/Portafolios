using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Heal")]
public class EnemyHealEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        Debug.LogWarning("EnemyHealEffect no debe usarse con cartas");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Debug.LogWarning("Usa Execute en vez de ApplyToEnemy");
    }

    public void Execute(Enemy caster, CombatManager combatManager)
    {
        if (caster == null || combatManager == null)
        {
            Debug.LogError("Caster o CombatManager NULL en EnemyHealEffect");
            return;
        }

        int healAmount = GetFinalValue(caster);

        var enemies = combatManager.enemies
            .Where(e => e != null && !e.IsDead)
            .ToList();

        if (enemies.Count == 0)
            return;

        switch (type)
        {
            case 1: // Curar a 1 enemigo (el más dañado)
                Enemy target = enemies
                    .OrderBy(e => (float)e.currentHP / e.MaxHP)
                    .First();

                target.Heal(healAmount);

                Debug.Log($"{caster.name} cura a {target.name} por {healAmount}");
                break;

            case 2: // Curar a todos
                foreach (var e in enemies)
                {
                    e.Heal(healAmount);
                }

                Debug.Log($"{caster.name} cura a TODO el equipo por {healAmount}");
                break;

            default:
                Debug.LogWarning($"Type {type} no válido en EnemyHealEffect");
                break;
        }
    }

    // Incluir la magia del caster en el valor final mostrado y usado
    public override int GetFinalValue(Enemy enemy)
    {
        int baseValue = value;
        int magia = (enemy != null) ? enemy.Magia : 0;
        return Mathf.Max(0, baseValue + magia);
    }

    // Opcional: texto de intención consistente (si quieres)
    public override string GetIntentText(Enemy enemy)
    {
        int finalValue = GetFinalValue(enemy);
        if (duration > 1)
            return $"{finalValue} ({duration})";
        return finalValue.ToString();
    }
}