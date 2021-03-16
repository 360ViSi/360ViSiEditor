using UnityEngine;

///<summary>
/// Popup panel for changin icon for WorldButton
///</summary>
public class IconSelectionPanel : MonoBehaviour
{
    [SerializeField] NodeInspector nodeInspector;
    [SerializeField] SO_Icons icondb;
    [SerializeField] Transform gridLayout;
    [SerializeField] GameObject iconButtonPrefab;

    ///<summary>
    /// On Button Click
    ///</summary>
    public void SelectIcon(Icon icon)
    {
        nodeInspector.SetIcon(icon.iconName);
        transform.parent.gameObject.SetActive(false);
    }

    private void Start()
    {
        foreach (var icon in icondb.icons)
        {
            var go = Instantiate(iconButtonPrefab, gridLayout);
            var iconButton = go.GetComponent<IconSelectButton>();
            iconButton.Icon = icon;
            iconButton.IconSelectionPanel = this;
        }
    }
}
