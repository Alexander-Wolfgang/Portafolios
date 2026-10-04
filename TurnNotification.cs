using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnNotification : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animación")]
    [SerializeField] private float appearOffsetX = 400f;     // px, empieza a la derecha (ajusta en Inspector)
    [SerializeField] private float disappearOffsetX = -80f;  // px, termina un poco a la izquierda
    [SerializeField] private float stayDuration = 0.25f;     // se queda 0.25s en el centro

    void Reset()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    // message ignorado (la imagen ya contiene texto)
    public IEnumerator Play(string message, float totalDuration, float fadeTime)
    {
        var graphics = GetComponentsInChildren<Graphic>(true);

        if (canvasGroup == null)
            canvasGroup = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;

        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null)
            rt = gameObject.AddComponent<RectTransform>();

        // parámetros defensivos
        fadeTime = Mathf.Clamp(fadeTime, 0.01f, totalDuration * 0.5f);
        float effectiveStay = Mathf.Min(stayDuration, Mathf.Max(0f, totalDuration - 2f * fadeTime));
        float inDuration = fadeTime;
        float outDuration = fadeTime;

        // posiciones en local (evitar conflictos con LayoutGroups)
        Vector3 centerPos = Vector3.zero;
        Vector3 startPos = centerPos + new Vector3(appearOffsetX, 0f, 0f);
        Vector3 endPos = centerPos + new Vector3(disappearOffsetX, 0f, 0f);

        // inicial
        SetAlpha(0f, graphics, canvasGroup);
        rt.localPosition = startPos;

        Debug.Log($"[TurnNotif] START pos={rt.localPosition} scale={rt.localScale}");

        // FADE IN + MOVE
        float t = 0f;
        while (t < inDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / inDuration);
            float alpha = Mathf.SmoothStep(0f, 1f, p);

            SetAlpha(alpha, graphics, canvasGroup);
            rt.localPosition = Vector3.Lerp(startPos, centerPos, p);
            yield return null;
        }

        SetAlpha(1f, graphics, canvasGroup);
        rt.localPosition = centerPos;

        Debug.Log($"[TurnNotif] AT_CENTER pos={rt.localPosition}");

        if (effectiveStay > 0f)
            yield return new WaitForSeconds(effectiveStay);

        // FADE OUT + MOVE
        t = 0f;
        while (t < outDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / outDuration);
            float alpha = Mathf.SmoothStep(1f, 0f, p);

            SetAlpha(alpha, graphics, canvasGroup);
            rt.localPosition = Vector3.Lerp(centerPos, endPos, p);
            yield return null;
        }

        SetAlpha(0f, graphics, canvasGroup);
        rt.localPosition = endPos;

        Debug.Log($"[TurnNotif] END pos={rt.localPosition}");
        yield break;
    }
    void SetAlpha(float a, Graphic[] graphics, CanvasGroup cg)
    {
        if (graphics != null && graphics.Length > 0)
        {
            for (int i = 0; i < graphics.Length; i++)
            {
                var g = graphics[i];
                if (g == null) continue;
                Color c = g.color;
                c.a = a;
                g.color = c;
            }
        }

        if (cg != null)
            cg.alpha = a;
    }
}
