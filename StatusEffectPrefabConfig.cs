using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectPrefabMapping
{
    public StatusEffect effect;
    public GameObject prefab;
}

[CreateAssetMenu(menuName = "UI/Status Effect Prefab Config")]
public class StatusEffectPrefabConfig : ScriptableObject
{
    [SerializeField]
    private List<StatusEffectPrefabMapping> mappings = new();

    public GameObject GetPrefabForEffect(StatusEffect effect)
    {
        if (effect == null)
        {
            Debug.LogError("StatusEffect NULL");
            return null;
        }

        // 1) Intentar coincidencia por referencia (preferible)
        foreach (var mapping in mappings)
        {
            if (mapping == null || mapping.effect == null)
                continue;

            if (mapping.effect == effect)
            {
                if (mapping.prefab != null)
                    return mapping.prefab;

                // Si mapping existe pero sin prefab, usar el uiPrefab embebido en el StatusEffect si existe
                if (mapping.effect.uiPrefab != null)
                {
                    Debug.LogWarning($"Mapping para '{effect.effectName}' existe pero no tiene prefab: usando 'uiPrefab' del StatusEffect.");
                    return mapping.effect.uiPrefab;
                }

                Debug.LogError($"Mapping para '{effect.effectName}' no tiene prefab asignado.");
                return null;
            }
        }

        // 2) Intentar coincidencia por GUID (si se generó)
        foreach (var mapping in mappings)
        {
            if (mapping == null || mapping.effect == null)
                continue;

            if (!string.IsNullOrEmpty(mapping.effect.GUID) && !string.IsNullOrEmpty(effect.GUID)
                && mapping.effect.GUID == effect.GUID)
            {
                if (mapping.prefab != null)
                    return mapping.prefab;

                if (mapping.effect.uiPrefab != null)
                {
                    Debug.LogWarning($"Encontrado prefab por GUID para efecto '{effect.effectName}' (usando uiPrefab del StatusEffect).");
                    return mapping.effect.uiPrefab;
                }

                Debug.LogError($"Mapping por GUID para '{effect.effectName}' no tiene prefab asignado.");
                return null;
            }
        }

        // 3) Fallback: usar prefab contenido directamente en el StatusEffect (si el autor lo asignó)
        if (effect.uiPrefab != null)
        {
            Debug.LogWarning($"Usando 'uiPrefab' directamente desde el StatusEffect '{effect.effectName}' (fallback).");
            return effect.uiPrefab;
        }

        // 4) Fallback por nombre (último recurso)
        foreach (var mapping in mappings)
        {
            if (mapping == null || mapping.effect == null)
                continue;

            if (string.Equals(mapping.effect.effectName, effect.effectName, System.StringComparison.OrdinalIgnoreCase))
            {
                if (mapping.prefab != null)
                    return mapping.prefab;

                if (mapping.effect.uiPrefab != null)
                    return mapping.effect.uiPrefab;

                Debug.LogError($"Prefab NULL para efecto (coincidencia por nombre): {effect.effectName}");
                return null;
            }
        }

        Debug.LogError($"No hay prefab asignado para: {effect.effectName}");
        return null;
    }
}