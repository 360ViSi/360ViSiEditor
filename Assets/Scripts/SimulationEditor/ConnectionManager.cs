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
    // Do not allow fromNode==toNode
    // if there is not connection from "fromNode" create new one
    // if there is connection from "fromNode" chenge "toNode"
    // Only one connection per fromNode

    if(fromNode==toNode)
    {
      return;
    }

    List<Connection> fromConnections = getConnections(fromNode, null);
    if(fromConnections.Count==0)
    {
      Connection connection = new Connection();
      connections.Add(connection);
      connection.setFromNode(fromNode);
      connection.setToNode(toNode);
      connection.setConnectionLine(createConnectionLine(fromNode));
      Debug.Log("new connection created: "+connections.Count);
      return;
    }
    if(fromConnections.Count==1)
    {
      fromConnections[0].setToNode(toNode);
      Debug.Log("Connection endPort chanced: "+connections.Count);
      return;
    }
    Debug.Log("Error: There is more than one connection from nodePort"+fromNode.name);

  }

  public List<Connection> getConnections(NodePort fromPort, NodePort toPort)
  {
    //if toPort==null then get based only on from point
    //if fromPoint==null then "get" every connection
    List<Connection> collectionList = new List<Connection>();
    int[] connectionIndexes=getConnectionIndex(fromPort,toPort);
    foreach(int connectIndex in connectionIndexes)
    {
      collectionList.Add(connections[connectIndex]);
    }
    return collectionList;
  }

  public List<Connection> getEveryPortConnection(NodePort nodePort)
  {
    List<Connection> connectionList = new List<Connection>();
    // add "from" connections
    connectionList=getConnections(nodePort,null);
    //add "to" connections
    connectionList.AddRange(getConnections(null,nodePort));

    return connectionList;
  }

  public int[] getConnectionIndex(NodePort fromPort, NodePort toPort)
  {
    //return connection indexes that have fromPort and toPort
    //null is used as "wildcard"
    //empty list is returned if no matches
    List<int> connectionIndexList = new List<int>();
    for(int i=0;i< connections.Count; i++)
    {
      if(fromPort!=null && connections[i].getFromNode()!=fromPort)
      {
        //if "from" don't match -> continue
        continue;
      }
      if(toPort!=null && connections[i].getToNode()!=toPort)
      {
        //if "to" don't match -> continue
        continue;
      }
      //both "to" and "from" matches (or are null)
      connectionIndexList.Add(i);
    }
    return connectionIndexList.ToArray();
  }

  public void deleteConnection(NodePort fromPort, NodePort toPort)
  {
    //Get first connections
    //and Remove those from the connections list
    int[] connectionIndexes=getConnectionIndex(fromPort,toPort);
    foreach(int connectIndex in connectionIndexes)
    {
      Destroy(connections[connectIndex].getConnectionLine().gameObject);
      connections.RemoveAt(connectIndex);
    }
    Debug.Log("Connections deleted");
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

  public void redrawConnection(NodePort fromPort, NodePort toPort)
  {
    // redraw connectionlines
    // null is used as wildcard
    List<Connection> selectedConnections = getConnections(fromPort,toPort);
    foreach(Connection connection in selectedConnections)
    {
      connection.redrawConnectionLine();
    }
  }


  public void showConnectionLine(NodePort fromPort, NodePort toPort, bool visible)
  {
    //Get first connections
    //and make connectionline visible or unvisible
    int[] connectionIndexes=getConnectionIndex(fromPort,toPort);
    foreach(int connectIndex in connectionIndexes)
    {
      if (visible)
      {
        connections[connectIndex].getConnectionLine().show();
      }
      else
      {
        connections[connectIndex].getConnectionLine().hide();
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

  public void redrawConnectionLine()
  {
    connectionLine.redrawLine(fromNode.getPosition(),toNode.getPosition());
  }

  public void drawDragLine(Vector3 endPosition)
  {
    connectionLine.redrawLine(fromNode.getPosition(),endPosition);
  }
}
