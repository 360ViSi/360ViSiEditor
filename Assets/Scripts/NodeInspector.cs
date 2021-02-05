using System.Collections;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;

///<summary>
/// Controls and manages the functions of the IN-APP-INSPECTOR (not Unity inspector)
/// used to change values for video- and actionnodes
///</summary>
public class NodeInspector : MonoBehaviour
{
    public static NodeInspector instance;
    VideoNode currentVideoNode = null;
    ActionNode currentActionNode = null;
    [SerializeField] EditorVideoPlayer editorVideoPlayer = null;
    [Header("UI Elements")]
    [SerializeField] GameObject textElementPrefab = null;
    [SerializeField] GameObject timeElementPrefab = null;
    [SerializeField] GameObject filenameElementPrefab = null;
    [SerializeField] GameObject toggleElementPrefab = null;


    public VideoNode CurrentVideoNode
    {
        get
        {
            if (currentVideoNode == null)
                Debug.LogError("NodeInspectors' currentVideoNode == null");
            return currentVideoNode;
        }
    }

    public ActionNode CurrentActionNode { get => currentActionNode; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    ///<summary>
    /// Creates and populates all the fields for the editor in-app-inspector
    ///</summary>
    public void CreateFields(VideoNode node, bool isUpdate = false)
    {
        NullCurrentNodes();
        currentVideoNode = node;
        currentVideoNode.GetComponent<Outline>().enabled = true;

        //Put bg to video
        if (isUpdate == false)
            //this is too slow here - needs a frame to function
            editorVideoPlayer.ChangeVideo(node.getVideoFileName());

        //Clean old children first
        for (int i = transform.childCount - 1; i > -1; i--)
            Destroy(transform.GetChild(i).gameObject);

        //Create new ones
        editorVideoPlayer.VideoPlayer.prepareCompleted += CreateVideoFields;
    }

    ///<summary>
    /// Overload for CreateFields that takes in an action node instead
    ///</summary>
    public void CreateFields(ActionNode node, bool isUpdate = false)
    {
        var oldVideoNode = currentVideoNode;

        NullCurrentNodes();
        //Clean old children first
        for (int i = transform.childCount - 1; i > -1; i--)
            Destroy(transform.GetChild(i).gameObject);

        currentActionNode = node;
        currentActionNode.GetComponent<Outline>().enabled = true;
        currentVideoNode = node.GetComponentInParent<VideoNode>();
        if (isUpdate == false && (oldVideoNode == null || currentVideoNode != oldVideoNode))
        {
            editorVideoPlayer.ChangeVideo(currentVideoNode.getVideoFileName());
            currentVideoNode = node.GetComponentInParent<VideoNode>();
            editorVideoPlayer.VideoPlayer.prepareCompleted += CreateActionFields;
        }
        else
        {
            CreateActionFields(editorVideoPlayer.VideoPlayer);
            editorVideoPlayer.RefreshMarkers();
        }
    }

    private void CreateVideoFields(VideoPlayer source)
    {
        editorVideoPlayer.VideoPlayer.prepareCompleted -= CreateVideoFields;
        CreateElement("Video filename",
                      ElementKey.VideoFileName,
                      filenameElementPrefab,
                      currentVideoNode.getVideoFileName());
        CreateElement("Loop video",
                      ElementKey.VideoLoop,
                      toggleElementPrefab,
                      currentVideoNode.getLoop());
        CreateElement("Video loop time",
                      ElementKey.VideoLoopTime,
                      timeElementPrefab,
                      currentVideoNode.getLoopTime(),
                      0);
        CreateElement("Video start time",
                      ElementKey.VideoStartTime,
                      timeElementPrefab,
                      currentVideoNode.getStartTime(),
                      0);
        CreateElement("Video end time",
                      ElementKey.VideoEndTime,
                      timeElementPrefab,
                      currentVideoNode.getEndTime(),
                      1);

    }



    private void CreateActionFields(VideoPlayer source)
    {
        editorVideoPlayer.VideoPlayer.prepareCompleted -= CreateActionFields;
        CreateElement("Action name", ElementKey.ActionName, textElementPrefab, currentActionNode.getActionText());
        CreateElement("Video end action", ElementKey.ActionAutoEnd, toggleElementPrefab, currentActionNode.getAutoEnd());
        if (!currentActionNode.getAutoEnd())
        {
            CreateElement("Action start time",
                          ElementKey.ActionStartTime,
                          timeElementPrefab,
                          currentActionNode.getStartTime(),
                          0);
            CreateElement("Action end time",
                          ElementKey.ActionEndTime,
                          timeElementPrefab,
                          currentActionNode.getEndTime(),
                          1);
        }

    }

    public void UpdateValue(ElementKey key, string value)
    {
        if (key == ElementKey.VideoFileName)
        {
            editorVideoPlayer.ChangeVideo(value);
            currentVideoNode.setVideoFileName(value);
        }
        if (key == ElementKey.ActionName) currentActionNode.setActionText(value);
    }

    public void UpdateValue(ElementKey key, bool value)
    {
        if (key == ElementKey.VideoLoop) currentVideoNode.setLoop(value);
        if (key == ElementKey.ActionAutoEnd)
        {
            currentActionNode.setAutoEnd(value);
            //changing autoend changes what other fields are shown so need to redraw
            CreateFields(currentActionNode, true);
        }
        editorVideoPlayer.RefreshMarkers();
    }

    public void UpdateValue(ElementKey key, float value)
    {
        if (key == ElementKey.VideoStartTime) currentVideoNode.setStartTime(value);
        if (key == ElementKey.VideoEndTime) currentVideoNode.setEndTime(value);
        if (key == ElementKey.VideoLoopTime) currentVideoNode.setLoopTime(value);

        if (key == ElementKey.ActionStartTime) currentActionNode.setStartTime(value);
        if (key == ElementKey.ActionEndTime) currentActionNode.setEndTime(value);

        //These all SO FAR (5) need to refresh the timeline markers, so I'm just going to do it here for all of them
        editorVideoPlayer.RefreshMarkers();
    }

    public float GetVideoLength() => (float)editorVideoPlayer.VideoPlayer.length;


    #region NodeInspectorElementFactory
    void CreateElement(string header, ElementKey key, GameObject prefab, string value)
    {
        var elementObj = Instantiate(prefab, transform);
        var element = elementObj.GetComponent<NodeInspectorElement>();
        element.InitializeElement(
            header,
            key,
            value);
    }
    void CreateElement(string header, ElementKey key, GameObject prefab, float value, float defaultValue) //S TODO defaultValue refactor
    {
        var elementObj = Instantiate(prefab, transform);
        var element = elementObj.GetComponent<NodeInspectorElement>();
        element.InitializeElement(
            header,
            key,
            value,
            defaultValue);
    }
    void CreateElement(string header, ElementKey key, GameObject prefab, bool value)
    {
        var elementObj = Instantiate(prefab, transform);
        var element = elementObj.GetComponent<NodeInspectorElement>();
        element.InitializeElement(
            header,
            key,
            value);
    }

    void NullCurrentNodes()
    {
        if (currentVideoNode != null)
        {
            currentVideoNode.GetComponent<Outline>().enabled = false;
            currentVideoNode = null;
        }
        if (currentActionNode != null)
        {
            currentActionNode.GetComponent<Outline>().enabled = false;
            currentActionNode = null;
        }
    }
    #endregion
}

public enum ElementKey
{
    VideoFileName,
    ActionName,
    ActionStartTime,
    ActionEndTime,
    ActionAutoEnd,
    VideoLoop,
    VideoLoopTime,
    VideoStartTime,
    VideoEndTime
}