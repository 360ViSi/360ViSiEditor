using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager instance;
    [SerializeField] SimulationManager simulationManager;
    [SerializeField] TMPro.TMP_Text infoText;
    [SerializeField] Image image;
    [SerializeField] GameObject videoTexture;
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

    public void OpenInfoPanel(Tool tool)
    {
        Time.timeScale = 0;
        simulationManager.SetVideoPauseState(true);
        currentTool = tool;
        infoText.text = currentTool.infoText;
        if (currentTool.spriteData == "")
        {
            image.gameObject.SetActive(false);
            videoTexture.SetActive(true);
            videoPlayer.url = currentTool.video2Dpath;
        }
        else
        {
            image.gameObject.SetActive(true);
            videoTexture.SetActive(false);
            StartCoroutine(SetSprite());
        }
        infoPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        Time.timeScale = 1;
        simulationManager.SetVideoPauseState(false);
        infoPanel.SetActive(false);
    }

    private IEnumerator SetSprite()
    {
        if (currentTool.spriteData != null)
        {
            yield return new WaitForEndOfFrame();
            imageSetter.SetOldLoadedSprite(currentTool.spriteData);
        }
        yield return null;
    }

}