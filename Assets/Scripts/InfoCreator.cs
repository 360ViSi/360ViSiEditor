using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class InfoCreator : MonoBehaviour
{
    [SerializeField] TMP_InputField infoInput;
    private ImageSetter imageSetter;
    private VideoSetter2D videoSetter2D;
    private bool imageVisible = false;

    public bool ImageVisible { get => imageVisible; set => imageVisible = value; }

    private void OnEnable()
    {
        infoInput.text = NodeInspector.instance.CurrentToolNode.InfoText;
        imageSetter = GetComponentInChildren<ImageSetter>();
        videoSetter2D = GetComponentInChildren<VideoSetter2D>();
        if (imageSetter != null && imageVisible == true)
        {
            imageVisible = true;
            GetComponentInChildren<ImageSetter>(true).gameObject.SetActive(true);
            GetComponentInChildren<RawImage>(true).gameObject.SetActive(false);
            StartCoroutine(SetSprite());
        }
        else
        {
            imageVisible = false;
            GetComponentInChildren<ImageSetter>(true).gameObject.SetActive(false);
            GetComponentInChildren<RawImage>(true).gameObject.SetActive(true);
        }     
    }

    public void EditInfo()
    {
        NodeInspector.instance.CurrentToolNode.InfoText = infoInput.text;
        NodeInspector.instance.CurrentToolNode.ImageVisible = imageVisible;
        if (imageSetter != null)
        {
            NodeInspector.instance.CurrentToolNode.SpriteData = imageSetter.SpriteData;
        }
        if (videoSetter2D != null)
        {
            NodeInspector.instance.CurrentToolNode.Video2Dpath = videoSetter2D.FullPath;
        }
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