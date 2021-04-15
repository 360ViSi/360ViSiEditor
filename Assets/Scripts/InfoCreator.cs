using UnityEngine;
using TMPro;

public class InfoCreator : MonoBehaviour
{
    [SerializeField] TMP_InputField infoInput;

    private void OnEnable()
    {
        infoInput.text = NodeInspector.instance.CurrentToolNode.InfoText;
    }

    public void EditInfo()
    {
        NodeInspector.instance.CurrentToolNode.InfoText = infoInput.text;
    }
}