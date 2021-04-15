using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string toolTipText = "";
    [SerializeField] GameEventString toolTipEnter;
    [SerializeField] GameEventString toolTipExit;

    public void OnPointerEnter(PointerEventData eventData)
        => toolTipEnter?.Raise(toolTipText);

    public void OnPointerExit(PointerEventData eventData) 
        => toolTipExit?.Raise(toolTipText);

    private void OnDestroy() => toolTipExit?.Raise(toolTipText);
}
