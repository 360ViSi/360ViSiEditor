using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VideoNode : MonoBehaviour
{
  [SerializeField]
  private int defaultVideoID = -2;
  private List<GameObject> actionGameObjects = new List<GameObject>();

  // video structure parameters
  private int videoID=-2; //no ID -2 should give error when parsing
  private string videoFileName = "None"; //no video file

  void Awake()
  {
    //set default videoID
    videoID = defaultVideoID;
  }

  public void createNewActionNode(string actionText)
  {
    //get prefab from structureManager and initilize that
    //add action node to the list and position it in UI
    StructureManager structureManager = GetComponentInParent<StructureManager>();
    GameObject newActionGameObject = Instantiate(structureManager.getActionNodePrefab(), GetComponent<RectTransform>());
    actionGameObjects.Add(newActionGameObject);
    RectTransform newRectTransform=newActionGameObject.GetComponent<RectTransform>();
    newRectTransform.anchoredPosition = calculateActionImagePosition(newRectTransform);
    newActionGameObject.GetComponent<ActionNode>().setActionText(actionText);
  }

  public void setVideoID(int newVideoID)
  {
    videoID = newVideoID;
  }

  public int getVideoID()
  {
    return videoID;
  }

  public void setVideoFileName(string newVideoFileName)
  {
    videoFileName = newVideoFileName;
  }
  public string getVideoFileName()
  {
    return videoFileName;
  }

  public List<ActionNode> getActionNodeList()
  {
    List<ActionNode> actionNodes = new List<ActionNode>();
    foreach (GameObject actionGameObject in actionGameObjects)
    {
      actionNodes.Add(actionGameObject.GetComponent<ActionNode>());
    }
    return actionNodes;
  }

  private Vector2 calculateActionImagePosition(RectTransform newNodeRectTransform)
  {
    //Calculates position for the next action GameObject
    //Assumes that every action gameobject is in same size
    Rect newNodeRect = newNodeRectTransform.rect;
    Vector3 newNodeScale = newNodeRectTransform.localScale;
    Vector2 realDimensions=new Vector2(newNodeRect.width*newNodeScale.x, newNodeRect.height*newNodeScale.y);
    return new Vector2(realDimensions.x/2, -realDimensions.y*(actionGameObjects.Count-0.5f));
  }

}
