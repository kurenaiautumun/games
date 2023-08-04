// Attach this to the GameObject with Camera component

using UnityEngine;
using UnityEngine.Rendering;

// Draws Grid Lines on the camera's view
// Must calculate the position everytime the camera moves

public class GridLineScript : MonoBehaviour
{
    public Material mat;
    float CellSize = 2f;

    void Start()
    {
        RenderPipelineManager.endCameraRendering += Rend;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= Rend;
    }

    void Rend(ScriptableRenderContext context, Camera camera)
    {

        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        float actualSizeY = Mathf.Ceil(Camera.main.orthographicSize / CellSize) * CellSize * 2;
        float actualSizeX = actualSizeY * Camera.main.aspect;


        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();

        GL.Begin(GL.LINES);
        GL.Color(Color.red);

        for (float i = 0; i <= actualSizeY; i += CellSize)
        {
            GL.Vertex(new Vector3(0, i/actualSizeY, 0));
            GL.Vertex(new Vector3(1, i/actualSizeY, 0));
        }
        for (float i = 0; i <= actualSizeX; i += CellSize)
        {
            GL.Vertex(new Vector3(i / actualSizeX, 0, 0));
            GL.Vertex(new Vector3(i / actualSizeX, 1, 0));
        }

        GL.End();
        GL.PopMatrix();
    }
}
