using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Using a dictionary would be better if adding more types dynamically
public enum objectType { None, Prop, Character }; 

public class GridObject : MonoBehaviour
{
    //public objectType type = objectType.None;
    public Vector2 gridPosition = Vector2.zero;
    public Vector2 size = Vector2.one;
    public float scale = 1f;
    public GameObject altViewObject = null;
    public int id = -1;
    public objectType type = objectType.None;

    public string GetName()
    {
        int clone = gameObject.name.IndexOf("(");
        if(clone > 0)
            return gameObject.name.Substring(0,clone);
        else
            return gameObject.name;
    }

    public Vector2 GetWorldPosition(int cellSize)
    {
        Vector2 worldPos = (gridPosition + new Vector2(0.5f, 0.5f)) * cellSize;
        return worldPos;
    }

    public Vector2 GetSpriteLowerLeftCorner()
    {
        return gameObject.GetComponent<SpriteRenderer>().sprite.bounds.min * scale;
    }

    public Vector2 GetSpriteUpperRightCorner()
    {
        return gameObject.GetComponent<SpriteRenderer>().sprite.bounds.max * scale;
    }
}

