using UnityEngine;
using UnityEngine.EventSystems;

public class WorldButtonImage: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{
    WorldButton button;

    public WorldButton Button { get => button; set => button = value; }

    public void OnPointerClick(PointerEventData eventData) => button.Activate();
    public void OnPointerExit(PointerEventData eventData) => button.MouseOver = false;
    public void OnPointerEnter(PointerEventData eventData) => button.MouseOver = true;
} 
