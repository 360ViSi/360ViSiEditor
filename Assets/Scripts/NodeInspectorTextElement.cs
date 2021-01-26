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

    public void UpdateValueFromFileBrowser(string value)
    {
        elementValue.text = value;
        UpdateValue(value);
    }

    public void UpdateValue(string value)
    {
        inspector.UpdateValue(elementKey, value);
    }
}