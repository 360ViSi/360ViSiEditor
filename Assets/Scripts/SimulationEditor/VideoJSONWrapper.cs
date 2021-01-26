using System.Collections.Generic;

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

            if (actionJSONObjects != null)
                actions = actionJSONObjects;
        }
        public int videoID = -2;
        public bool loop = true;
        public string videoFileName = "";
        public List<ActionJSONObject> actions;
    }

    [System.Serializable]
    public class ActionJSONObject
    {
        public ActionJSONObject(ActionNode actionNode)
        {
            actionText = actionNode.getActionText();
            nextVideo = actionNode.getNextVideoID();
        }

        public string actionText = "";
        public int nextVideo = -2;
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

