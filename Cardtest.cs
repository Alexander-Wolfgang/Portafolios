using UnityEngine;

public class CardTest : MonoBehaviour
{
    public CombatManager player;
    public CombatManager enemy;
    public CardData shieldShort;
    public CardData shieldLong;
    public CardData attack;

    private void Start()
    {
        Debug.Log("Turno 1 - El jugador se aplica dos escudos");

        // Aplica el efecto directamente en el jugador
        foreach (Effect effect in shieldShort.effects)
            effect.Apply(null, player);

        foreach (Effect effect in shieldLong.effects)
            effect.Apply(null, player);

        Debug.Log($"Escudo total: {player.TotalShield}");

        foreach (Effect effect in attack.effects)
            effect.Apply(null, player);

        Debug.Log($"Escudo restante: {player.TotalShield} | Vida: {player.currentHealth}");

        Debug.Log("=== Comienza el Turno 2 ===");
        player.ProcessStartTurnLogic();

        Debug.Log($"Escudo restante tras StartTurn: {player.TotalShield} | Vida: {player.currentHealth}");
    }
}
