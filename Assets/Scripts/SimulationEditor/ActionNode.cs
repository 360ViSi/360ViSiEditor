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

    private NodePort nodePort;
    private ConnectionManager connectionManager;
    private TextMeshProUGUI actionText;
    private Image thisImage;

    void Awake()
    {
      //setup object color
      thisImage = GetComponent<Image>();
      thisImage.color=notConnectedColor;

      // get NodePort
      nodePort = GetComponentInChildren<NodePort>();
      if (nodePort==null)
      {
        Debug.Log("There are no NodePort in "+ name);
      }

      // get Text object for action Text
      actionText = GetComponentInChildren<TextMeshProUGUI>();
      if (actionText==null)
      {
        Debug.Log("There are no text object in "+ name);
      }

      //get connectionManager
      connectionManager = GetComponentInParent<ConnectionManager>();
      if (connectionManager==null)
      {
        Debug.Log(name +" Did not get ConnectionManager");
        return;
      }
    }

    void Update()
    {
    }

    public void setMode()
    {
      // Changes the color regarding to connection status

      List<Connection> portConnections = connectionManager.getConnections(this.nodePort,null);

      //empty connection list == not connected
      if (portConnections.Count==0)
      {
        thisImage.color=notConnectedColor;
        return;
      }
      // get connection "to" node and its VideoNode
      VideoNode connectedVideoNode = portConnections[0].getToNode().getParentVideoNode();
      if (connectedVideoNode!=null && connectedVideoNode.getVideoID()==-1)
      {
        thisImage.color=endActionColor;
        return;
      }
      thisImage.color=connectedColor;
    }


    public void setActionText(string newActionText)
    {
      actionText.text=newActionText;
    }

    public string getActionText()
    {
      return actionText.text;
    }

    public NodePort getNodePort()
    {
      return nodePort;
    }

//    public int getNextVideoID()
//    {
      // get video node from connected port
      // and get that VideoID (-2 if not connected)

//      List<Connection> portConnections = connectionManager.getConnections(this.nodePort,null);
      //empty connection list == not connected
//      if (portConnections.Count==0)
//      {
//        return -2;
//      }

      // get connection "to" node and its VideoNode
//      VideoNode connectedVideoNode = portConnections[0].getToNode().getParentVideoNode();
//      if (connectedVideoNode==null)
//      {
//        return -2;
//      }

//      return connectedVideoNode.getVideoID();
//    }
}
