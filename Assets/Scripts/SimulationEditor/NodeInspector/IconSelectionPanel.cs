using UnityEngine;

public class IconSelectionPanel : MonoBehaviour
{
    [SerializeField] NodeInspector nodeInspector;
    [SerializeField] SO_Icons icondb;
    [SerializeField] Transform gridLayout;
    [SerializeField] GameObject iconButtonPrefab;
    public void SelectIcon(Icon icon)
    {
        Debug.Log($"Selecting icon {icon.iconName}");
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
