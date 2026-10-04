using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData e) => Debug.Log("Hover ON: " + gameObject.name);
    public void OnPointerExit(PointerEventData e) => Debug.Log("Hover OFF: " + gameObject.name);
}
