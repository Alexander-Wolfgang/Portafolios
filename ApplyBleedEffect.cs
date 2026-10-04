using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effects/Apply Bleed")]
public class ApplyBleedEffect : Effect
{
    public StatusEffect bleedStatus;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        ApplyBleed(target.activeStatuses, target.characterName);
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        ApplyBleed(enemy.activeStatuses, enemy.name);
    }

    private void ApplyBleed(List<ActiveStatus> list, string targetName)
    {
        foreach (var status in list)
        {
            if (status.effect.effectName == "Hemorragia")
            {
                // 🔥 YA EXISTE → SUMAR AL ESTALLIDO
                status.burstValue += value;

                Debug.Log($"{targetName} acumula hemorragia. Daño final ahora: {status.burstValue}");
                return;
            }
        }

        // 🔥 NO EXISTE → CREAR NUEVA
        var newStatus = new ActiveStatus(bleedStatus, type, duration);
        // 👆 daño por turno = 1 fijo (puedes hacerlo variable si quieres)

        newStatus.burstValue = value; // daño final

        list.Add(newStatus);

        Debug.Log($"{targetName} recibe hemorragia ({value}) por {duration} turnos");
    }
}