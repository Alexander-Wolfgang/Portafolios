using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/If/Shield Based On Low Health")]
public class If_ShieldBasedOnLowHealth : IfEffect
{
    public int baseShield = 3;
    public int bonusShield = 7;
    public float porcent = 0.25f;

    public override void Apply(CardRuntime source, CombatManager target)
    {
        int totalShield = baseShield;

        if ((float)target.currentHealth / target.maxHealth <= porcent)
        {
            totalShield += bonusShield;
            Debug.Log($"{target.characterName} tiene menos del 25% de salud. Gana {totalShield} de escudo!");
        }
        else
        {
            Debug.Log($"{target.characterName} gana {baseShield} de escudo base.");
        }

        target.AddShield(totalShield, 2);
    }
}
