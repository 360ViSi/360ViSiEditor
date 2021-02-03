using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

///<summary>
/// Controls and manages the functions of the IN-APP-INSPECTOR (not Unity inspector)
/// used to change values for video- and actionnodes
///</summary>
public class NodeInspector : MonoBehaviour
{
    public static NodeInspector instance;
    VideoNode _currentVideoNode = null;
    ActionNode _currentActionNode = null;
    [SerializeField] EditorVideoPlayer _editorVideoPlayer = null;
    [Header("UI Elements")]
    [SerializeField] GameObject _textElementPrefab = null;
    [SerializeField] GameObject _statsElementPrefab = null;
    [SerializeField] GameObject _filenameElementPrefab = null;
    [SerializeField] GameObject _toggleElementPrefab = null;

    public VideoNode CurrentVideoNode
    {
        get
        {
            if (_currentVideoNode == null)
                Debug.LogError("NodeInspectors' currentVideoNode == null");
            return _currentVideoNode;
        }
    }

    public ActionNode CurrentActionNode { get => _currentActionNode; }

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
        NullCurrentNodes();

        _currentVideoNode = node;
        _currentVideoNode.GetComponent<Outline>().enabled = true;

        //Clean old children first
        for (int i = transform.childCount - 1; i > -1; i--)
            Destroy(transform.GetChild(i).gameObject);

        //Put bg to video
        _editorVideoPlayer.ChangeVideo(node.getVideoFileName());
        Debug.Log(node.getEndTime());
        Debug.Log((float)GetVideoLenght());
        Debug.Log(Utilities.FloatToTime(node.getEndTime(), (float)GetVideoLenght()));
        //Create new ones
        //Video file name
        var textElementObj = Instantiate(_filenameElementPrefab, transform);
        var textElement = textElementObj.GetComponent<NodeInspectorTextElement>();
        textElement.InitializeElement(
            "Video filename",
            ElementKey.VideoFileName,
            node.getVideoFileName());

        //loop toggle
        var toggleElementObj = Instantiate(_toggleElementPrefab, transform);
        var toggleElement = toggleElementObj.GetComponent<NodeInspectorToggleElement>();
        toggleElement.InitializeElement(
            "Loop video",
            ElementKey.VideoLoop,
            node.getLoop());

        //Video start time
        var startTimeTextElementObj = Instantiate(_statsElementPrefab, transform);
        var startTimeTextElement = startTimeTextElementObj.GetComponent<NodeInspectorStatsElement>();
        startTimeTextElement.InitializeElement(
            "Video start time ",
            ElementKey.VideoStartTime,
            Utilities.FloatToTime(node.getStartTime(), (float)GetVideoLenght()));

        //Video end time
        var endTimeTextElementObj = Instantiate(_statsElementPrefab, transform);
        var endTimeTextElement = endTimeTextElementObj.GetComponent<NodeInspectorStatsElement>();
        endTimeTextElement.InitializeElement(
            "Video end time ",
            ElementKey.VideoEndTime,
            Utilities.FloatToTime(node.getEndTime(), (float)GetVideoLenght()));
    }


    ///<summary>
    /// Overload for CreateFields that takes in an action node instead
    ///</summary>
    public void CreateFields(ActionNode node)
    {
        NullCurrentNodes();

        _currentActionNode = node;
        _currentVideoNode = node.GetComponentInParent<VideoNode>();
        _editorVideoPlayer.ChangeVideo(_currentVideoNode.getVideoFileName());
        _currentActionNode.GetComponent<Outline>().enabled = true;

        //Clean old children first
        for (int i = transform.childCount - 1; i > -1; i--)
            Destroy(transform.GetChild(i).gameObject);

        //Create new ones
        var elementObj = Instantiate(_textElementPrefab, transform);
        var element = elementObj.GetComponent<NodeInspectorTextElement>();
        element.InitializeElement("Action name", ElementKey.ActionName, node.getActionText());
    }

    void NullCurrentNodes()
    {
        if (_currentVideoNode != null)
        {
            _currentVideoNode.GetComponent<Outline>().enabled = false;
            _currentVideoNode = null;
        }
        if (_currentActionNode != null)
        {
            _currentActionNode.GetComponent<Outline>().enabled = false;
            _currentActionNode = null;
        }
    }

    public void UpdateValue(ElementKey key, string value)
    {
        if (key == ElementKey.VideoFileName)
        {
            _editorVideoPlayer.ChangeVideo(value);
            _currentVideoNode.setVideoFileName(value);
        }
        if (key == ElementKey.ActionName) _currentActionNode.setActionText(value);
    }

    public void UpdateValue(ElementKey key, bool value)
    {
        if (key == ElementKey.VideoLoop) _currentVideoNode.setLoop(value);
    }

    public void UpdateValue(ElementKey key, float value)
    {
        Debug.Log(key.ToString() + " - " + value);
        if (key == ElementKey.VideoStartTime) _currentVideoNode.setStartTime(value);
        if (key == ElementKey.VideoEndTime) _currentVideoNode.setEndTime(value);
    }

    public double GetVideoLenght() => _editorVideoPlayer.VideoPlayer.length;
}

public enum ElementKey
{
    VideoFileName,
    ActionName,
    ActionStartTime,
    ActionEndTime,
    VideoLoop,
    VideoStartTime,
    VideoEndTime
}