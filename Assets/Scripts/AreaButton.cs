using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AreaButton : MonoBehaviour
{
    int nextVideoID;
    SimulationManager simulationManager;
    MeshRenderer meshRenderer;
    float startTime;
    float endTime;
    bool mouseOver;
    [SerializeField] GameObject actionTextPanel;
    [SerializeField] TMP_Text tmpText;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetAction(Action action, SimulationManager simulationManager)
    {
        this.nextVideoID = action.getNextNode();
        this.startTime = action.getStartTime();
        this.endTime = action.getEndTime();
        this.simulationManager = simulationManager;
        tmpText.text = action.getActionText();

        transform.position = action.getWorldPosition();
        var oldRotation = transform.rotation;
        transform.LookAt(simulationManager.transform);
        SetupMesh(action.getAreaMarkerVertices());
    }

    private void Update()
    {

        if (mouseOver && EventSystem.current.IsPointerOverGameObject() == false)
        {
            meshRenderer.material.SetColor("_Color", new Color(0, 1, 0, .5f));
            actionTextPanel.SetActive(true);
            return;
        }

        meshRenderer.material.SetColor("_Color", new Color(1, 1, 1, .3f));
        actionTextPanel.SetActive(false);

    }

    private void SetupMesh(Vector3[] points)
    {
        var meshFilter = GetComponent<MeshFilter>();
        var meshCollider = GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4]{
            points[0],
            points[1],
            points[2],
            points[3]
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            0, 2, 1,
            2, 0, 3
        };

        mesh.triangles = tris;

        mesh.RecalculateNormals();

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void Activate() => simulationManager.GoToNode(nextVideoID);
    public void SetActive(double currentTime) => gameObject.SetActive(currentTime > startTime && currentTime <= endTime);
    private void OnMouseDown() => Activate();
    private void OnMouseEnter() => mouseOver = true;
    private void OnMouseExit() => mouseOver = false;
}