using UnityEngine;

[CreateAssetMenu(menuName = "Potions/Effects/Energy")]
public class EnergyPotionEffect : PotionEffect
{
    public int energyAmount = 1;

    public override void Apply(CombatManager user)
    {
        if (!user.usaEnergia)
            return;

        user.energia_Actual += energyAmount;

        user.energia_Actual = Mathf.Min(
            user.energia_Actual,
            user.energiaBase
        );

        user.ActualizarRayos();

        Debug.Log($"Poción otorga {energyAmount} de energía");
    }
}