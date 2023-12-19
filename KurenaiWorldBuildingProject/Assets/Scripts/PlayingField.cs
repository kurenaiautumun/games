using UnityEngine;

public class PlayingField
{
    public Vector2Int subGridSize = Vector2Int.one;
    public Vector2Int subGridCount = Vector2Int.one;
    private GameObject[,] cells;
    public Vector2Int size = Vector2Int.one;

    public PlayingField(Vector2Int playingFieldSize, Vector2Int playingFieldCount)
    {
        subGridSize = playingFieldSize;
        subGridCount = playingFieldCount;
        size = new Vector2Int(subGridSize.x * subGridCount.x, subGridSize.y * subGridCount.y);
        cells = new GameObject[size.y, size.x];
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

    public Vector2Int GetSubGrid(int index)
    {
        return new Vector2Int(index % subGridCount.x, index / subGridCount.x);
    }

    public int GetSubGridIndex(Vector2Int pos)
    {
        int yval = pos.y / subGridSize.y;
        int xval = pos.x / subGridSize.x;
        return yval * subGridCount.x + xval;
    }

    public int GetSubGridXIndex(Vector2Int pos)
    {
        return pos.x / subGridSize.x;
    }

    public GameObject GetCellObjectInSubGrid(int index, Vector2Int cellPos)
    {
        Vector2Int offset = GetSubGrid(index);
        return cells[cellPos.x + offset.x, cellPos.y + offset.y];
    }

    public void SetCellObjectInSubGrid(int index, Vector2Int cellPos, GameObject obj)
    {
        Vector2Int offset = GetSubGrid(index);
        cells[cellPos.x + offset.x, cellPos.y + offset.y] = obj;
    }

    public void Clear()
    {
        for(int x=0; x< size.x; x++)
            for(int y=0; y< size.y; y++)
                cells[x,y] = null;
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

    public GameObject this[int index, int row, int col]
    {
        get => GetCellObjectInSubGrid(index, new Vector2Int(row, col));
        set => SetCellObjectInSubGrid(index, new Vector2Int(row, col), value);
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

    public string ToString(int index)
    {
        string s = "";
        for (int i = 0; i < subGridSize.x; i++)
        {
            for (int j = 0; j < subGridSize.y; j++)
                s += (GetCellObjectInSubGrid(index, new Vector2Int(i, j)) == null ? "null" : GetCellObjectInSubGrid(index, new Vector2Int(i, j))) + ", \t";
            s += "\n";
        }
        return s;
    }
}
