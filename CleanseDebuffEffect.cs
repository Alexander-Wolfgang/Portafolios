using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Cleanse Debuff (Flexible)")]
public class CleanseDebuffEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target.activeStatuses == null || target.activeStatuses.Count == 0)
        {
            Debug.Log("No hay estados");
            return;
        }

        // 🔻 Filtrar SOLO debuffs
        var debuffs = target.activeStatuses
            .Where(s => s.effect != null && s.effect.category == StatusEffect.StatusCategory.Debuff)
            .ToList();

        if (debuffs.Count == 0)
        {
            Debug.Log("No hay debuffs para limpiar");
            return;
        }

        // =========================
        // 🟣 TYPE 2 → LIMPIAR TODOS
        // =========================
        if (type == 2)
        {
            Debug.Log("Se eliminan TODOS los debuffs");

            foreach (var debuff in debuffs)
            {
                target.activeStatuses.Remove(debuff);
            }

            return;
        }

        // =========================
        // 🔵 TYPE 1 → LIMPIAR N (value)
        // =========================
        int cleanses = Mathf.Max(1, value);

        for (int i = 0; i < cleanses; i++)
        {
            // refrescar lista por si ya eliminaste cosas
            debuffs = target.activeStatuses
                .Where(s => s.effect != null && s.effect.category == StatusEffect.StatusCategory.Debuff)
                .ToList();

            if (debuffs.Count == 0)
                break;

            var randomDebuff = debuffs[Random.Range(0, debuffs.Count)];

            Debug.Log($"Se elimina debuff: {randomDebuff.effect.effectName}");

            target.activeStatuses.Remove(randomDebuff);
        }
    }
}