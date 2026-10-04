using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/SacrificeRandomCardEffect")]
public class SacrificeRandomCardEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        CombatManager owner = source != null ? source.owner : null;

        if (owner == null)
        {
            Debug.LogError("Owner NULL en SacrificeRandomCardEffect");
            return;
        }

        if (owner.deck == null)
        {
            Debug.LogError("Deck NULL en SacrificeRandomCardEffect");
            return;
        }

        int amount = Mathf.Max(0, value);
        int validCards = owner.deck.HandCount - 1;

        // Valida si tienes suficientes cartas para sacrificar (excluyendo la carta que se está ejecutando)
        if (validCards < amount)
        {
            Debug.Log("No hay suficientes cartas para sacrificar");

            owner.cancelCardExecution = true; // 🔥 cancela la carta completa
            return;
        }

        //Si value es 0, no hace nada (Value no deberia ser 0, eso significa que lo configure mal)
        if (amount == 0)
        {
            Debug.Log("SacrificeRandomCardEffect con value 0, no hace nada");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            // 🔒 Si no hay cartas en mano, detener
            if (owner.deck.HandCount == 0)
            {
                Debug.Log("No hay más cartas para sacrificar");
                break;
            }

            owner.deck.SacrificeRandom();
        }

        Debug.Log($"{owner.characterName} sacrifica {amount} carta(s) aleatoria(s)");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Debug.Log("SacrificeRandomCardEffect no aplica a enemigos");
    }
}