using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerStatusUIManager : MonoBehaviour
{
    [SerializeField] private StatusEffectPrefabConfig prefabConfig;
    [SerializeField] private RectTransform statusContainer;
    [SerializeField] private int maxDisplayedStatuses = 8;
    [SerializeField] private float iconSize = 64f;

    [Header("Stat Prefabs")]
    [SerializeField] private GameObject fuerzaPrefab;
    [SerializeField] private GameObject robustezPrefab;
    [SerializeField] private GameObject magiaPrefab;

    private CombatManager combatManager;

    // Ahora cada estado tiene SU icono
    private Dictionary<ActiveStatus, StatusIconUI> iconDictionary =
        new Dictionary<ActiveStatus, StatusIconUI>();

    private Dictionary<string, StatusIconUI> statIconDictionary =
        new Dictionary<string, StatusIconUI>();

    private void Awake()
    {
        combatManager = GetComponentInParent<CombatManager>();

        if (prefabConfig == null)
            Debug.LogError("StatusEffectPrefabConfig no asignado.");

        if (statusContainer == null)
            Debug.LogError("StatusContainer no asignado.");
    }

    public void UpdateStatuses()
    {
        if (combatManager == null)
            return;

        var activeStatuses = combatManager.activeStatuses;

        //----------------------------------------------------
        // Eliminar iconos cuyos estados ya no existen
        //----------------------------------------------------

        List<ActiveStatus> borrar = new List<ActiveStatus>();

        foreach (var kv in iconDictionary)
        {
            if (!activeStatuses.Contains(kv.Key))
            {
                if (kv.Value != null)
                    Destroy(kv.Value.gameObject);

                borrar.Add(kv.Key);
            }
        }

        foreach (var s in borrar)
            iconDictionary.Remove(s);

        //----------------------------------------------------
        // Crear / actualizar iconos existentes
        //----------------------------------------------------

        int visibles = 0;

        foreach (var status in activeStatuses)
        {
            if (visibles >= maxDisplayedStatuses)
                break;

            if (!iconDictionary.TryGetValue(status, out StatusIconUI icon))
            {
                GameObject prefab =
                    prefabConfig.GetPrefabForEffect(status.effect);

                if (prefab == null)
                {
                    Debug.LogError(
                        $"No existe prefab para {status.effect.effectName}");
                    continue;
                }

                GameObject go =
                    Instantiate(prefab, statusContainer);

                RectTransform rt = go.GetComponent<RectTransform>();

                if (rt != null)
                {
                    rt.sizeDelta = new Vector2(iconSize, iconSize);
                    rt.localScale = Vector3.one;
                }

                icon = go.GetComponent<StatusIconUI>();

                if (icon == null)
                {
                    Debug.LogError(
                        $"El prefab {status.effect.effectName} no tiene StatusIconUI");
                    Destroy(go);
                    continue;
                }

                iconDictionary.Add(status, icon);
            }

            icon.gameObject.SetActive(true);
            icon.SetStatus(status);

            visibles++;
        }

        Canvas.ForceUpdateCanvases();
        UpdateStatIcon("Fuerza", combatManager.GetFuerza());

        UpdateStatIcon("Robustez", combatManager.GetRobustez());

        UpdateStatIcon("Magia", combatManager.GetMagia());
    }
    private void UpdateStatIcon(string statName, int value)
    {
        // Si el stat volvió a 0, eliminar icono
        if (value == 0)
        {
            if (statIconDictionary.TryGetValue(statName, out StatusIconUI oldIcon))
            {
                Destroy(oldIcon.gameObject);
                statIconDictionary.Remove(statName);
            }

            return;
        }

        //----------------------------------------------------
        // Si ya existe, solo actualizar el número
        //----------------------------------------------------

        if (statIconDictionary.TryGetValue(statName, out StatusIconUI icon))
        {
            icon.SetStat(null, statName, value);
            return;
        }

        //----------------------------------------------------
        // Elegir prefab
        //----------------------------------------------------

        GameObject prefab = null;

        switch (statName)
        {
            case "Fuerza":
                prefab = fuerzaPrefab;
                break;

            case "Robustez":
                prefab = robustezPrefab;
                break;

            case "Magia":
                prefab = magiaPrefab;
                break;
        }

        if (prefab == null)
        {
            Debug.LogError($"No hay prefab asignado para {statName}");
            return;
        }

        //----------------------------------------------------
        // Crear icono
        //----------------------------------------------------

        GameObject go = Instantiate(prefab, statusContainer);

        RectTransform rt = go.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.sizeDelta = new Vector2(iconSize, iconSize);
            rt.localScale = Vector3.one;
        }

        StatusIconUI newIcon = go.GetComponent<StatusIconUI>();

        if (newIcon == null)
        {
            Destroy(go);
            return;
        }

        newIcon.SetStat(null, statName, value);

        statIconDictionary.Add(statName, newIcon);
    }
    public void ClearAllIcons()
    {
        foreach (var icon in iconDictionary.Values)
        {
            if (icon != null)
                icon.Clear();
        }
    }

    public void DestroyAllIcons()
    {
        foreach (var icon in iconDictionary.Values)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }

        iconDictionary.Clear();
    }
}