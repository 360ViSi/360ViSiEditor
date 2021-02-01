using System.Collections.Generic;
using UnityEngine;

///<summary>
///This class handles the conversion between structuremanager's video and action nodes and a json format
///</summary>
[System.Serializable]
public class VideoJSONWrapper
{
    public VideoJSONWrapper(List<VideoNode> videoNodes, int startId)
    {
        videos = ConvertVideoNodeListToJSONFormat(videoNodes);
        this.startId = startId;
    }

    public List<VideoJSONObject> videos;
    public int startId = -1;

    #region JSONObjects

    [System.Serializable]
    public class VideoJSONObject
    {
        public VideoJSONObject(VideoNode videoNode, List<ActionJSONObject> actionJSONObjects = null)
        {
            videoID = videoNode.getVideoID();
            videoFileName = videoNode.getVideoFileName();
            position = videoNode.GetComponent<RectTransform>().anchoredPosition;
            loop = videoNode.getLoop();
            loopTime = videoNode.getLoopTime();

            if (actionJSONObjects != null)
                actions = actionJSONObjects;
        }
        public int videoID = -2;
        public bool loop = false;
        public string videoFileName = "";
        public float loopTime = 0;
        public Vector2 position = Vector2.zero;
        public List<ActionJSONObject> actions;
    }

    [System.Serializable]
    public class ActionJSONObject
    {
        public ActionJSONObject(ActionNode actionNode)
        {
            actionText = actionNode.getActionText();
            nextVideo = actionNode.getNextVideoID();
            autoEnd = actionNode.getAutoEnd();
        }

        public string actionText = "";
        public int nextVideo = -2;
        public bool autoEnd = false;
    }
    #endregion

    public List<VideoJSONObject> ConvertVideoNodeListToJSONFormat(List<VideoNode> videoNodes)
    {
        var output = new List<VideoJSONObject>();

        foreach (var item in videoNodes)
        {
            var actions = ConvertActionNodeListToJSONFormat(item.getActionNodeList());
            output.Add(new VideoJSONObject(item, actions));
        }

        return output;
    }

    public List<ActionJSONObject> ConvertActionNodeListToJSONFormat(List<ActionNode> actionNodes)
    {
        if(actionNodes.Count == 0)
            return null;

        var output = new List<ActionJSONObject>();
        foreach (var item in actionNodes)
            output.Add(new ActionJSONObject(item));

        return output;
    }
}

