using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
  private List<Connection> connections = new List<Connection>();
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}

public class Connection
{
  private NodePort fromNode;
  private NodePort toNode;

  public void setFromNode(NodePort newFromNode)
  {
    fromNode = newFromNode;
  }
  public NodePort getFromNode()
  {
    return fromNode;
  }

  public void setToNode(NodePort newToNode)
  {
    toNode = newToNode;
  }
  public NodePort getToNode()
  {
    return toNode;
  }
}
