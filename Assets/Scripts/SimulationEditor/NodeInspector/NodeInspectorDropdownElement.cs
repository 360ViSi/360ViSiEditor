using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NodeInspectorDropdownElement : NodeInspectorElement
{
    [SerializeField] TMP_Dropdown elementValue;
    public override void InitializeElement(string header, ElementKey key, int value)
    {
        elementKey = key;
        elementName.text = header;

        var options = new List<TMP_Dropdown.OptionData>();
        if(key == ElementKey.ActionType)
        {
            foreach (var item in Utilities.GetValues<ActionType>())
            {
                var option = new TMP_Dropdown.OptionData
                {
                    text = item.ToString()
                };
                options.Add(option);
            }
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