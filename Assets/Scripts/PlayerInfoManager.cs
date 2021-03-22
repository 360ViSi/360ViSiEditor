using UnityEngine;

public class PlayerInfoManager : MonoBehaviour {
    public static PlayerInfoManager instance;
    [SerializeField] TMPro.TMP_Text infoText;
    [SerializeField] GameObject infoPanel;
    Tool currentTool;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void OpenInfoPanel(Tool tool)
    {
        currentTool = tool;
        infoText.text = currentTool.infoText;
        infoPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        infoPanel.SetActive(false);
    }


}