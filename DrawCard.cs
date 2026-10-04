using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/DrawCardEffect")]
public class DrawCard : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        CombatManager owner = source != null ? source.owner : null;

        if (owner == null)
        {
            Debug.LogError("Owner NULL en DrawCardEffect");
            return;
        }

        if (owner.deck == null)
        {
            Debug.LogError("Deck NULL en DrawCardEffect");
            return;
        }

        int drawAmount = Mathf.Max(0, value); // 🔥 clave: usar Value

        if (drawAmount == 0)
        {
            Debug.Log("DrawCardEffect con value 0, no roba cartas");
            return;
        }

        for (int i = 0; i < drawAmount; i++)
        {
            // 🔒 límite de mano
            if (owner.deck.HandCount >= owner.manoMaxima)
            {
                Debug.Log("Mano llena, no puedes robar más cartas");
                break;
            }

            // 🚫 sin reshuffle
            if (owner.deck.DrawPileCount == 0)
            {
                Debug.Log("No hay cartas en el mazo");
                break;
            }

            owner.deck.Draw(1);
        }

        Debug.Log($"{owner.characterName} roba {drawAmount} carta(s)");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Debug.Log("DrawCardEffect no aplica a enemigos");
    }
}