using UnityEngine;

[CreateAssetMenu(menuName = "Cards/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public string effectName;
    public string description;   // para mostrar en UI
    public Sprite icon;          // opcional, para la UI
    [Tooltip("Si vacío, afecta a todos los tipos. Si 'Knife', solo afecta ataques tipo 'Knife'")]
    public string affectedType;
    public bool revertOnExpire;
    public int statType;
    public StatusCategory category;
    public enum StatusCategory
    {
        Buff,
        Debuff,
        Special //idealmente significa ambos dependiendo de la situacion, tambien se puede usar para casos especiales
    }

    // Prefab opcional asignable directamente en el StatusEffect
    [Tooltip("Prefab de UI que representa este efecto. Usado como fallback si no hay mapping en StatusEffectPrefabConfig.")]
    public GameObject uiPrefab;

    // Identificador persistente para evitar dependencia exclusiva de referencia en memoria
    [HideInInspector, SerializeField]
    private string guid;

    public string GUID => guid;

#if UNITY_EDITOR
    // Se ejecuta en editor para garantizar que cada asset tenga GUID persistente
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString();
    }
#endif
}