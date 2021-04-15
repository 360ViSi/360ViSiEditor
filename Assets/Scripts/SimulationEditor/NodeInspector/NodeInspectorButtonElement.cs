using UnityEngine;
using UnityEngine.UI;
public class NodeInspectorButtonElement : NodeInspectorElement {
    System.Action action;
    public override void InitializeElement(string header, System.Action buttonAction)
    {
        elementName.text = header;

        //Sizing according to text lenght - approximated
        int rowCount = header.Length / 25;
        var myRect = GetComponent<RectTransform>();
        myRect.sizeDelta = new Vector2(myRect.sizeDelta.y, 30 + rowCount * 15);
        var childRect = transform.GetChild(0).GetComponent<RectTransform>();
        childRect.sizeDelta = new Vector2(childRect.sizeDelta.y, 30 + rowCount * 15);

        action = buttonAction;
    }

    public void ButtonClick()
    {
        action.Invoke();
    }
}