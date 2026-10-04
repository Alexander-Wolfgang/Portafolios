using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatusUIManager : MonoBehaviour
{
    [SerializeField] private StatusEffectPrefabConfig prefabConfig;
    [SerializeField] private RectTransform statusContainer;
    [SerializeField] private int maxDisplayedStatuses = 8;
    [SerializeField] private float iconSize = 64f;

    private List<StatusIconUI> activeIconsUI = new List<StatusIconUI>();
    private Enemy enemy;
    private GridLayoutGroup layoutGroup;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        layoutGroup = statusContainer?.GetComponent<GridLayoutGroup>();

        if (enemy == null)
            Debug.LogError("Enemy no encontrado en EnemyStatusUIManager");

        if (prefabConfig == null)
            Debug.LogError("StatusEffectPrefabConfig no asignado en EnemyStatusUIManager");

        if (statusContainer == null)
            Debug.LogWarning("statusContainer no asignado en EnemyStatusUIManager");
    }
    public void UpdateStatuses()
    {
        if (enemy == null || statusContainer == null || prefabConfig == null)
            return;

        var activeStatuses = enemy.activeStatuses;

        CleanupOldIcons();

        // Crear / actualizar iconos
        for (int i = 0; i < activeStatuses.Count && i < maxDisplayedStatuses; i++)
        {
            StatusIconUI iconUI = GetOrCreateIconUI(activeStatuses[i], i);
            if (iconUI != null)
            {
                iconUI.gameObject.SetActive(true);
                iconUI.SetStatus(activeStatuses[i]);
            }
        }

        // Desactivar sobrantes
        for (int i = activeStatuses.Count; i < activeIconsUI.Count; i++)
        {
            if (activeIconsUI[i] != null)
                activeIconsUI[i].gameObject.SetActive(false);
        }

        Canvas.ForceUpdateCanvases();
    }

    private StatusIconUI GetOrCreateIconUI(ActiveStatus status, int index)
    {
        if (status == null)
        {
            Debug.LogError("ActiveStatus es NULL en GetOrCreateIconUI");
            return null;
        }

        if (index < activeIconsUI.Count && activeIconsUI[index] != null)
        {
            var iconUI = activeIconsUI[index];
            if (!iconUI.gameObject.activeSelf)
                iconUI.gameObject.SetActive(true);
            return iconUI;
        }

        if (status.effect == null)
        {
            Debug.LogError($"ActiveStatus.effect es NULL en índice {index} (no se puede crear icono)");
            return null;
        }

        GameObject prefab = prefabConfig.GetPrefabForEffect(status.effect);

        if (prefab == null)
        {
            string effectName = status.effect != null ? status.effect.effectName : "<NULL>";
            Debug.LogError($"No hay prefab para el efecto: {effectName}");
            return null;
        }

        GameObject go = Instantiate(prefab, statusContainer);
        if (prefab == null)
        {
            Debug.LogError("PREFAB ES NULL");
            return null;
        }
        RectTransform rt = go.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.sizeDelta = new Vector2(iconSize, iconSize);
            rt.localScale = Vector3.one;
        }

        StatusIconUI newIconUI = go.GetComponent<StatusIconUI>();

        if (newIconUI == null)
        {
            Debug.LogError($"Prefab de {status.effect.effectName} no contiene StatusIconUI");
            Destroy(go);
            return null;
        }

        if (index >= activeIconsUI.Count)
            activeIconsUI.Add(newIconUI);
        else
            activeIconsUI[index] = newIconUI;

        return newIconUI;
    }

    private void CleanupOldIcons()
    {
        activeIconsUI.RemoveAll(icon => icon == null);
    }

    public void ClearAllIcons()
    {
        foreach (var iconUI in activeIconsUI)
        {
            if (iconUI != null)
                iconUI.Clear();
        }
    }

    public void DestroyAllIcons()
    {
        foreach (var iconUI in activeIconsUI)
        {
            if (iconUI != null)
                Destroy(iconUI.gameObject);
        }
        activeIconsUI.Clear();
    }
}