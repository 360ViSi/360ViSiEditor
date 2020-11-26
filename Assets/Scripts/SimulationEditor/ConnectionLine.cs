using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLine : MonoBehaviour
{
  public int lineVertexCount = 20;
  public float bezierCurvature = 0.5f;

  private LineRenderer connectLine;
  private Vector3[] verticesPos;

  void Awake()
  {
    //Initialize LineRenderer
    connectLine = GetComponent<LineRenderer>();
    setPositionCount(connectLine, lineVertexCount);
    hide();
    //Debug.Log("line renderer initialized");
  }

  void Update()
  {

  }

  public void redrawLine(Vector3 startPos, Vector3 endPos)
  {
    //calculates and sets kurve vertex positions
    verticesPos = bezierize(bezierCurvature, lineVertexCount,startPos, endPos);
    connectLine.SetPositions(verticesPos);
  }

  public void hide()
  {
    connectLine.enabled = false;
  }

  public void show()
  {
    connectLine.enabled = true;
  }

  private Vector3[] bezierize(float curvature, int vertexCount, Vector3 startPoint, Vector3 endPoint)
  {
    // Calculates BezierCurve positions
    // curvature: control point's percentage distance from end controlPointStart
    // vertexcount: number of vetices on line = resolution

    Vector3[] points = new Vector3[vertexCount];

    // generate control points
    // y-coordinate comes from the end points
    Vector3 vec1 = endPoint - startPoint;
    vec1.y=0f;
    Vector3 controlPointStart = startPoint + curvature * (vec1);
    Vector3 controlPointEnd = endPoint - curvature * vec1;

    // t = percentage of the pathlength = next point on path
    float t = 0f;
    for (int i = 0; i < vertexCount; i++)
    {
      //Cubic bezier curve
      points[i] = Mathf.Pow(
      (1 - t), 3) * startPoint
      + 3 * Mathf.Pow((1 - t), 2) * t * controlPointStart
      + 3 * (1 - t) * Mathf.Pow(t, 2) * controlPointEnd
      + Mathf.Pow(t, 3) * endPoint;

      t += (1f / (points.Length-1));
    }
    return points;
  }

  private void setPositionCount(LineRenderer line, int newCount)
  {
    line.positionCount=newCount;
  }

}
