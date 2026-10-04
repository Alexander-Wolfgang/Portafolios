using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public bool playerTurn = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EndPlayerTurn()
    {
        if (!playerTurn) return;
        Debug.Log("TurnManager: Fin del turno del jugador.");
        playerTurn = false;
        StartCoroutine(EnemyTurnCoroutine());
    }

    IEnumerator EnemyTurnCoroutine()
    {
        Debug.Log("TurnManager: Inicio del turno enemigo.");
        yield return new WaitForSeconds(0.6f);

        var player = Object.FindFirstObjectByType<CombatManager>();
        if (player != null)
        {
            Debug.Log("TurnManager: (simulado) enemigo ataca por 3 daño");
            player.TakeDamage(3, null);
        }

        yield return new WaitForSeconds(0.4f);
        EndEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        Debug.Log("TurnManager: Fin del turno enemigo. Volviendo al jugador.");
        playerTurn = true;

        // Usa el nuevo método compatible
        var allChars = Object.FindObjectsByType<CombatManager>(FindObjectsSortMode.None);
        foreach (var c in allChars)
        {
            //c.StartTurn();
            c.ProcessStartTurnLogic();
        }
    }
}
