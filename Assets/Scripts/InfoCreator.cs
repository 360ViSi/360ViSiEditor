using UnityEngine;
using System.Collections;
using TMPro;

public class InfoCreator : MonoBehaviour
{
    [SerializeField] TMP_InputField infoInput;
    private ImageSetter imageSetter;
    private VideoSetter2D videoSetter2D;
    private bool imageVisible = true;
    public Texture videoTexture;

    public bool ImageVisible { get => imageVisible; set => imageVisible = value; }

    private void OnEnable()
    {
        infoInput.text = NodeInspector.instance.CurrentToolNode.InfoText;
        imageSetter = GetComponentInChildren<ImageSetter>();
        videoSetter2D = GetComponentInChildren<VideoSetter2D>();

        if (imageSetter != null && NodeInspector.instance.CurrentToolNode.SpritePath != "")
        {
            imageSetter.SetOldLoadedSprite(NodeInspector.instance.CurrentToolNode.SpritePath);
        }
        else if(NodeInspector.instance.CurrentToolNode.Video2Dpath != null)
        {
            StartCoroutine(SetupVideo());
        }

    }

    public void EditInfo()
    {
        NodeInspector.instance.CurrentToolNode.InfoText = infoInput.text;
        if (imageSetter.image.texture != videoTexture)
        {           
            NodeInspector.instance.CurrentToolNode.SpritePath = imageSetter.SpritePath;
            NodeInspector.instance.CurrentToolNode.Video2Dpath = null;            
        }
        if (imageSetter.image.texture == videoTexture)
        {
            NodeInspector.instance.CurrentToolNode.Video2Dpath = videoSetter2D.VideoPath;
            NodeInspector.instance.CurrentToolNode.SpritePath = "";
            imageSetter.SpritePath = "";
        }
        UndoRedoHandler.instance.SaveState();
    }

    private IEnumerator SetupVideo()
    {
        yield return new WaitForEndOfFrame();
        videoSetter2D.SetOldLoadedVideo(NodeInspector.instance.CurrentToolNode.Video2Dpath);

    }
}