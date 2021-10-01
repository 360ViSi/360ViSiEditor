using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///This class handles the conversion between structuremanager's video and action nodes and a json format
///</summary>
[System.Serializable]
public class VideoJSONWrapper
{
    public VideoJSONWrapper(List<VideoNode> videoNodes, List<ToolNode> toolNodes, int startId, Vector2 startNodePosition, Vector2 endNodePosition)
    {
        videos = ConvertVideoNodeListToJSON(videoNodes);
        tools = ConvertToolNodeListToJSON(toolNodes);
        this.startId = startId;
        this.startNodePosition = startNodePosition;
        this.endNodePosition = endNodePosition;
    }
    public List<VideoJSONObject> videos;
    public List<ToolJSONObject> tools;
    public int startId = -1;
    public Vector2 startNodePosition;
    public Vector2 endNodePosition;

    #region JSONObjects

    [System.Serializable]
    public class VideoJSONObject
    {
        public VideoJSONObject(VideoNode videoNode, List<ActionJSONObject> actionJSONObjects = null)
        {
            nodeId = videoNode.GetVideoID();
            videoFileName = videoNode.GetVideoFileName();
            nodePosition = videoNode.GetComponent<RectTransform>().anchoredPosition;
            loop = videoNode.GetLoop();
            loopTime = videoNode.GetLoopTime();
            startTime = videoNode.GetStartTime();
            endTime = videoNode.GetEndTime();
            videoStartRotation = videoNode.GetVideoStartRotation();

            if (actionJSONObjects != null)
                actions = actionJSONObjects;
        }
        public int nodeId = -2;
        public string videoFileName = "";
        public float startTime = 0;
        public float endTime = 1;
        public bool loop = false;
        public float loopTime = 0;
        public Vector3 videoStartRotation = Vector3.zero;
        public Vector2 nodePosition = Vector2.zero;
        public List<ActionJSONObject> actions;
    }

    ///<summary>
    /// Handles converting ActionNode to a JSON friendly version
    ///</summary>
    [System.Serializable]
    public class ActionJSONObject
    {
        public ActionJSONObject(ActionNode actionNode)
        {
            actionText = actionNode.getActionText();
            nextNode = actionNode.getNextVideoID();
            autoEnd = actionNode.getAutoEnd();
            startTime = actionNode.getStartTime();
            endTime = actionNode.getEndTime();
            actionType = actionNode.getActionType();
            worldPosition = actionNode.getWorldPosition();
            iconName = actionNode.getIconName();
            areaMarkerVertices = actionNode.getAreaMarkerVertices();
            timer = actionNode.getActionTimer();
        }

        public string actionText = "";
        public int nextNode = -2;
        public bool autoEnd = false;
        public float startTime = 0;
        public float endTime = 1;
        public ActionType actionType = ActionType.ScreenButton;
        public Vector3 worldPosition = Vector3.zero;
        public string iconName = "touch";
        public Vector3[] areaMarkerVertices = null;
        public float timer = 0;
    }

    ///<summary>
    /// Handles converting ToolNode to a JSON friendly version
    ///</summary>
    [System.Serializable]
    public class ToolJSONObject
    {
        public ToolJSONObject(ToolNode toolNode)
        {
            nodeId = toolNode.NodeId;
            nextNodes = toolNode.GetNextVideos();
            nodePosition = toolNode.GetComponent<RectTransform>().anchoredPosition;
            toolTypeInt = (int)toolNode.ToolType;
            question = toolNode.Question;
            infoText = toolNode.InfoText;
            spritePath = toolNode.SpritePath;
            video2Dpath = toolNode.Video2Dpath;
        }
        public int nodeId = -2;
        public int[] nextNodes = new int[0];
        public Vector2 nodePosition;
        public int toolTypeInt;
        public Question question;
        public string infoText;
        public string spritePath;
        public bool imageVisible;
        public string video2Dpath;
    }
    #endregion

    #region Helpers
    public List<VideoJSONObject> ConvertVideoNodeListToJSON(List<VideoNode> videoNodes)
    {
        var output = new List<VideoJSONObject>();

        foreach (var item in videoNodes)
        {
            var actions = ConvertActionNodeListToJSONFormat(item.GetActionNodeList());
            output.Add(new VideoJSONObject(item, actions));
        }

        return output;
    }

    public List<ActionJSONObject> ConvertActionNodeListToJSONFormat(List<ActionNode> actionNodes)
    {
        if (actionNodes.Count == 0)
            return null;

        var output = new List<ActionJSONObject>();
        foreach (var item in actionNodes)
            output.Add(new ActionJSONObject(item));

        return output;
    }

    public List<ToolJSONObject> ConvertToolNodeListToJSON(List<ToolNode> toolNodes)
    {
        if (toolNodes.Count == 0) return null;
        var output = new List<ToolJSONObject>();
        foreach (var item in toolNodes)
            output.Add(new ToolJSONObject(item));

        return output;
    }
    #endregion
}

