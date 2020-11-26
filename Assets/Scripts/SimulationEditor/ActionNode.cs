using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActionNode : MonoBehaviour
{
    [SerializeField]
    private Color connectedColor;
    [SerializeField]
    private Color notConnectedColor;
    [SerializeField]
    private Color endActionColor;

    private NodePort portGameObject;
    private TextMeshProUGUI actionText;
    private Image thisImage;

    void Awake()
    {
      //setup object color
      thisImage = GetComponent<Image>();
      thisImage.color=notConnectedColor;

      // get NodePort
      portGameObject = GetComponentInChildren<NodePort>();
      if (portGameObject==null)
      {
        Debug.Log("There are no NodePort in "+ name);
      }

      // get Text object for action Text
      actionText = GetComponentInChildren<TextMeshProUGUI>();
      if (portGameObject==null)
      {
        Debug.Log("There are no text object in "+ name);
      }

    }

    void Update()
    {
<<<<<<< .merge_file_a11980
      verticesPos[0] = startPortRectTrans.transform.position;
      if (nodeConnected && endPortRectTrans != null)
=======
    }

    public void setMode()
    {
      // Changes the color regarding to connection status

      NodePort connectedPort = portGameObject.getConnectedPort();
      Debug.Log("Connected port: "+ connectedPort);
      if (connectedPort==null)
      {
        thisImage.color=notConnectedColor;
        return;
      }
      VideoNode connectedVideoNode = connectedPort.GetComponentInParent<VideoNode>();
      if (connectedVideoNode!=null && connectedVideoNode.getVideoID()==-1)
>>>>>>> .merge_file_a07608
      {
        thisImage.color=endActionColor;
        return;
      }
<<<<<<< .merge_file_a11980
      
      connectLine.SetPositions(verticesPos);
=======
      thisImage.color=connectedColor;
>>>>>>> .merge_file_a07608
    }


    public void setActionText(string newActionText)
    {
      actionText.text=newActionText;
    }

    public string getActionText()
    {
      return actionText.text;
    }

    public int getNextVideoID()
    {
      // get video node from connected port
      // and get that VideoID (-2 if not connected)

<<<<<<< .merge_file_a11980
    public void Disconnect()
    {
      endPortRectTrans=null;
      nodeConnected=false;
      connectLine.enabled=false;
      nextVideoNode.RemoveInputNode(this);
      nextVideoNode=null;
      if(!isStartNode) GetComponent<Image>().color=baseColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
//      Debug.Log("OnBeginDrag");
      connectLine.enabled =true;
      nodeConnected=false;
      if(!isStartNode)
=======
      if (portGameObject==null)
>>>>>>> .merge_file_a07608
      {
        Debug.Log(this.name + "Have no port game object");
        return -2;
      }

      NodePort connectedPort = portGameObject.getConnectedPort();
      if (connectedPort == null)
      {
        return -2;
      }

<<<<<<< .merge_file_a11980
      nextVideoNode =dropNode.GetComponent<VideoNode>();
      nextVideoNode.AddInputNode(this);
      endPortRectTrans = nextVideoNode.portGameObject;
      nodeConnected=true;
      connectLine.enabled =true;

      //color change

      if (isStartNode)
=======
      VideoNode connectedVideoNode = connectedPort.getParentVideoNode();
      if(connectedVideoNode == null)
>>>>>>> .merge_file_a07608
      {
        return -2;
      }
      else
      {
        return connectedVideoNode.getVideoID();
      }
    }

}
