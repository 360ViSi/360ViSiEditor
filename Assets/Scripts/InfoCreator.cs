using UnityEngine;
using System.Collections;
using TMPro;

public class InfoCreator : MonoBehaviour
{
    [SerializeField] TMP_InputField infoInput;
    private ImageSetter imageSetter;

    private void OnEnable()
    {
        infoInput.text = NodeInspector.instance.CurrentToolNode.InfoText;
        imageSetter = GetComponentInChildren<ImageSetter>();
        StartCoroutine(SetSprite());
    }

    public void EditInfo()
    {
        NodeInspector.instance.CurrentToolNode.InfoText = infoInput.text;
        NodeInspector.instance.CurrentToolNode.SpriteData = imageSetter.SpriteData;
        UndoRedoHandler.instance.SaveState();
    }

    private IEnumerator SetSprite()
    {
        if (NodeInspector.instance.CurrentToolNode.SpriteData != null)
        {
            yield return new WaitForEndOfFrame();
            imageSetter.SetOldLoadedSprite(NodeInspector.instance.CurrentToolNode.SpriteData);
        }
        yield return null;
    }
}