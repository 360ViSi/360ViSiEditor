using UnityEngine;
using TMPro;

public class NodeInspectorTextElement : NodeInspectorElement
{
    [SerializeField] TMP_InputField elementValue;

    public void InitializeElement(string header, ElementKey key, string value)
    {
        elementKey = key;
        elementName.text = header;
        elementValue.text = value;
    }

    public void UpdateValues(string value)
    {
        inspector.UpdateValue(elementKey, value);
    }
}
