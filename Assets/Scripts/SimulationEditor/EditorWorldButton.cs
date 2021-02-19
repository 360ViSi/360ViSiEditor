using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditorWorldButton : MonoBehaviour
{
    [SerializeField] TMP_Text tmpText;
    [SerializeField] Image image;

    public void Initialize(ActionNode actionNode, Sprite sprite)
    {
        tmpText.text = actionNode.getActionText();
        image.sprite = sprite;
    }
}
