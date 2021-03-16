using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionDraggable : MonoBehaviour
{
    public TimelineDraggable start;
    public TimelineDraggable end;
    ///<summary>
    /// Text that appears on the timeline
    ///</summary>
    public TMP_Text actionText;
    ///<summary>
    /// Image that appaers on the left side of the start draggable
    ///</summary>
    public Image actionImage;
}

