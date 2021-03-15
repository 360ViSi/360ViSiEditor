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
    [SerializeField] SO_Icons icons;
    [SerializeField] Transform videoCamTransform = null;
    [SerializeField] EditorVideoPlayer editorVideoPlayer = null;
    [SerializeField] EditorVideoControls editorVideoControls = null;
    [SerializeField] WorldInspector worldInspector = null;
    [SerializeField] GameObject iconSelectionPanel = null;
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
    public ActionNode CurrentActionNode { get => currentActionNode; }

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

        CreateWorldMarkers();
    }

    private void CreateActionFields(VideoPlayer source)
    {
        editorVideoPlayer.VideoPlayer.prepareCompleted -= CreateActionFields;
        CreateElement("Action name", ElementKey.ActionName, textElementPrefab, currentActionNode.getActionText());
        CreateElement("Video end action", ElementKey.ActionAutoEnd, toggleElementPrefab, currentActionNode.getAutoEnd());

        if (!currentActionNode.getAutoEnd())
            CreateElement("Action type", ElementKey.ActionType, dropdownElementPrefab, (int)currentActionNode.getActionType());

        if (currentActionNode.getActionType() != ActionType.ScreenButton)
            CreateElement("Set Marker", buttonElementPrefab, StartWorldMarkerPositioning);
        if (currentActionNode.getActionType() == ActionType.WorldButton)
            CreateElement("Change Icon", buttonElementPrefab, OpenIconSelection);
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
        if (key == ElementKey.ActionIsTimed)
        {
            currentActionNode.setIsTimed(value);
            currentActionNode.setStartTime(0);
            currentActionNode.setEndTime(1);
            CreateFields(currentActionNode, true);
        }
        editorVideoPlayer.RefreshTimeline();
    }

    public void UpdateValue(ElementKey key, int value)
    {
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
    ActionIsTimed,
    VideoLoop,
    VideoLoopTime,
    VideoStartTime,
    VideoEndTime
}