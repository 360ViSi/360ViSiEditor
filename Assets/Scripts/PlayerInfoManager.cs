using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager instance;
    [SerializeField] SimulationManager simulationManager;
    [SerializeField] TMPro.TMP_Text infoText;
    [SerializeField] RawImage image;
    [SerializeField] Texture videoTexture;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject infoPanel;
    Tool currentTool;
    ImageSetter imageSetter;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        imageSetter = image.GetComponent<ImageSetter>();
    }
    //Loads text and image/video from values provided by JSON-file
    public void OpenInfoPanel(Tool tool)
    {
        Time.timeScale = 0;
        simulationManager.SetVideoPauseState(true);
        currentTool = tool;
        infoText.text = currentTool.infoText;
        if (currentTool.spritePath == "")
        {
            image.texture = videoTexture;
            videoPlayer.url = currentTool.video2Dpath;
        }
        else
        {
            imageSetter.SetOldLoadedSprite(currentTool.spritePath);
        }

        infoPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        Time.timeScale = 1;
        simulationManager.SetVideoPauseState(false);
        infoPanel.SetActive(false);
    }
}