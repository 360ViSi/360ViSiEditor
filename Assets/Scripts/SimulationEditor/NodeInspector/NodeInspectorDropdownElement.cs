using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NodeInspectorDropdownElement : NodeInspectorElement
{
    [SerializeField] TMP_Dropdown elementValue;
    public override void InitializeElement(string header, ElementKey key, int value)
    {
        Debug.Log("Initialize Dropdown menu");
        elementKey = key;
        elementName.text = header;

        var options = new List<TMP_Dropdown.OptionData>();

        switch (key)
        {
            case ElementKey.ActionType:
                foreach (var item in Utilities.GetValues<ActionType>())
                {
                    var option = new TMP_Dropdown.OptionData
                    {
                        text = item.ToString()
                    };
                    options.Add(option);
                }
                break;
            case ElementKey.ToolType:
                foreach (var item in Utilities.GetValues<ToolType>())
                {
                    var option = new TMP_Dropdown.OptionData
                    {
                        text = item.ToString()
                    };
                    options.Add(option);
                }
                break;
            default:
                Debug.Log("Not implemented");
                break;
        }

        elementValue.AddOptions(options);
        elementValue.value = value;
        elementValue.onValueChanged.AddListener(UpdateValue);
    }

    public void UpdateValue(int value)
    {
        inspector.UpdateValue(elementKey, value);
    }
}