using System.Collections.Generic;

[System.Serializable]
public class VideoJSONWrapper
{
    public VideoJSONWrapper(List<VideoJSONObject> videoNodes)
    {
        videos = videoNodes;
    }
    public List<VideoJSONObject> videos;
    public List<ActionJSONObject> actionJSONObjects;

    [System.Serializable]
    public class VideoJSONObject
    {
        public VideoJSONObject(VideoNode videoNode, List<ActionJSONObject> actionJSONObjects = null)
        {
            videoID = videoNode.getVideoID();

            if (actionJSONObjects != null)
                actions = actionJSONObjects;
        }
        public int videoID = -2;
        public List<ActionJSONObject> actions = new List<ActionJSONObject>();
    }

    [System.Serializable]
    public class ActionJSONObject
    {
        public ActionJSONObject(ActionNode actionNode)
        {
            actionName = actionNode.getActionText();
            nextVideoID = actionNode.getNextVideoID();
        }

        public string actionName = "";
        public int nextVideoID = -2;
    }

    public static List<VideoJSONObject> ConvertVideoNodeListToJSONFormat(List<VideoNode> videoNodes)
    {
        var output = new List<VideoJSONObject>();

        foreach (var item in videoNodes)
        {
            var actions = ConvertActionNodeListToJSONFormat(item.getActionNodeList());
            output.Add(new VideoJSONObject(item, actions));
        }

        return output;
    }

    public static List<ActionJSONObject> ConvertActionNodeListToJSONFormat(List<ActionNode> actionNodes)
    {
        var output = new List<ActionJSONObject>();
        foreach (var item in actionNodes)
            output.Add(new ActionJSONObject(item));

        return output;
    }
}

