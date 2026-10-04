using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/MultiplierShield")]
public class MultiplierShield : Effect
{
    public override void Apply(CardRuntime source, CombatManager target)
    {
        if (target == null) return;

        // 🔥 1. Obtener escudo total actual
        int totalShield = target.TotalShield;

        if (totalShield <= 0)
        {
            Debug.Log("No hay escudo para convertir");
            return;
        }

        // 🔥 2. Calcular porcentaje basado en value
        float percentage = value / 100f;
        int newShield = Mathf.FloorToInt(totalShield * percentage);

        if (newShield <= 0)
        {
            Debug.Log("El escudo resultante es 0");
            return;
        }

        // 🔥 3. Aplicar nuevo escudo de duración 1 turno
        target.AddShield(newShield, 1);

        Debug.Log($"{target.characterName} convierte {totalShield} escudo en {newShield} escudo temporal (1 turno)");
    }
}