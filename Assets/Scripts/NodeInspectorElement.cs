using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NodeInspectorElement : MonoBehaviour
{
    public NodeInspector inspector = null;
    public ElementKey elementKey;
    public TMP_Text elementName;
    public void OnEnable() => inspector = GetComponentInParent<NodeInspector>();
}