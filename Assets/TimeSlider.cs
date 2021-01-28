using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TimeSlider : Slider
{
    public UnityEvent OnHold;
    public UnityEvent OnRelease;
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnHold?.Invoke();
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnRelease?.Invoke();
    }
}