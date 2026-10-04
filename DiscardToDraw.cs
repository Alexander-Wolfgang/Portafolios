using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "DiscardToDraw", menuName = "Cards/Effects/DiscardToDraw")]
public class DiscardToDraw : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        CombatManager owner = source != null ? source.owner : null;

        if (owner == null)
        {
            Debug.LogError("Owner NULL en RecoverRandomFromDiscardEffect");
            return;
        }

        if (owner.deck == null)
        {
            Debug.LogError("Deck NULL en RecoverRandomFromDiscardEffect");
            return;
        }

        if (owner.deck.DiscardCount == 0)
        {
            Debug.Log("No hay cartas en el cementerio");
            return;
        }

        int amount = Mathf.Max(1, value);
        int mode = Mathf.Clamp(duration, 1, 3); // 1 down drawpile, 2 random, 3 up drawpile

        for (int i = 0; i < amount; i++)
        {
            if (owner.deck.DiscardCount == 0)
                break;

            owner.deck.MoveRandomDiscardToDrawPile(mode);
        }

        Debug.Log($"{owner.characterName} recupera {amount} carta(s) del cementerio (modo {mode})");
        //Deck.ContMazo();
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Debug.Log("RecoverRandomFromDiscardEffect no aplica a enemigos");
    }
}
