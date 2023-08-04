using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

// Generates Grid Lines using a Mesh (N-by-N grids only)
// Even when moving the camera the grid stays in place so easier to work with

public class GridMesh : MonoBehaviour
{
    public int GridSize;
    public float CellSize;
    public bool fitToCamera;

    private Vector2 minCamPosForTranslation, maxCamPosForTranslation;

    private void Start()
    {
        if (CellSize <= 0 || GridSize <= 0)
        {
            Debug.LogWarning("Size should not be negative or zero.");
            return;
        }

        if(fitToCamera)
        {
            float camSizeY = Camera.main.orthographicSize * 2;
            float camSizeX = camSizeY * Camera.main.aspect;

            GridSize = Mathf.CeilToInt(Mathf.Max(camSizeY, camSizeX) / 2);
        }

        minCamPosForTranslation = new Vector2(-CellSize, -CellSize);
        maxCamPosForTranslation = new Vector2(CellSize, CellSize);

        GenerateGrid();
    }

    private void Update()
    {
        // This is used for translating the grid to only fit within the view of camera especially if using fitToCamera

        var camPos = Camera.main.transform.position;

        if(camPos.x < minCamPosForTranslation.x)
        {
            transform.position -= Vector3.right * CellSize;
            minCamPosForTranslation.x -= CellSize;
            maxCamPosForTranslation.x -= CellSize;
        }
        else if(camPos.x > maxCamPosForTranslation.x)
        {
            transform.position += Vector3.right * CellSize;
            maxCamPosForTranslation.x += CellSize;
            minCamPosForTranslation.x += CellSize;
        }

        if(camPos.y < minCamPosForTranslation.y)
        {
            transform.position -= Vector3.up * CellSize;
            minCamPosForTranslation.y -= CellSize;
            maxCamPosForTranslation.y -= CellSize;
        }
        else if(camPos.y > maxCamPosForTranslation.y)
        {
            transform.position += Vector3.up * CellSize;
            maxCamPosForTranslation.y += CellSize;
            minCamPosForTranslation.y += CellSize;
        }
    }

    private void GenerateGrid()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();
        var indicies = new List<int>();

        int index = 0;

        // Unusual cell sizes can cause the origin to not line up properly so we calculate a new size where GridSize is the max possible size
        float fittedGridSize = Mathf.Ceil(GridSize / CellSize) * CellSize * 2;

        for (float i = 0; i <= fittedGridSize; i += CellSize, index++)
        {
            verticies.Add(new Vector3(i, 0, 0));
            verticies.Add(new Vector3(i, fittedGridSize, 0));

            indicies.Add(4 * index + 0);
            indicies.Add(4 * index + 1);

            verticies.Add(new Vector3(0, i, 0));
            verticies.Add(new Vector3(fittedGridSize, i, 0));

            indicies.Add(4 * index + 2);
            indicies.Add(4 * index + 3);
        }

        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.white;

        float center = fittedGridSize / 2;

        transform.Translate(-center, -center, -1);
    }

    private void GenerateGrid2()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();
        var indicies = new List<int>();

        int index = 0;

        float actualSizeY = Mathf.Ceil(Camera.main.orthographicSize / CellSize) * CellSize * 2;
        float actualSizeX = actualSizeY * Camera.main.aspect;

        for (float i = 0; i <= actualSizeY; i += CellSize, index++)
        {
            verticies.Add(new Vector3(i, 0, 0));
            verticies.Add(new Vector3(i, actualSizeY, 0));

            indicies.Add(4 * index + 0);
            indicies.Add(4 * index + 1);

            verticies.Add(new Vector3(0, i, 0));
            verticies.Add(new Vector3(actualSizeX, i, 0));

            indicies.Add(4 * index + 2);
            indicies.Add(4 * index + 3);
        }

        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //meshRenderer.material.SetVector("_Tint", gridColor);

        float centerY = actualSizeY / 2;
        float centerX = actualSizeX / 2;

        transform.Translate(-centerX, -centerY, -1);
    }
}