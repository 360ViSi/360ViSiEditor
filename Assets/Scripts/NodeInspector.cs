using System;
using UnityEngine;

///<summary>
/// Controls and manages the functions of the IN-APP-INSPECTOR (not Unity inspector)
/// used to change values for video- and actionnodes
///</summary>
public class NodeInspector : MonoBehaviour
{
    public static NodeInspector instance;
    VideoNode _currentVideoNode = null;
    ActionNode _currentActionNode = null;
    [SerializeField] GameObject _textElementPrefab = null;
    [SerializeField] GameObject _toggleElementPrefab = null;


    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    ///<summary>
    /// Creates and populates all the fields for the editor in-app-inspector
    ///</summary>
    public void CreateFields(VideoNode node)
    {
        _currentActionNode = null;

        if (node == _currentVideoNode)
            return;

        //Clean old children first
        for (int i = transform.childCount - 1; i > -1; i--)
            Destroy(transform.GetChild(i).gameObject);

        _currentVideoNode = node;
        //Create new ones
        //Video file name
        var textElementObj = Instantiate(_textElementPrefab, transform);
        var textElement = textElementObj.GetComponent<NodeInspectorTextElement>();
        textElement.InitializeElement("Video filename", ElementKey.VideoFileName, node.getVideoFileName());

        //loop toggle
        var toggleElementObj = Instantiate(_toggleElementPrefab, transform);
        var toggleElement = toggleElementObj.GetComponent<NodeInspectorToggleElement>();
        toggleElement.InitializeElement("Loop video", ElementKey.VideoLoop, node.getLoop());
    }

    ///<summary>
    /// Overload for CreateFields that takes in an action node instead
    ///</summary>
    public void CreateFields(ActionNode node)
    {
        _currentVideoNode = null;

        if (node == _currentActionNode)
            return;

        //Clean old children first
        for (int i = transform.childCount - 1; i > -1; i--)
            Destroy(transform.GetChild(i).gameObject);

        _currentActionNode = node;
        //Create new ones
        var elementObj = Instantiate(_textElementPrefab, transform);
        var element = elementObj.GetComponent<NodeInspectorTextElement>();
        element.InitializeElement("Action name", ElementKey.ActionName, node.getActionText());
    }

    public void UpdateValue(ElementKey key, string value)
    {
        if (key == ElementKey.VideoFileName) _currentVideoNode.setVideoFileName(value);
        if (key == ElementKey.ActionName) _currentActionNode.setActionText(value);
    }

    public void UpdateValue(ElementKey key, bool value)
    {
        if (key == ElementKey.VideoLoop) _currentVideoNode.setLoop(value);
    }
}

public enum ElementKey
{
    VideoFileName,
    ActionName,
    VideoLoop
}