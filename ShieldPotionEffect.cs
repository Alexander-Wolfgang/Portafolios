using UnityEngine;

[CreateAssetMenu(menuName = "Potions/Effects/Shield")]
public class ShieldPotionEffect : PotionEffect
{
    public int shieldAmount = 10;
    public int duration = 2;

    public override void Apply(CombatManager user)
    {
        user.AddShield(shieldAmount, duration);

        Debug.Log($"Poción otorga {shieldAmount} de escudo por {duration} turnos");
    }
}