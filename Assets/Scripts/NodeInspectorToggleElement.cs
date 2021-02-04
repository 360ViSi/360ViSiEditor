using UnityEngine;
using UnityEngine.UI;

public class NodeInspectorToggleElement : NodeInspectorElement
{
    [SerializeField] Toggle elementValue;

    public override void InitializeElement(string header, ElementKey key, bool value)
    {
        elementKey = key;
        elementName.text = header;
        elementValue.isOn = value;
    }

    public void UpdateValues(bool value)
    {
        inspector.UpdateValue(elementKey, value);
    }
}