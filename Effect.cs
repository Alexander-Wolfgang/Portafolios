using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [Header("Datos básicos")]
    public string effectName;
    public Sprite icon;

    [Header("Valores")]
    public int value;
    public int duration = 1;

    [Header("Buffs")]
    // 1 = Fuerza
    // 2 = Robustez
    // 3 = Magia
    public int type = 1;

    [Header("Objetivo")]
    public EffectTarget target = EffectTarget.Self;

    public abstract void Apply(CardRuntime source, CombatManager target);

    public virtual void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy es NULL en ApplyToEnemy");
            return;
        }

        if (enemy.PCM == null)
        {
            Debug.LogError("Enemy.PCM es NULL");
            return;
        }

        Debug.LogWarning($"{effectName} no implementa ApplyToEnemy()");
        Apply(source, enemy.PCM);
    }

    public virtual int GetFinalValue(Enemy enemy)
    {
        return value;
    }

    public virtual string GetIntentText(Enemy enemy)
    {
        int finalValue = GetFinalValue(enemy);

        if (duration > 1)
            return $"{finalValue} ({duration})";

        return finalValue.ToString();
    }

    public enum EffectTarget
    {
        Self,
        Enemy
    }
}