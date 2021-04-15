using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VideoData : MonoBehaviour
{
    [SerializeField] GameEvent endGameEvent;
    //variables used in editor
    [SerializeField]
    private SO_String projectFilePath;
    //private variables
    private VideoStructure videoStructure;

    public VideoStructure VideoStructure { get => videoStructure; set => videoStructure = value; }

    //awake is called before start
    void Awake()
    {
        var fileString = File.ReadAllText(projectFilePath.data);
        VideoStructure = JsonUtility.FromJson<VideoStructure>(fileString);

        //debuging print video filenames
        foreach (VideoPart video in VideoStructure.videos)
        {
            Debug.Log(video.getVideoFileName());
        }
        print($"Is valid structure: {isVideoIDsValid(VideoStructure)}");
    }

    //public functions

    public VideoPart getVideoPart(int askedVideoID)
    {
        return VideoStructure.getVideoPart(askedVideoID);
    }

    public VideoPart getStartPart()
    {
        return VideoStructure.getStartPart();
    }

    public string getFolderPath()
    {
        return Utilities.FolderPathFromFilePath(projectFilePath.data);
    }

    //private functions
    private bool isVideoIDsValid(VideoStructure structure)
    {
        // no overlapping videoIDs
        int[] videoIDs = new int[structure.videos.Length];

        //construct videoIDs list and
        //check that all video IDs are positive (not negative)
        for (int i = 0; i < structure.videos.Length; i++)
        {
            videoIDs[i] = structure.videos[i].getNodeId();

            if (videoIDs[i] < 0)
            {
                Debug.Log("There is negative video ID used: " + structure.videos[i].getVideoFileName());
                return false;
            }
        }

        //check that every video ID is used only ones
        if (videoIDs.Count() != videoIDs.Distinct().Count())
        {
            Debug.Log("There is dublicated videoIDs");
            return false;
        }

        //check that there is video for every action
        foreach (VideoPart videoPart in structure.videos)
        {
            for (int a = 0; a < videoPart.getActionCount(); a++)
            {
                Action action = videoPart.getAction(a);
                for (int i = 0; i < videoIDs.Length; i++)
                {

                    //ID -1 is "end action"
                    if (action.getNextNode() == -1)
                    {
                        break;
                    }

                    //video ID matches
                    if (action.getNextNode() == videoIDs[i])
                    {
                        break;
                    }

                    //No matching video ID in videoIDs list
                    if (i == videoIDs.Length - 1)
                    {
                        Debug.Log("No such video ID " + action.getNextNode() + ". Action " + action.getActionText() + " in videoPart (ID) " + videoPart.getNodeId());
                        return false;
                    }
                }
            }
        }
        return true;
    }
}

//Videodata structure
[System.Serializable]
public class VideoStructure
{
    public VideoPart[] videos;
    public Tool[] tools;
    public int startId;
    public VideoPart getVideoPart(int askedVideoID)
    {
        foreach (VideoPart vPart in videos)
            if (vPart.getNodeId() == askedVideoID)
                return vPart;
        //Debug.Log("There is not a videoID "+askedVideoID);
        return null;
    }
    public VideoPart getStartPart()
    {
        return getVideoPart(startId);
    }
}

[System.Serializable]
public class VideoPart
{
    public string videoFileName;
    public int nodeId;
    public List<Action> actions;
    public bool loop;
    public float loopTime;
    public float startTime;
    public float endTime;
    public Vector3 videoStartRotation;
    public string getVideoFileName() => videoFileName;
    public int getNodeId() => nodeId;
    public int getActionCount() => actions.Count;

    public string getActionText(int askedActionID)
    {
        Action action = getAction(askedActionID);
        if (action != null)
        {
            return action.getActionText();
        }
        else
        {
            return "";
        }
    }

    public bool getLoop() => loop;
    public float getLoopTime() => loopTime;
    public float getStartTime() => startTime;
    public float getEndTime() => endTime;
    public Vector3 getVideoStartRotation() => videoStartRotation;

    public int getNextVideoID(int askedActionID)
    {
        Action action = getAction(askedActionID);
        if (action != null)
        {
            return action.getNextNode();
        }
        else
        {
            return -2; //error
        }

    }

    public Action getAction(int askedActionID)
    {
        if (askedActionID < 0)
        {
            return null;
        }
        if (askedActionID > actions.Count - 1)
        {
            return null;
        }
        return actions[askedActionID];
    }
}

[Serializable]
public class Action
{
    public string actionText;
    public int nextNode;
    public bool autoEnd;
    public float startTime;
    public float endTime;
    public ActionType actionType;
    public Vector3 worldPosition;
    public string iconName = "touch";
    public Vector3[] areaMarkerVertices;
    public float timer;
    public string getActionText() => actionText;
    public int getNextNode() => nextNode;
    public bool getAutoEnd() => autoEnd;
    public float getStartTime() => startTime;
    public float getEndTime() => endTime;
    public ActionType getActionType() => actionType;
    public Vector3 getWorldPosition() => worldPosition;
    public string getIconName() => iconName;
    public Vector3[] getAreaMarkerVertices() => areaMarkerVertices;
    public float getTimer() => timer;
}

[Serializable]
public class Tool
{
    public int nodeId;
    public int[] nextNodes;
    public int toolTypeInt;
    public Question question;
    public string infoText;

    internal void ProcessTool(Action<int> goToNode)
    {
        ToolType toolType = (ToolType)toolTypeInt;

        switch (toolType)
        {
            case ToolType.Random:
                ProcessRandomTool(goToNode);
                break;
            case ToolType.QuestionTask:
                ProcessQuestionTool();
                break;
            case ToolType.Info:
                ProcessInfoTool();
                break;
        }
    }

    private void ProcessQuestionTool() => PlayerQuestionManager.instance.OpenQuestionPanel(this);
    private void ProcessInfoTool() => PlayerInfoManager.instance.OpenInfoPanel(this);

    void ProcessRandomTool(Action<int> goToNode)
    {
        var rnd = nextNodes[UnityEngine.Random.Range(0, nextNodes.Length)];
        Debug.Log("Randomed to " + rnd);
        goToNode?.Invoke(rnd);
    }
}
