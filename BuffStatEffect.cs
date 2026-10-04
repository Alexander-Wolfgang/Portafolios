using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Buff Stat")]
public class BuffStatEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null) return;

        int amount = value;

        switch (type)
        {
            case 1: // Fuerza
                target.Fuerza += amount;
                Debug.Log($"{target.characterName} gana +{amount} Fuerza");
                break;

            case 2: // Magia
                target.Magia += amount;
                Debug.Log($"{target.characterName} gana +{amount} Magia");
                break;

            case 3: // Robustez
                target.Robustez += amount;
                Debug.Log($"{target.characterName} gana +{amount} Robustez");
                break;

            default:
                Debug.LogWarning("Tipo de buff desconocido");
                break;
        }
    }
}