using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Purge")]
public class Purge : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (source == null)
        {
            Debug.LogError("Source NULL en PurgeEffect");
            return;
        }

        source.purgeOnUse = true;

        Debug.Log($"{source.cardData.cardName} será purgada tras usarse");
    }
}