using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/SacrificeSelectCardEffect")]
public class SacrificeSelectCard : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        CombatManager owner = source != null ? source.owner : null;

        if (owner == null)
        {
            Debug.LogError("Owner NULL en SacrificeSelectCardEffect");
            return;
        }

        if (owner.deck == null)
        {
            Debug.LogError("Deck NULL en SacrificeSelectCardEffect");
            return;
        }

        int amount = Mathf.Max(1, value);

        // 🔥 cartas válidas (excluyendo la carta que estás jugando)
        int validCards = owner.deck.HandCount - 1;

        // ❌ no puede pagar
        if (validCards < amount)
        {
            Debug.Log("No hay suficientes cartas para sacrificar");

            owner.cancelCardExecution = true; // 🔥 cancela TODA la carta
            return;
        }

        // ✅ iniciar selección múltiple
        owner.StartCardSelectionForSacrifice(source, amount);

        Debug.Log($"{owner.characterName} debe elegir {amount} carta(s) para sacrificar");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Debug.Log("SacrificeSelectCardEffect no aplica a enemigos");
    }
}