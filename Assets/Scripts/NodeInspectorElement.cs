using UnityEngine;
using TMPro;
public class NodeInspectorElement : MonoBehaviour
{
    NodeInspector inspector = null;
    ElementKey elementKey;
    [SerializeField] TMP_Text elementName;
    [SerializeField] TMP_InputField elementValue;

    private void OnEnable()
    {
        inspector = GetComponentInParent<NodeInspector>();
    }

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
