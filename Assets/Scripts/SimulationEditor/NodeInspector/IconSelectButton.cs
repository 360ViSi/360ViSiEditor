using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



///<summary>
/// Button in Icon Selection Panel, sends click event to through the panel to the NodeInspector
///</summary>
[RequireComponent(typeof(Button))]
public class IconSelectButton : MonoBehaviour
{
    IconSelectionPanel iconSelectionPanel;
    Icon icon;
    public Icon Icon
    {
        get => icon; set
        {
            icon = value;
            transform.GetChild(0).GetComponent<Image>().sprite = icon.sprite;
        }
    }
    public IconSelectionPanel IconSelectionPanel { get => iconSelectionPanel; set => iconSelectionPanel = value; }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => iconSelectionPanel.SelectIcon(icon));
    }
}
