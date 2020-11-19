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

    public int getNextVideoID()
    {
      // get video node from connected port
      // and get that VideoID (-2 if not connected)

      if (portGameObject==null)
      {
        Debug.Log(this.name + "Have no port game object");
        return -2;
      }

      NodePort connectedPort = portGameObject.getConnectedPort();
      if (connectedPort == null)
      {
        return -2;
      }

      VideoNode connectedVideoNode = connectedPort.getParentVideoNode();
      if(connectedVideoNode == null)
      {
        return -2;
      }
      else
      {
        return connectedVideoNode.getVideoID();
      }
    }

}
