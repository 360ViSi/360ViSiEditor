using System.Globalization;
using System.Net;
using System.Linq;
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
    ToolNode currentToolNode = null;
    [SerializeField] SO_Icons icons;
    [SerializeField] Transform videoCamTransform = null;
    [SerializeField] EditorVideoPlayer editorVideoPlayer = null;
    [SerializeField] EditorVideoControls editorVideoControls = null;
    [SerializeField] GameObject iconSelectionPanel = null;
    [SerializeField] GameObject questionCreatorPanel = null;
    [SerializeField] GameObject infoCreatorPanel = null;
    [SerializeField] ActionDraggables actionDraggables = null;
    bool editingAreaMarker = false;

    [Header("UI Elements")]
    [SerializeField] GameObject textElementPrefab = null;
    [SerializeField] GameObject timeElementPrefab = null;
    [SerializeField] GameObject filenameElementPrefab = null;
    [SerializeField] GameObject toggleElementPrefab = null;
    [SerializeField] GameObject dropdownElementPrefab = null;
    [SerializeField] GameObject buttonElementPrefab = null;
    [Header("World Elements")]
    [SerializeField] GameObject worldButtonPrefab = null;
    [SerializeField] GameObject floorButtonPrefab = null;
    [SerializeField] GameObject areaButtonPrefab = null;
    List<GameObject> currentWorldMarkers = new List<GameObject>();

    public VideoNode CurrentVideoNode => currentVideoNode;
    public ActionNode CurrentActionNode => currentActionNode;
    public ToolNode CurrentToolNode => currentToolNode;
    public ActionDraggables ActionDraggables => actionDraggables;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DestroyAllInspectorElements();
    }

    

    void DestroyAllInspectorElements()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    ///<summary>
    /// Creates and populates all the fields for the editor in-app-inspector
    ///</summary>
    public void CreateFields(VideoNode node, bool isUpdate = false)
    {
        NullCurrentNodes();
        currentVideoNode = node;
        actionDraggables.CreateActionDraggables(node);
        currentVideoNode.GetComponent<Outline>().enabled = true;

        videoCamTransform.localEulerAngles = currentVideoNode.getVideoStartRotation();
        DestroyAllInspectorElements();

        //Put bg to video
        if (!isUpdate)
        {
            var canUseVideo = editorVideoPlayer.TryChangeVideo(node.getVideoFileName());
            if (canUseVideo)
                editorVideoPlayer.VideoPlayer.prepareCompleted += CreateVideoFields;
        }

        //Create new ones
        CreateElement("Video filename",
                      ElementKey.VideoFileName,
                      filenameElementPrefab,
                      currentVideoNode.getVideoFileName());

        if (isUpdate)
            CreateVideoFields(editorVideoPlayer.VideoPlayer);
    }

    ///<summary>
    /// Overload for CreateFields that takes in an action node instead
    ///</summary>
    public void CreateFields(ActionNode node, bool isUpdate = false)
    {
        var oldVideoNode = currentVideoNode;

        NullCurrentNodes();
        DestroyAllInspectorElements();

        currentActionNode = node;
        currentActionNode.GetComponent<Outline>().enabled = true;
        currentVideoNode = node.GetComponentInParent<VideoNode>();
        actionDraggables.CreateActionDraggables(currentVideoNode);

        videoCamTransform.localEulerAngles = currentVideoNode.getVideoStartRotation();

        //Clicking on action that isnt from the same video as the previous one (or none)
        if (isUpdate == false && (oldVideoNode == null || currentVideoNode != oldVideoNode))
        {
            editorVideoPlayer.TryChangeVideo(currentVideoNode.getVideoFileName());
            editorVideoPlayer.VideoPlayer.prepareCompleted += CreateActionFields;
        }
        else
        {
            CreateActionFields(editorVideoPlayer.VideoPlayer);
            editorVideoPlayer.RefreshTimeline();
        }
    }

    ///<summary>
    /// Overload for CreateFields that takes in a ToolNode instead
    ///</summary>
    public void CreateFields(ToolNode node, bool isUpdate = false)
    {
        editorVideoPlayer.VideoPlayer.Stop();
        NullCurrentNodes();
        DestroyAllInspectorElements();
        currentToolNode = node;

        currentToolNode.GetComponent<Outline>().enabled = true;

        CreateElement("Tool", ElementKey.ToolType, dropdownElementPrefab, (int)node.ToolType);
        if (node.ToolType == ToolType.QuestionTask)
        {
            if (node.Question == null)
                CreateElement("Create Question", buttonElementPrefab, OpenQuestionCreator);
            else
                CreateElement("Edit Question", buttonElementPrefab, OpenQuestionCreator);
        }
        else if (node.ToolType == ToolType.Info)
        {
            if (string.IsNullOrEmpty(node.InfoText))
                CreateElement("Create Info Text", buttonElementPrefab, OpenInfoCreator);
            else
                CreateElement("Edit Info Text", buttonElementPrefab, OpenInfoCreator);
        }
    }



    private void CreateVideoFields(VideoPlayer source)
    {
        editorVideoPlayer.VideoPlayer.prepareCompleted -= CreateVideoFields;
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
        CreateElement("Set current rotation as starting rotation",
        buttonElementPrefab, SetCurrentRotationAsStartRotation);

        CreateWorldMarkers();
    }



    private void CreateActionFields(VideoPlayer source)
    {
        editorVideoPlayer.VideoPlayer.prepareCompleted -= CreateActionFields;
        CreateElement("Action name", ElementKey.ActionName, textElementPrefab, currentActionNode.getActionText());
        CreateElement("Change Icon", buttonElementPrefab, OpenIconSelection);

        CreateElement("Video end action", ElementKey.ActionAutoEnd, toggleElementPrefab, currentActionNode.getAutoEnd());

        if (currentActionNode.getAutoEnd())
        {
            CreateWorldMarker(currentActionNode, true);
            return;
        }

        CreateElement("Action type", ElementKey.ActionType, dropdownElementPrefab, (int)currentActionNode.getActionType());

        if (currentActionNode.getActionType() == ActionType.Timer)
        {
            CreateElement("Time to trigger", ElementKey.Timer, textElementPrefab, currentActionNode.getActionTimer().ToString("0.0"));
            return;
        }

        if (currentActionNode.getActionType() != ActionType.ScreenButton)
            CreateElement("Set Marker", buttonElementPrefab, StartWorldMarkerPositioning);
        if (currentActionNode.getActionType() == ActionType.AreaButton && currentActionNode.getAreaMarkerVertices() != null)
            CreateElement("Edit Marker", buttonElementPrefab, EditAreaMarkerPositioning);

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
        CreateWorldMarker(currentActionNode, true);
    }

    ///<summary>
    /// Spawns markers for all the actions in the current video
    ///</summary>
    void CreateWorldMarkers()
    {
        foreach (var item in currentVideoNode.getActionNodeList())
            CreateWorldMarker(item);
    }

    ///<summary>
    /// Spawns a marker in the world depending on the ActionType.
    /// Called from EditorVideoControls while setting a new position
    ///</summary>
    public void CreateWorldMarker(ActionNode node, bool removeOld = false)
    {
        if (removeOld)
            RemoveMarkers();

        //Instantiate specific prefab
        GameObject go = null;
        var type = node.getActionType();
        switch (type)
        {
            case ActionType.WorldButton:
                go = Instantiate(worldButtonPrefab);
                if (go.TryGetComponent(out EditorWorldButton editorWorldButton))
                    editorWorldButton.Initialize(node, icons.GetIconSprite(node.getIconName()));
                break;
            case ActionType.FloorButton:
                go = Instantiate(floorButtonPrefab);
                break;
            case ActionType.AreaButton:
                go = Instantiate(areaButtonPrefab);
                var vertices = node.getAreaMarkerVertices();
                go.GetComponent<EditorAreaButton>().Initialize(this, videoCamTransform.GetComponent<Camera>(), editingAreaMarker, vertices);
                break;
            default:
                Destroy(go);
                return;
        }

        go.layer = 9;

        go.transform.position = node.getWorldPosition();
        var oldRotation = go.transform.rotation;
        go.transform.LookAt(videoCamTransform);

        if (node.getActionType() == ActionType.FloorButton)
        {
            go.transform.localEulerAngles = new Vector3(oldRotation.x, 0, oldRotation.z);
            go.transform.localScale = new Vector3(10, .5f, 10);
        }

        currentWorldMarkers.Add(go);
    }


    public void UpdateValue(ElementKey key, string value)
    {
        if (key == ElementKey.VideoFileName)
        {
            editorVideoPlayer.TryChangeVideo(value);
            currentVideoNode.setVideoFileName(value);
        }
        if (key == ElementKey.ActionName)
        {
            currentActionNode.setActionText(value);
            CreateWorldMarker(currentActionNode, true);
        }
        if (key == ElementKey.Timer)
        {
            if (float.TryParse(value, out float result))
                currentActionNode.setActionTimer(result);
            else Debug.LogError("Time is in the wrong format!");
        }
    }

    public void UpdateValue(ElementKey key, bool value)
    {
        if (key == ElementKey.VideoLoop)
        {
            currentVideoNode.setLoop(value);
            //If video will loop, none of the actions can autoend
            if (value)
            {
                foreach (var item in currentVideoNode.getActionNodeList())
                    item.setAutoEnd(false);
            }
        }

        if (key == ElementKey.ActionAutoEnd)
        {
            currentActionNode.setAutoEnd(value);
            //If set one action as autoend, remove that from others
            //And set video to NOT loop (would override autoend)
            if (value)
            {
                foreach (var item in currentVideoNode.getActionNodeList())
                {
                    if (item != currentActionNode)
                        item.setAutoEnd(false);
                }

                currentVideoNode.setLoop(false);
            }
            //changing autoend changes what other fields are shown so need to redraw
            CreateFields(currentActionNode, true);
        }
        editorVideoPlayer.RefreshTimeline();
    }

    public void UpdateValue(ElementKey key, int value)
    {
        if (key == ElementKey.ToolType)
        {
            currentToolNode.ToolType = (ToolType)value;
            CreateFields(currentToolNode, true);
            return;
        }

        if (key == ElementKey.ActionType) currentActionNode.setActionType((ActionType)value);
        CreateFields(currentActionNode, true);
    }

    public void UpdateValue(ElementKey key, float value)
    {
        if (key == ElementKey.VideoStartTime) currentVideoNode.setStartTime(value);
        if (key == ElementKey.VideoEndTime) currentVideoNode.setEndTime(value);
        if (key == ElementKey.VideoLoopTime) currentVideoNode.setLoopTime(value);

        if (key == ElementKey.ActionStartTime) currentActionNode.setStartTime(value);
        if (key == ElementKey.ActionEndTime) currentActionNode.setEndTime(value);

        //These all SO FAR (5) need to refresh the timeline markers, so I'm just going to do it here for all of them
        editorVideoPlayer.RefreshTimeline();

    }

    public void StartWorldMarkerPositioning()
    {
        if (editingAreaMarker)
            editorVideoControls.NodeCanvas.SetActive(false);
        else
            editorVideoControls.PlacingWorldSpaceMarker = true;

        editorVideoPlayer.VideoPlayer.Pause();
        editingAreaMarker = false;
    }
    public void StopAreaMarkerPositioning(Vector3[] vertices)
    {
        editorVideoControls.PlacingWorldSpaceMarker = false;
        currentActionNode.setAreaMarkerVertices(vertices);
        CreateFields(currentActionNode, true);
    }

    public void EditAreaMarkerPositioning()
    {
        editingAreaMarker = true;
        CreateWorldMarker(currentActionNode, true);
        StartWorldMarkerPositioning();
    }

    public void OpenIconSelection() => iconSelectionPanel.SetActive(true);

    public void SetIcon(string iconName)
    {
        RemoveMarkers();
        CurrentActionNode.setIconName(iconName);
        CreateWorldMarker(currentActionNode);
        actionDraggables.Refresh();
    }
    void OpenQuestionCreator() => questionCreatorPanel.SetActive(true);

    void OpenInfoCreator() => infoCreatorPanel.SetActive(true);

    private void SetCurrentRotationAsStartRotation() => CurrentVideoNode.setVideoStartRotation(videoCamTransform.localEulerAngles);

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
    void CreateElement(string header, ElementKey key, GameObject prefab, float value, float defaultValue)
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
    void CreateElement(string header, ElementKey key, GameObject prefab, int value)
    {
        var elementObj = Instantiate(prefab, transform);
        var element = elementObj.GetComponent<NodeInspectorElement>();
        element.InitializeElement(
            header,
            key,
            value);
    }

    void CreateElement(string header, GameObject prefab, System.Action buttonAction)
    {
        var elementObj = Instantiate(prefab, transform);
        var element = elementObj.GetComponent<NodeInspectorElement>();
        element.InitializeElement(
            header,
            buttonAction);
    }


    public void NullCurrentNodes()
    {
        RemoveMarkers();
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
        if (currentToolNode != null)
        {
            currentToolNode.GetComponent<Outline>().enabled = false;
            currentToolNode = null;
        }
    }

    void RemoveMarkers()
    {
        if (currentWorldMarkers.Count != 0)
        {
            foreach (var item in currentWorldMarkers)
                Destroy(item);
            currentWorldMarkers.Clear();
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
    ActionType,
    VideoLoop,
    VideoLoopTime,
    VideoStartTime,
    VideoEndTime,
    ToolType,
    Timer
}