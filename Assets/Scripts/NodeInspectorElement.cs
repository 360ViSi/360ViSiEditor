using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class NodeInspectorElement : MonoBehaviour
{
    [HideInInspector] public NodeInspector inspector = null;
    [HideInInspector] public ElementKey elementKey;
    public TMP_Text elementName;
    public void OnEnable() => inspector = GetComponentInParent<NodeInspector>();

    public virtual void InitializeElement(string header, ElementKey key, string value){}
    public virtual void InitializeElement(string header, ElementKey key, float value, float defaultValue){}
    public virtual void InitializeElement(string header, ElementKey key, bool value){}
    public virtual void InitializeElement(string header, ElementKey key, int value){}
}