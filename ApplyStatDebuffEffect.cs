using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Stat Debuff")]
public class ApplyStatDebuffEffect : Effect
{
    public StatusEffect statusEffect;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null)
        {
            Debug.LogError("Target NULL en ApplyStatDebuffEffect");
            return;
        }

        if (statusEffect == null)
        {
            Debug.LogError("statusEffect NULL en ApplyStatDebuffEffect");
            return;
        }

        // 🔥 Aplicar al jugador
        target.AddStatus(statusEffect, value, duration);
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy NULL en ApplyStatDebuffEffect");
            return;
        }

        if (statusEffect == null)
        {
            Debug.LogError("statusEffect NULL en ApplyStatDebuffEffect");
            return;
        }

        if (enemy.activeStatuses == null)
        {
            enemy.activeStatuses = new List<ActiveStatus>();
        }

        // 🔥 Aplicar debuff inmediato
        enemy.AddStat(type, -value);

        // 🔥 Crear status
        ActiveStatus newStatus = new ActiveStatus(statusEffect, value, duration);
        newStatus.type = type;

        // 🔥 Agregar status
        enemy.activeStatuses.Add(newStatus);

        Debug.Log($"{enemy.name} pierde {value} de stat tipo {type} por {duration} turnos");

        // 🔥 ACTUALIZAR UI
        if (enemy.statusUI != null)
        {
            enemy.statusUI.UpdateStatuses();
        }
        else
        {
            Debug.LogWarning($"statusUIManager NULL en {enemy.name}");
        }

        // 🔥 Recalcular intención
        enemy.RecalcularIntencion();
    }
}