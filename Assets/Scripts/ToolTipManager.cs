using System.Net.Http.Headers;
using TMPro;
using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    string currentText;
    [SerializeField] RectTransform toolTipPanel;
    [SerializeField] TMP_Text toolTipPanelText;
    private void Start()
    {
        toolTipPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!toolTipPanel.gameObject.activeInHierarchy) return;
        var mousePos = Input.mousePosition;
        var xPivot = mousePos.x > Screen.width / 2 ? 1 : -.3f;
        var yPivot = mousePos.y > Screen.height / 2 ? 2 : 0;
        toolTipPanel.pivot = new Vector2(xPivot, yPivot);
        toolTipPanel.position = mousePos;
    }
    //Add event listener for string events
    public void SetToolTip(string toolTipText)
    {
        currentText = toolTipText;
        toolTipPanel.gameObject.SetActive(true);

        toolTipPanelText.text = currentText;
        toolTipPanel.sizeDelta = toolTipPanelText.GetPreferredValues(currentText) + new Vector2(10, 10);
    }
    public void RemoveToolTip(string toolTipText)
    {
        if (currentText != toolTipText) return;

        toolTipPanel.gameObject.SetActive(false);
    }
}
