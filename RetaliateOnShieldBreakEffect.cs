using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Retaliate On Shield Break")]
public class RetaliateOnShieldBreakEffect : Effect
{
    [Tooltip("Daño que inflige al atacante si rompe el escudo este turno.")]
    public int retaliateDamage = 10;

    // NOTE: No re-declaramos `duration` aquí: usamos `duration` heredado de Effect.

    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null) return;

        // Handler que será llamado cuando el Character dispare OnShieldBrokenThisTurn
        System.Action<CombatManager, CombatManager> handler = null;
        handler = (self, attacker) =>
        {
            // Solo activamos si hay un atacante válido
            if (attacker != null)
            {
                Debug.Log($"{self.characterName} contraataca a {attacker.characterName} por {retaliateDamage} al romper su escudo.");
                attacker.TakeDamage(retaliateDamage, null);
            }
        };

        // Suscribimos el handler
        target.OnShieldBrokenThisTurn += handler;

        // Añadimos un ActiveStatus para que el sistema de turnos lo gestione (y expire en `duration` turnos).
        // Usamos un StatusEffect nulo porque solo necesitamos que StartTurn() lo decremente y lo elimine.
        target.activeStatuses.Add(new ActiveStatus(null, retaliateDamage, duration));

        // Cuando el ActiveStatus expire (StartTurn lo eliminará), queremos también desuscribir el handler.
        // Dado que ActiveStatus no tiene enlace al handler, lo más simple es desuscribir en StartTurn() del Character
        // cuando elimine ese ActiveStatus — para esto deberás ampliar Character.StartTurn() para desuscribir
        // handlers asociados cuando detecte un ActiveStatus con effect == null y value == retaliateDamage (o mejor: con una marca).
        // Si prefieres, puedo darte la modificación de Character.StartTurn() para hacer esa limpieza automáticamente.

        Debug.Log($"{target.characterName} gana efecto 'contraataque al romper escudo' por {duration} turno(s). Daño: {retaliateDamage}");
    }
}
