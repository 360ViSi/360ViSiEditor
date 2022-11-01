using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class InfoCreator : MonoBehaviour
{
    [SerializeField] TMP_InputField infoInput;
    private ImageSetter imageSetter;
    private VideoSetter2D videoSetter2D;
    [SerializeField] private Texture videoTexture;
    public Texture VideoTexture { get => videoTexture; set => videoTexture = value; }

    private Image topPanelImage;
    private Image backgroundImage;

    private void Awake()
    {
        imageSetter = GetComponentInChildren<ImageSetter>();
        videoSetter2D = GetComponentInChildren<VideoSetter2D>();

        topPanelImage = this.transform.Find("TopPanel").GetComponent<Image>();
        backgroundImage = this.transform.Find("Panel").GetComponent<Image>();
        
    }
    //Sets image or video and text active depending on what was saved to JSON
    private void OnEnable()
    {
        var currentNode=NodeInspector.instance.CurrentToolNode;
        infoInput.text = currentNode.InfoText;

        if (imageSetter != null && currentNode.SpritePath != "")
        {
            imageSetter.SetOldLoadedSprite(currentNode.SpritePath);
        }
        else if(currentNode.Video2Dpath != null)
        {
            StartCoroutine(SetupVideo());
        }
        backgroundImage.color = currentNode.GetBottomPanelColor();
        topPanelImage.color = currentNode.GetTopPanelColor();

    }
    //Saves image or video path and text to JSON and to UndoRedoHandler
    public void EditInfo()
    {
        NodeInspector.instance.CurrentToolNode.InfoText = infoInput.text;
        if (imageSetter.RawImageInstance.texture != VideoTexture)
        {           
            NodeInspector.instance.CurrentToolNode.SpritePath = imageSetter.SpritePath;
            NodeInspector.instance.CurrentToolNode.Video2Dpath = null;            
        }
        if (imageSetter.RawImageInstance.texture == VideoTexture)
        {
            NodeInspector.instance.CurrentToolNode.Video2Dpath = videoSetter2D.VideoPath;
            NodeInspector.instance.CurrentToolNode.SpritePath = "";
            imageSetter.SpritePath = "";
        }
        UndoRedoHandler.instance.SaveState();
    }

    //Setups video after first frame to make sure all components are loaded before executing
    private IEnumerator SetupVideo()
    {
        yield return new WaitForEndOfFrame();
        videoSetter2D.SetOldLoadedVideo(NodeInspector.instance.CurrentToolNode.Video2Dpath);
    }
}