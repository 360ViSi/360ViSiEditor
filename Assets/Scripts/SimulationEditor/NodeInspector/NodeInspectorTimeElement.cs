using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeInspectorTimeElement : NodeInspectorElement
{
    [SerializeField] TMP_Text elementValue;
    [SerializeField] GameObject resetButton;
    float resetValue;
    public override void InitializeElement(string header, ElementKey key, float value, float defaultValue)
    {
        resetButton.SetActive(value != defaultValue);
        
        elementKey = key;
        elementName.text = header;
        elementValue.text = Utilities.FloatToTime(value, inspector.GetVideoLength(), false);
        resetValue = defaultValue;
    }

    public void ResetToDefaultValue()
    {
        elementValue.text = Utilities.FloatToTime(resetValue, inspector.GetVideoLength(), false);
        inspector.UpdateValue(elementKey, resetValue);
        resetButton.SetActive(false);
    }
}
