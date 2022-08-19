using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using static Enums;
using UnityEngine.UI;

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
    [SerializeField] StructureManager structureManager = null;
    [SerializeField] DragSelect dragSelect = null;
    [SerializeField] NodeSelectionHandler nodeSelectionHandler = null;
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
    [SerializeField] Color32 videoNodeColor=new Color32(0,0,0,255);
    [SerializeField] Color32 actionNodeColor=new Color32(100,0,0,255);
    [SerializeField] Color32 toolNodeColor=new Color32(0,100,0,255);
    
    [Header("World Elements")]
//    [SerializeField] GameObject infoCreatorPanel = null;
//    [SerializeField] ActionDraggables actionDraggables = null;
    [SerializeField] GameObject worldButtonPrefab = null;
    [SerializeField] GameObject floorButtonPrefab = null;
    [SerializeField] GameObject areaButtonPrefab = null;
    List<GameObject> currentWorldMarkers = new List<GameObject>();

    public VideoNode CurrentVideoNode => currentVideoNode;
    public ActionNode CurrentActionNode => currentActionNode;
    public ToolNode CurrentToolNode => currentToolNode;
    public ActionDraggables ActionDraggables => actionDraggables;
    public NodeSelectionHandler NodeSelectionHandler => nodeSelectionHandler;
    public DragSelect DragSelect => dragSelect;

    private Image backgroundImage = null;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DestroyAllInspectorElements();
        nodeSelectionHandler = new NodeSelectionHandler();

        backgroundImage=gameObject.GetComponent<Image>();
    }

    void DestroyAllInspectorElements()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    #region Fields

    ///<summary>
    /// Creates and populates all the fields for the editor in-app-inspector
    ///</summary>
    public void CreateFields(VideoNode node, bool isUpdate = false)
    {
        
        //RefreshSelection();
        currentVideoNode = node;
        actionDraggables.CreateActionDraggables(node);

        videoCamTransform.localEulerAngles = currentVideoNode.GetVideoStartRotation();

        //Put bg to video
        if (!isUpdate)
        {
            var canUseVideo = editorVideoPlayer.TryChangeVideo(node.GetVideoFileName());
            if (canUseVideo)
                editorVideoPlayer.VideoPlayer.prepareCompleted += CreateVideoFields;
        }

        //Create new ones
        CreateElement("Video filename",
                      ElementKey.VideoFileName,
                      filenameElementPrefab,
                      currentVideoNode.GetVideoFileName());

        if (isUpdate)
            CreateVideoFields(editorVideoPlayer.VideoPlayer);
        
        //Change Color
        backgroundImage.color = videoNodeColor;
    }

    ///<summary>
    /// Overload for CreateFields that takes in an action node instead
    ///</summary>
    public void CreateFields(ActionNode node, bool isUpdate = false)
    {
        var oldVideoNode = currentVideoNode;

        NullCurrentNodes();
        DestroyAllInspectorElements();

        //if action is selected, both lists can be cleared, actions dont need to support multiselect ( I think? )
        foreach (var item in NodeSelectionHandler.SelectedNodes)
        {
            var selectable = structureManager.GetNode(item);
            selectable.Outline(false);
        }
        nodeSelectionHandler = new NodeSelectionHandler();
        node.GetComponent<Outline>().enabled = true;

        currentActionNode = node;
        currentVideoNode = node.GetComponentInParent<VideoNode>();
        actionDraggables.CreateActionDraggables(currentVideoNode);

        videoCamTransform.localEulerAngles = currentVideoNode.GetVideoStartRotation();

        //Clicking on action that isnt from the same video as the previous one (or none)
        if (isUpdate == false && (oldVideoNode == null || currentVideoNode != oldVideoNode))
        {
            editorVideoPlayer.TryChangeVideo(currentVideoNode.GetVideoFileName());
            editorVideoPlayer.VideoPlayer.prepareCompleted += CreateActionFields;
        }
        else
        {
            CreateActionFields(editorVideoPlayer.VideoPlayer);
            editorVideoPlayer.RefreshTimeline();
        }
        //Change Color
        backgroundImage.color = actionNodeColor;
    }

    ///<summary>
    /// Overload for CreateFields that takes in a ToolNode instead
    ///</summary>
    public void CreateFields(ToolNode node, bool isUpdate = false)
    {
        editorVideoPlayer.VideoPlayer.Stop();
        //RefreshSelection();

        currentToolNode = node;


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
        //Change Color
        backgroundImage.color = toolNodeColor;
    }



    private void CreateVideoFields(VideoPlayer source)
    {
        editorVideoPlayer.VideoPlayer.prepareCompleted -= CreateVideoFields;
        CreateElement("Loop video",
                      ElementKey.VideoLoop,
                      toggleElementPrefab,
                      currentVideoNode.GetLoop());
        CreateElement("Video loop time",
                      ElementKey.VideoLoopTime,
                      timeElementPrefab,
                      currentVideoNode.GetLoopTime(),
                      0);
        CreateElement("Video start time",
                      ElementKey.VideoStartTime,
                      timeElementPrefab,
                      currentVideoNode.GetStartTime(),
                      0);
        CreateElement("Video end time",
                      ElementKey.VideoEndTime,
                      timeElementPrefab,
                      currentVideoNode.GetEndTime(),
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

    #endregion

    internal void RefreshSelection()
    {
        NullCurrentNodes();
        DestroyAllInspectorElements();

        foreach (var item in NodeSelectionHandler.OldNodes)
        {
            var selectable = structureManager.GetNode(item);
            if(selectable != null)
                selectable.Outline(false);
        }

        foreach (var item in NodeSelectionHandler.SelectedNodes)
        {
            var selectable = structureManager.GetNode(item);
            if(selectable != null)
                selectable.Outline(true);
        }

        NodeSelectionHandler.SaveOldList();

        if (NodeSelectionHandler.SelectedNodes.Count > 1)
            CreateElement("Items selected:", ElementKey.MultiSelectInfo, textElementPrefab, NodeSelectionHandler.SelectedNodes.Count.ToString());
        else if (NodeSelectionHandler.SelectedNodes.Count == 1)
        {
            var selectable = structureManager.GetNode(NodeSelectionHandler.SelectedNodes[0]);
            NodeType selectableType = selectable.NodeType;

            //hate it here
            if (selectableType == NodeType.Video)
                CreateFields((VideoNode)selectable);
            else if (selectableType == NodeType.Tool)
                CreateFields((ToolNode)selectable);
        }
    }

    ///<summary>
    /// Spawns markers for all the actions in the current video
    ///</summary>
    void CreateWorldMarkers()
    {
        foreach (var item in currentVideoNode.GetActionNodeList())
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

    #region Updating Values

    public void UpdateValue(ElementKey key, string value)
    {
        if (key == ElementKey.VideoFileName)
        {
            editorVideoPlayer.TryChangeVideo(value);
            currentVideoNode.SetVideoFileName(value);
        }
        if (key == ElementKey.ActionName)
        {
            currentActionNode.SetActionText(value);
            CreateWorldMarker(currentActionNode, true);
        }
        if (key == ElementKey.Timer)
        {
            if (float.TryParse(value, out float result))
                currentActionNode.SetActionTimer(result);
            else Debug.LogError("Time is in the wrong format!");
        }
        UndoRedoHandler.instance.SaveState();

    }

    public void UpdateValue(ElementKey key, bool value)
    {
        if (key == ElementKey.VideoLoop)
        {
            currentVideoNode.SetLoop(value);
            //If video will loop, none of the actions can autoend
            if (value)
            {
                foreach (var item in currentVideoNode.GetActionNodeList())
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
                foreach (var item in currentVideoNode.GetActionNodeList())
                {
                    if (item != currentActionNode)
                        item.setAutoEnd(false);
                }

                currentVideoNode.SetLoop(false);
            }
            //changing autoend changes what other fields are shown so need to redraw
            CreateFields(currentActionNode, true);
        }
        editorVideoPlayer.RefreshTimeline();
        UndoRedoHandler.instance.SaveState();

    }

    public void UpdateValue(ElementKey key, int value)
    {
        if (key == ElementKey.ToolType)
        {
            currentToolNode.ToolType = (ToolType)value;
            CreateFields(currentToolNode, true);
            UndoRedoHandler.instance.SaveState();

            return;
        }

        if (key == ElementKey.ActionType) currentActionNode.SetActionType((ActionType)value);
        CreateFields(currentActionNode, true);
        UndoRedoHandler.instance.SaveState();

    }

    public void UpdateValue(ElementKey key, float value)
    {
        if (key == ElementKey.VideoStartTime) currentVideoNode.SetStartTime(value);
        if (key == ElementKey.VideoEndTime) currentVideoNode.SetEndTime(value);
        if (key == ElementKey.VideoLoopTime) currentVideoNode.SetLoopTime(value);

        if (key == ElementKey.ActionStartTime) currentActionNode.SetStartTime(value);
        if (key == ElementKey.ActionEndTime) currentActionNode.SetEndTime(value);

        //These all SO FAR (5) need to refresh the timeline markers, so I'm just going to do it here for all of them
        editorVideoPlayer.RefreshTimeline();
        UndoRedoHandler.instance.SaveState();
    }
    #endregion
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
        currentActionNode.SetAreaMarkerVertices(vertices);
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
        CurrentActionNode.SetIconName(iconName);
        CreateWorldMarker(currentActionNode);
        actionDraggables.Refresh();
    }
    void OpenQuestionCreator() => questionCreatorPanel.SetActive(true);

    void OpenInfoCreator() => infoCreatorPanel.SetActive(true);

    private void SetCurrentRotationAsStartRotation() => CurrentVideoNode.SetVideoStartRotation(videoCamTransform.localEulerAngles);

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
    Timer,
    MultiSelectInfo

}
