using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeInspectorStatsElement : NodeInspectorElement
{
    [SerializeField] TMP_Text elementValue;
    public void InitializeElement(string header, ElementKey key, string value)
    {
        elementKey = key;
        elementName.text = header;
        elementValue.text = value;
    }
}
