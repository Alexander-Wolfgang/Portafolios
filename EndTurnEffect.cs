using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/EndTurnEffect")]
public class EndTurnEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        CombatManager actor = source != null ? source.owner : null;

        string actorName = actor != null ? actor.characterName : "Desconocido";
        Debug.Log($"{actorName} usa {effectName} y termina su turno.");

        if (actor != null)
        {
            actor.EndTurnButton(); // 🔥 ahora usa el CombatManager
        }
        else
        {
            Debug.LogWarning("EndTurnEffect: actor es null.");
        }
    }
}