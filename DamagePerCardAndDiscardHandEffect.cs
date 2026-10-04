using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/DamagePerCardAndDiscardHandEffect")]
public class DamagePerCardAndDiscardHandEffect : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null)
        {
            Debug.LogError("Target NULL en DamagePerCardAndDiscardHandEffect");
            return;
        }

        CombatManager attacker = source != null ? source.owner : null;

        if (attacker == null)
        {
            Debug.LogError("Attacker NULL en DamagePerCardAndDiscardHandEffect");
            return;
        }

        if (attacker.deck == null)
        {
            Debug.LogError("Deck NULL en DamagePerCardAndDiscardHandEffect");
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

        // 🔥 aplicar daño
        target.TakeDamage(totalDamage, attacker);

        // 🔥 consumir TODA la mano
        attacker.deck.DiscardAllFromHand(source);

        Debug.Log($"{attacker.characterName} inflige {totalDamage} daño ({perCard} x {handCount}) y descarta toda su mano");
    }

    public override void ApplyToEnemy(CardRuntime source, Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy NULL en DamagePerCardAndDiscardHandEffect");
            return;
        }

        CombatManager attacker = source != null ? source.owner : null;

        if (attacker == null || attacker.deck == null)
        {
            Debug.LogError("Attacker o Deck NULL en DamagePerCardAndDiscardHandEffect");
            return;
        }

        int perCard = Mathf.Max(0, value);
        int handCount = attacker.deck.HandCount;

        if (handCount <= 0 || perCard <= 0)
            return;

        int totalDamage = perCard * handCount;

        enemy.TakeDamage(totalDamage);

        attacker.deck.DiscardAllFromHand(source);

        Debug.Log($"{attacker.characterName} inflige {totalDamage} daño y descarta toda su mano");
    }
}