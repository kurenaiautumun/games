// Attach this to the GameObject with Camera component

using UnityEngine;
using UnityEngine.Rendering;

// Draws Grid Lines on the camera's view
// Must calculate the position everytime the camera moves

public class GridLineScript : MonoBehaviour
{
    public Material mat;
    public Color lineColor = Color.white;

    private float offsetY = 0, offsetX = 0;
    private float CellSize;
    private GameManagerController gameManagerController;

    private void Start()
    {
        gameManagerController = GameObject.Find("GameManager").GetComponent<GameManagerController>();

        RenderPipelineManager.endCameraRendering += RenderGridLines;
        CellSize = gameManagerController.gridCellSize;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= RenderGridLines;
    }

    public void EnableGridLines()
    {
        RenderPipelineManager.endCameraRendering += RenderGridLines;
    }

    public void DisableGridLines()
    {
        RenderPipelineManager.endCameraRendering -= RenderGridLines;
    }


    // Recalculates the grid offsets if the camera is translated by other means
    public void RecalculateGrid()
    {
        var camPos = transform.position;
        var gridPos = new Vector3(Mathf.Ceil(camPos.x / CellSize), Mathf.Ceil(camPos.y / CellSize), 0) * CellSize;

        offsetX = gridPos.x;
        offsetY = gridPos.y;
    }

    void RenderGridLines(ScriptableRenderContext context, Camera camera)
    {
        // TODO: Whatever fixes made here should also be adjusted with the grid coordinates in GameManagerController
        // TODO: Make the grid be able to handle dynamic changes (changing cell size, able to adjust to different camera sizes during gameplay)

        CellSize = gameManagerController.gridCellSize;

        if (CellSize <= 0)
        {
            Debug.LogError("Cannot have Cell size be negative or zero");
            return;
        }

        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector (Use Sprite-Default)");
            return;
        }

        // Translate the grid if we have moved the camera by some distance
        RecalculateGrid();

        // Calculate the size of the grid to fit the camera's view
        float actualSizeY = Mathf.Ceil(Camera.main.orthographicSize / CellSize) * CellSize * 2;
        float actualSizeX = Mathf.Round(Camera.main.orthographicSize * Camera.main.aspect / CellSize) * CellSize * 2;

        // Extend the grid by a cell on both ends so it doesn't cut abruptly when moving the camera
        actualSizeY += CellSize * 2;
        actualSizeX += CellSize * 2;

        // Find the center of the grid so the grid's center lies somewhat at the center of the camera
        float centerY = actualSizeY / 2;
        float centerX = actualSizeX / 2;

        // Using GL to draw the lines (Not Ortho)

        GL.Begin(GL.LINES);
        mat.SetPass(0);
        GL.Color(lineColor);

        // Horizontal Lines
        for (float i = 0; i < actualSizeY; i += CellSize)
        {
            GL.Vertex3(-centerX + offsetX, i - centerY + offsetY, 0);
            GL.Vertex3(actualSizeX - centerX + offsetX, i - centerY + offsetY, 0);
        }

        // Vertical Lines
        for (float i = 0; i < actualSizeX; i += CellSize)
        {
            GL.Vertex3(i - centerX + offsetX, -centerY + offsetY, 0);
            GL.Vertex3(i - centerX + offsetX, actualSizeY - centerY + offsetY, 0);
        }

        GL.End();

        
    }
}
