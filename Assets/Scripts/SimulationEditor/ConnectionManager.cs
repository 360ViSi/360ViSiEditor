using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
  [SerializeField]
  private GameObject connectionLinePrefab;

  private List<Connection> connections = new List<Connection>();
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void createConnection(NodePort fromNode, NodePort toNode)
  {
    // if there is not jet this connection make new one and add it to the list
    // Only one connection per fromNode

    Connection connection = getConnection(fromNode);
    if(connection==null)
    {
      connection = new Connection();
      connections.Add(connection);
      connection.setFromNode(fromNode);
      connection.setConnectionLine(createConnectionLine(fromNode));
    }
    connection.setToNode(toNode);
    Debug.Log("new connection created: "+connections.Count);
  }

  public bool isConnected(NodePort fromNode)
  {
    //Return true if there is connection that
    //starts from fromNode and ends to toNode
    foreach(Connection connection in connections)
    {
//      Debug.Log("connected from: "+connection.getFromNode().GetInstanceID()+", testing: "+fromNode.GetInstanceID());
//      Debug.Log("connected to: "+connection.getToNode().GetInstanceID()+", testing: "+toNode.GetInstanceID());
      if(connection.getFromNode()==fromNode)
      {
        Debug.Log("match");
        return true;
      }
    }
    Debug.Log("No match");
    return false;
  }
  public Connection getConnection(NodePort fromPort)
  {
    int connectionIndex=getConnectionIndex(fromPort);
    if(connectionIndex!= -1)
    {
      return connections[connectionIndex];
    }
    return null;
  }

  public int getConnectionIndex(NodePort fromPort)
  {
    //return connection index that have fromNode as first nodes
    //return -1 if there is no chuch connection
    for(int i=0;i< connections.Count; i++)
    {
      if (connections[i].getFromNode()==fromPort)
      {
        return i;
      }
    }
    return -1;
  }

  public void deleteConnection(NodePort fromNode)
  {
    //Get first connection (index) where fromNode is the first
    //and Remove it from the connections list
    //remove from the list (-1 = no such connection)
    int connectionIndex = getConnectionIndex(fromNode);
    if (connectionIndex !=-1)
    {
        Destroy(connections[connectionIndex].getConnectionLine().gameObject);
        connections.RemoveAt(connectionIndex);
    }
    Debug.Log("Connection deleted");
  }

  public void connectionList()
  {
    string list="";
    foreach(Connection connection in connections)
    {
      list += connection.getFromNode().GetInstanceID() + "->"+connection.getToNode().GetInstanceID()+";";
    }
    Debug.Log(list);
  }

  public void redrawConnection(NodePort fromNode)
  {
      try
      {
        Connection connection = getConnection(fromNode);
        connection.getConnectionLine().redrawLine(connection.getFromNode().getPosition(),connection.getToNode().getPosition());
      }
      catch(System.NullReferenceException e)
      {
        Debug.Log("Null on redrawConnection");
      }
  }

  public void showConnectionLine(NodePort fromNode, bool visible)
  {
    //Get first connection (index) where fromNode is the first
    //and make connectionline visible
    //remove from the list (-1 = no such connection)
    int connectionIndex = getConnectionIndex(fromNode);
    if (connectionIndex != -1)
    {
      if (visible)
      {
        connections[connectionIndex].getConnectionLine().show();
      }
      else
      {
        connections[connectionIndex].getConnectionLine().hide();
      }
    }
  }
  public ConnectionLine createConnectionLine(NodePort fromNode)
  {
    //Instantiate LineConnection
    GameObject newConnectionLine_GO = GameObject.Instantiate(connectionLinePrefab, fromNode.GetComponent<Transform>());
    ConnectionLine newConnectionLine = newConnectionLine_GO.GetComponent<ConnectionLine>();
    if (newConnectionLine==null)
    {
      Debug.Log(this +" Did not get ConnectionLine");
    }
    return newConnectionLine;
  }
}

public class Connection
{
  private NodePort fromNode;
  private NodePort toNode;
  private ConnectionLine connectionLine;

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

  public void setConnectionLine(ConnectionLine newConnectionLine)
  {
    connectionLine = newConnectionLine;
  }

  public ConnectionLine getConnectionLine()
  {
    return connectionLine;
  }
}
