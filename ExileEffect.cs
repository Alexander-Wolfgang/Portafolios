using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/ExileEffect")]
public class ExileEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (source == null)
        {
            Debug.LogError("Source NULL en ExileEffect");
            return;
        }

        source.exhaustOnUse = true;

        Debug.Log($"{source.cardData.cardName} será exiliada tras usarse");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        Apply(source, null);
    }
}