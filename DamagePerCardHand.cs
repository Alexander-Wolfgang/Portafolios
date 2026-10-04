using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/DamagePerCardHand")]
public class DamagePerCardHand : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null)
        {
            Debug.LogError("Target NULL en DamagePerCardInHandEffect");
            return;
        }

        CombatManager attacker = source != null ? source.owner : null;

        if (attacker == null)
        {
            Debug.LogError("Attacker NULL en DamagePerCardInHandEffect");
            return;
        }

        if (attacker.deck == null)
        {
            Debug.LogError("Deck NULL en DamagePerCardInHandEffect");
            return;
        }

        int perCard = Mathf.Max(0, value);
        int handCount = attacker.deck.HandCount;

        if (handCount <= 0 || perCard <= 0)
        {
            Debug.Log("No hay cartas en mano o value es 0");
            return;
        }

        int totalDamage = perCard * handCount;

        target.TakeDamage(totalDamage, attacker);

        Debug.Log($"{attacker.characterName} inflige {totalDamage} daño ({perCard} x {handCount} cartas en mano) a {target.characterName}");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy NULL en DamagePerCardInHandEffect");
            return;
        }

        CombatManager attacker = source != null ? source.owner : null;

        if (attacker == null || attacker.deck == null)
        {
            Debug.LogError("Attacker o Deck NULL en DamagePerCardInHandEffect");
            return;
        }

        int perCard = Mathf.Max(0, value);
        int handCount = attacker.deck.HandCount;

        if (handCount <= 0 || perCard <= 0)
            return;

        int totalDamage = perCard * handCount;

        enemy.TakeDamage(totalDamage);

        Debug.Log($"{attacker.characterName} inflige {totalDamage} daño ({perCard} x {handCount} cartas en mano) a {enemy.name}");
    }
}