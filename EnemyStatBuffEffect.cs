using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/StatBuff")]
public class EnemyStatBuffEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        Debug.LogWarning("EnemyStatBuffEffect no debe usarse con cartas");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Debug.LogWarning("Usa Execute en vez de ApplyToEnemy");
    }

    public void Execute(Enemy enemy, CombatManager combatManager)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy NULL en EnemyStatBuffEffect");
            return;
        }

        int amount = value;

        switch (type)
        {
            case 1: // 🔥 Fuerza
                enemy.Fuerza += amount;
                Debug.Log($"{enemy.name} gana +{amount} de Fuerza. Total: {enemy.Fuerza}");
                break;

            case 2: // 🔥 Magia
                enemy.Magia += amount;
                Debug.Log($"{enemy.name} gana +{amount} de Magia. Total: {enemy.Magia}");
                break;

            case 3: // 🔥 Robustez
                enemy.Robustez += amount;
                Debug.Log($"{enemy.name} gana +{amount} de Magia. Total: {enemy.Magia}");
                break;

            default:
                Debug.LogWarning($"Duration {duration} no válida en EnemyStatBuffEffect");
                break;
        }
    }

    // Mostrar la duración solo si es mayor que 1, y usar el valor absoluto
    public override string GetIntentText(Enemy enemy)
    {
        int displayValue = Mathf.Abs(value);
        if (duration > 1)
            return $"{displayValue} ({duration})";
        return displayValue.ToString();
    }
}