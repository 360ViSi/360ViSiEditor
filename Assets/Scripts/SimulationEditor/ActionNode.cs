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

      //setup ActionNode GameObject color
      thisImage = GetComponent<Image>();
      this.setMode();

    }

    void Update()
    {
    }

    public void setMode()
    {
      // Changes the color regarding to connection status
      // Colors are predefined in Prefab (Unity Inspector)

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

}
