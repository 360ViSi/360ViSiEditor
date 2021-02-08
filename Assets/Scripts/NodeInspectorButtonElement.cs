using UnityEngine;
using UnityEngine.UI;
public class NodeInspectorButtonElement : NodeInspectorElement {
    System.Action action;
    public override void InitializeElement(string header, System.Action buttonAction)
    {
        elementName.text = header;
        action = buttonAction;
    }

    public void ButtonClick()
    {
        action.Invoke();
    }
}