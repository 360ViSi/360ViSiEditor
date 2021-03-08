﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorAreaButton : MonoBehaviour
{
    [SerializeField] Transform[] points = new Transform[4];
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    bool mouseOver;
    NodeInspector nodeInspector;
    GameObject confirmButtonObject;
    bool editing;
    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        UpdateMesh();
    }

    private void ConfirmAreaButtonVertices()
    {
        var vertices = points.Select(e => e.localPosition).ToArray();
        nodeInspector.StopAreaMarkerPositioning(vertices);
        confirmButtonObject.SetActive(false);
        ShowVerticesAndHighlight(false);
    }

    private void Update()
    {
        if (mouseOver && editing)
            meshRenderer.material.SetColor("_Color", new Color(0, 1, 0, .5f));
        else
            meshRenderer.material.SetColor("_Color", new Color(1, 1, 1, .5f));
    }

    public void Initialize(NodeInspector nodeInspector, Camera videoCamera, Vector3[] vertices = null, bool showVertices = true, bool enableConfirmButton = false)
    {
        this.nodeInspector = nodeInspector;
        GetComponentInChildren<Canvas>().worldCamera = videoCamera;

        var confirmButton = GetComponentInChildren<Button>();
        confirmButtonObject = confirmButton.gameObject;
        confirmButton.onClick.AddListener(ConfirmAreaButtonVertices);
        confirmButtonObject.SetActive(enableConfirmButton);

        foreach (var item in points)
            item.GetComponent<AreaButtonDragVertex>().Initialize(this, videoCamera);

        if (vertices == null || vertices.Length == 0)
            return;

        ShowVerticesAndHighlight(showVertices);

        for (int i = 0; i < points.Length; i++)
            points[i].localPosition = vertices[i];
    }   

    void ShowVerticesAndHighlight(bool value)
    {
        editing = value;

        foreach (var item in points)
            item.GetComponent<MeshRenderer>().enabled = value;
    }

    internal void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4]{
            points[0].localPosition,
            points[1].localPosition,
            points[2].localPosition,
            points[3].localPosition
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

    private void OnMouseOver() => mouseOver = true;
    private void OnMouseExit() => mouseOver = false;
}
