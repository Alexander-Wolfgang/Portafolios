using UnityEngine;

[CreateAssetMenu(menuName = "Potions/Effects/Draw")]
public class DrawPotionEffect : PotionEffect
{
    public int drawAmount = 1;

    public override void Apply(CombatManager target)
    {
        target.deck.Draw(drawAmount);

        Debug.Log($"Poción roba {drawAmount} carta(s)");
    }
}