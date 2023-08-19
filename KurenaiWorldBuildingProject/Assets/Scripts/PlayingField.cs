using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayingField
{
    public Vector2Int size = Vector2Int.one;
    private GameObject[,] cells;

    public PlayingField(Vector2Int playingFieldSize)
    {
        size = playingFieldSize;
        cells = new GameObject[size.x, size.y];
    }

    public PlayingField(int x, int y)
    {
        size = new Vector2Int(x, y);
        cells = new GameObject[size.x, size.y];
    }

    public bool IsValidCell(Vector2Int cellPos)
    {
        if(cellPos.x >= size.x || cellPos.y >= size.y || cellPos.x < 0 || cellPos.y < 0)
            return false;
        return true;
    }

    public bool IsCellOccupied(Vector2Int cellPos)
    {
        return cells[cellPos.x, cellPos.y] != null;
    }

    public void SetCellObject(Vector2Int cellPos, GameObject obj)
    {
        cells[cellPos.x, cellPos.y] = obj;
    }

    public GameObject GetCellObject(Vector2Int cellPos)
    {
        return cells[cellPos.x, cellPos.y];
    }

    public GameObject[] GetRow(int row)
    {
        GameObject[] rowObjects = new GameObject[row];
        for(int i = 0; i < row; i++)
            rowObjects[i] = cells[row, i].gameObject;
        return rowObjects;
    }

    public GameObject[] this[int row]
    {
        get => GetRow(row);
    }

    public GameObject this[int row, int col]
    {
        get => cells[row, col];
        set => cells[row, col] = value;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
                s += (cells[i, j] == null ? "null": cells[i,j]) + ", \t";
            s += "\n";
        }
        return s;
    }
}
