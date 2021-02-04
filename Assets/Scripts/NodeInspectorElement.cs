using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class NodeInspectorElement : MonoBehaviour
{
    [HideInInspector] public NodeInspector inspector = null;
    [HideInInspector] public ElementKey elementKey;
    public TMP_Text elementName;
    public void OnEnable() => inspector = GetComponentInParent<NodeInspector>();

    public virtual void InitializeElement(string header, ElementKey key, string value){}
    public virtual void InitializeElement(string header, ElementKey key, float value, float defaultValue){}
    public virtual void InitializeElement(string header, ElementKey key, bool value){}
}