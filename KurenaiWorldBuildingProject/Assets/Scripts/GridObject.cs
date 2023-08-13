using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Using a dictionary would be better if adding more types dynamically
public enum objectType { None, House, Tree };

public class GridObject : MonoBehaviour
{
    public objectType type = objectType.None;
    public Vector2 gridPosition = Vector2.zero;
    public Vector2 size = Vector2.one;
    public GameObject altViewObject = null;

    private static Dictionary<int, string> objectTypeName = new Dictionary<int, string>()
    {
        {0, "" },
        {1, "House" },
        {2, "Tree" }    
    };

    public static string GetName(objectType type)
    {
        return objectTypeName.TryGetValue((int)type, out var name) ? name : string.Empty;
    }

    public Vector2 GetWorldPosition(int cellSize)
    {
        Vector2 worldPos = (gridPosition + new Vector2(0.5f, 0.5f)) * cellSize;
        return worldPos;
    }
}

