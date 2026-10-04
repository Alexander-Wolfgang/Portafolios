using UnityEngine;

[CreateAssetMenu(menuName = "Potions/Effects/Stat Buff")]
public class StatBuffPotionEffect : PotionEffect
{
    [Header("Buff")]
    public int value = 1;

    [Tooltip("1=Fuerza, 2=Robustez, 3=Magia")]
    public int type = 1;

    public bool perma = true;

    [Min(1)]
    public int duration = 1;

    [Header("Estado temporal (solo si perma = false)")]
    public StatusEffect temporaryBuffStatus;

    public override void Apply(CombatManager target)
    {
        if (perma)
        {
            target.AddStat(type, value);

            Debug.Log($"Poción otorga +{value} al stat {type} durante el combate.");
            return;
        }
        if (!perma && temporaryBuffStatus == null)
        {
            Debug.LogError("No se asignó temporaryBuffStatus");
            //temporaryBuffStatus.statType = type;
            return;
        }

        // Aplicar inmediatamente
        target.AddStat(type, value);

        // Crear buff temporal usando tu sistema de estados
        target.AddStatus(
            temporaryBuffStatus,
            value,
            duration
        );

        Debug.Log($"Poción otorga +{value} al stat {type} por {duration} turnos.");
    }
}