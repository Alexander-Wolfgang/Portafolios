using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Debuff Stat")]
public class StatDebuffEffect : Effect
{
    public StatusEffect statusEffect;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null || statusEffect == null)
        {
            Debug.LogError("Falta target o statusEffect");
            return;
        }

        int finalValue = -Mathf.Abs(value);

        target.AddStat(statusEffect.statType, finalValue);

        target.activeStatuses.Add(new ActiveStatus(statusEffect, Mathf.Abs(value), duration));

        Debug.Log($"{target.characterName} pierde {Mathf.Abs(value)} de stat por {duration} turnos");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null) return;

        enemy.AddStatus(statusEffect, value, duration, type);
    }

    public override string GetIntentText(Enemy enemy)
    {
        // Mostrar la duración solo si es mayor que 1 (evita el " (1)" cuando duration == 1)
        int displayValue = Mathf.Abs(value);
        if (duration > 1)
            return $"{displayValue} ({duration})";
        return displayValue.ToString();
    }
}