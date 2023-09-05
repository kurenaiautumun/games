using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.TimeZoneInfo;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

public enum GridInteractionMode {Select, Place, Clear };

[Serializable]
public class LoadableObjects
{
    public int id;
    public int gridPosX;
    public int gridPosY;

    public LoadableObjects(int t, int x, int y) { id = t; gridPosX = x; gridPosY = y; }
    public LoadableObjects(int t, Vector2Int pos) { id = t; gridPosX = pos.x; gridPosY = pos.y; }
    public LoadableObjects(int t, Vector2 pos) { id = t; gridPosX = (int)pos.x; gridPosY = (int)pos.y; }
};

[Serializable]
public class GridData
{
    public int gridNo;
    public List<LoadableObjects> objects = new List<LoadableObjects>();
}

public class SaveData
{
    public int gridCount;
    public List<GridData> data = new List<GridData>();
}

public class GameManagerController : MonoBehaviour
{
    [Header("Game View Section")]
    [SerializeField] private GameObject topDownModePrefab;
    [SerializeField] private GameObject sideModePrefab;
    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private float sideViewScaleMult = 1f;
    [SerializeField] private float sideViewMinScale = 0.2f;
    [SerializeField] private int playingFieldCount = 1;
    [SerializeField] private GameObject sideViewMarker;
    private bool isSideModeEnabled;
    private Vector3 topDownViewCameraPos, sideViewCameraPos;
    private float topDownViewZoom, sideViewZoom;
    private int sideViewActiveLayer;
    private int sideViewActiveGrid;
    private GameObject topDownMode;
    private GameObject sideMode;
    private PlayingField playingField;
    private Grid sideGrid;
    private GameObject[] sideViewMarkers;
    private GameObject sideViewMarkerHierarchy;
    private GameObject[] topDownSubGrids;
    private GameObject[,] sideViewCrossGridObjects;
    private GameObject boundsHierarchy;

    [Header("Grid Section")]
    public float gridCellSize = 1f;
    public Vector2Int playingFieldSubGridSize = Vector2Int.one;
    [SerializeField] private Vector2Int playingFieldOrigin = Vector2Int.zero;
    [SerializeField] private Vector2Int playingFieldGridCount = Vector2Int.one;
    public GridInteractionMode gridInteractionMode = GridInteractionMode.Select;
    [SerializeField] private GameObject gridHighlight;
    [SerializeField] private GameObject boundaryHighlight;


    [Header("Debug Only Section")]
    [SerializeField] bool isDebugEnabled;
    [SerializeField] private GameObject debugCanvasObject;
    [SerializeField] private TMP_Text mouseCoordinateScreenText;
    [SerializeField] private TMP_Text mouseCoordinateWorldText;
    [SerializeField] private TMP_Text gridCoordinateText;
    [SerializeField] private GameObject filler;

    [Header("Transition Section")]
    [SerializeField] private TransitionSettings transition;
    private TransitionManager transitionManager;

    [Header("UI Section")]
    [SerializeField] private GameObject itemPickerDropdownContainer;
    [SerializeField] private GameObject gameUIPanelContainer;
    [SerializeField] private GameObject pausePanelContainer;
    [SerializeField] private TMP_Text activeLayerText, activeGridText;

    [Header("Misc Section")]
    public CameraController cameraController;

    private Vector2 previousTouchLocation;
    private bool hasInputTouchDragged;
    private bool hasInputTouchOverUI;
    private bool isGamePaused;

    void Start()
    {
        if (isDebugEnabled) debugCanvasObject.SetActive(true);
        else debugCanvasObject.SetActive(false);

        if(playingFieldSubGridSize.x <= 0 ||  playingFieldSubGridSize.y <= 0 || playingFieldGridCount.x <= 0 || playingFieldGridCount.y <= 0)
        {
            Debug.LogError("Wrong size for playing field");
            return;
        }
        
        previousTouchLocation = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        CreateEmptyMap();
        
        transitionManager = TransitionManager.Instance();

        hasInputTouchDragged = false;
        hasInputTouchOverUI = false;
        isSideModeEnabled = false;

        topDownViewCameraPos = Camera.main.transform.position;
        sideViewCameraPos = Camera.main.transform.position;
        topDownViewZoom = 6;
        sideViewZoom = 3;

        sideViewActiveLayer = -1;
        sideViewActiveGrid = 0;

        sideViewMarkers = new GameObject[playingField.size.x];
        isGamePaused = false;
        gameUIPanelContainer.SetActive(true);
        pausePanelContainer.SetActive(false);
        topDownMode.SetActive(true);
    }

    void Update()
    {
        if (isGamePaused)
            return;

        DisplayDebugCanvas();
        HandleTouchInput();
    }

    private void DisplayDebugCanvas()
    {
        if (isDebugEnabled)
        {
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            var gridPos = GetGridPosition(mouseWorldPos);

            // Debug display coords
            mouseCoordinateScreenText.SetText("MouseScreenPos: " + mouseScreenPos.ToString());
            mouseCoordinateWorldText.SetText("MouseWorldPos: " + mouseWorldPos.ToString());
            gridCoordinateText.SetText("GridPos: " + gridPos.ToString());
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    hasInputTouchOverUI = true;
                else
                {
                    previousTouchLocation = touch.position;
                    SetHighlight(touch.position, gridInteractionMode == GridInteractionMode.Place);
                }
            }

            else if (touch.phase == TouchPhase.Moved)
            {
                if (!hasInputTouchOverUI)
                {
                    var displacement = touch.position - previousTouchLocation;
                    previousTouchLocation = touch.position;
                    if (displacement.magnitude > 25)
                    {
                        MoveCamera(displacement);
                        hasInputTouchDragged = true;
                    }
                }

            }

            else if (touch.phase == TouchPhase.Ended)
            {
                if (!hasInputTouchOverUI)
                {
                    if (!hasInputTouchDragged)
                    {
                        var touchScreenPos = touch.position;
                        var touchWorldPos = Camera.main.ScreenToWorldPoint(touchScreenPos);
                        var gridPos = GetGridPosition(touchWorldPos);

                       

                        // Top down mode
                        if (!isSideModeEnabled)
                        {
                            if(gridInteractionMode == GridInteractionMode.Clear && !IsValidGridCells(gridPos))
                            {
                                ClearObjectFromWorld(gridPos);
                            }
                            if (!IsValidGridCells(gridPos, gridInteractionMode == GridInteractionMode.Place))
                                SetHighlightColor(Color.red);
                            else
                            {
                                SetHighlightColor(new Color(1, 0.5f, 0));
                                if(gridInteractionMode == GridInteractionMode.Place)
                                    PlaceObjectInWorld(gridPos);
                            }             
                        }

                        // Side mode
                        else
                        {
                            gridPos = sideGrid.WorldToCell(touchWorldPos);
                            if ((gridPos.x - playingFieldOrigin.x) < (playingField.size.x * playingFieldCount))
                                gridPos.x = ((gridPos.x - playingFieldOrigin.x) % playingField.size.x) + playingFieldOrigin.x;

                            gridPos.y = sideViewActiveLayer + playingFieldOrigin.y;

                            if (gridInteractionMode == GridInteractionMode.Clear && !IsValidGridCells(gridPos))
                            {
                                ClearObjectFromWorld(gridPos);
                            }
                            if (!IsValidGridCells(gridPos, gridInteractionMode == GridInteractionMode.Place))
                                SetHighlightColor(Color.red);
                            else
                            {
                                SetHighlightColor(new Color(1, 0.5f, 0));
                                if (gridInteractionMode == GridInteractionMode.Place)
                                    PlaceObjectInWorld(gridPos);
                            }
                        }
                    }
                }
                hasInputTouchOverUI = false;
                hasInputTouchDragged = false;
            }
        }

    }

    // Converts a world position into grid position
    private Vector3 GetGridPosition(Vector3 worldPos)
    {
        return new Vector3(Mathf.Floor(worldPos.x / gridCellSize), Mathf.Floor(worldPos.y / gridCellSize), 0);
    }

    // Sets the position and size of the higlighter (default: uses the item selected in the dropdown menu for size)
    private void SetHighlight(Vector3 screenPos, bool useDropdownItem = true)
    {
        var item = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetItem();
        var gridSize = Vector2.one;
        if (item != null && useDropdownItem)
            gridSize = item.gameObject.GetComponent<GridObject>().size;

        gridHighlight.transform.position = GetGridPosition(Camera.main.ScreenToWorldPoint(screenPos)) * gridCellSize;
        gridHighlight.GetComponent<LineRenderer>().SetPosition(1, Vector3.right * gridCellSize * gridSize.x);
        gridHighlight.GetComponent<LineRenderer>().SetPosition(2, new Vector3(gridSize.x, gridSize.y, 0) * gridCellSize);
        gridHighlight.GetComponent<LineRenderer>().SetPosition(3, Vector3.up * gridCellSize * gridSize.y);
    }

    // Sets the position and size of the game boundary
    private void SetBoundary(Vector3 screenPos)
    {
        // Main boundary
        boundaryHighlight.transform.position = ( /*GetGridPosition(Camera.main.ScreenToWorldPoint(screenPos)) +*/ new Vector3(playingFieldOrigin.x, playingFieldOrigin.y, 0)) * gridCellSize;
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(1, Vector3.right * gridCellSize * playingField.size.x);
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(2, new Vector3(playingField.size.x, playingField.size.y, 0) * gridCellSize);
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(3, Vector3.up * gridCellSize * playingField.size.y);

        // Sub grid boundaries
        boundsHierarchy = new GameObject("Bounds");

        for(int x = 0; x < playingFieldGridCount.x; x++)
        {
            for(int y = 0; y < playingFieldGridCount.y; y++)
            {
                var subgridBound = Instantiate(boundaryHighlight);
                subgridBound.name = "SubgridBound" + x + "," + y;
                subgridBound.transform.position = new Vector3(x * playingFieldSubGridSize.x + playingFieldOrigin.x, y * playingFieldSubGridSize.y + playingFieldOrigin.y, 0) * gridCellSize;
                subgridBound.transform.SetParent(boundsHierarchy.transform);

                LineRenderer line = subgridBound.GetComponent<LineRenderer>();
                line.startColor = Color.white;
                line.endColor = Color.cyan;
                line.SetPosition(1, Vector3.right * gridCellSize * playingFieldSubGridSize.x);
                line.SetPosition(2, new Vector3(playingFieldSubGridSize.x, playingFieldSubGridSize.y, 0) * gridCellSize);
                line.SetPosition(3, Vector3.up * gridCellSize * playingFieldSubGridSize.y);

            }
        }
    }

    // Set the colour of the highlighter
    private void SetHighlightColor(Color color)
    {
        gridHighlight.GetComponent<LineRenderer>().startColor = color;
        gridHighlight.GetComponent<LineRenderer>().endColor = color;
    }

    // Place selected item into both top view and side view (separate GameObjects but linked via GridObject)
    private void PlaceObjectInWorld(Vector3 gridPos, GameObject item = null)
    {
        if(item == null)
            item = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetItem();
        int itemID = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetCurrentIndex();
            
        var size = Vector2.one;
        if(item.TryGetComponent<GridObject>(out var component))
            size = component.size;
        Vector2Int pos = Vector2Int.RoundToInt(gridPos) - playingFieldOrigin;

        // Top Down View
        int gridIndex = playingField.GetSubGridIndex(pos);
        Vector3 sizeOffset = (Vector3)(size - Vector2.one) * 0.5f;
        var gridObject = Instantiate(item, (gridPos + new Vector3(0.5f, 0.5f, 0) + sizeOffset ) * gridCellSize, Quaternion.identity);
        gridObject.transform.SetParent(topDownMode.transform);
        gridObject.GetComponent<GridObject>().gridPosition = gridPos;
        gridObject.GetComponent<GridObject>().id = itemID;
        gridObject.transform.localScale = size;

        bool isCrossGrid = false;

        for(int x = 0; x < size.x; x++)
        {
            if(gridIndex != playingField.GetSubGridIndex(pos + new Vector2Int(x,0)))
            {
                isCrossGrid = true;
            }
                
            for(int y = 0; y < size.y; y++)
            {
                playingField[y + pos.y, x + pos.x] = gridObject;
            } 
        }

        // Side View
        var sideViewScale = GetSideViewWorldScale(playingField.size.y - pos.y);
        var sideGridPos = sideGrid.WorldToCell(new Vector3(gridPos.x, gridPos.y/2, gridPos.y)) + new Vector3((item.GetComponent<GridObject>().size.x - 1) * 0.5f + sideViewActiveGrid * playingField.size.x, - item.GetComponent<GridObject>().GetSpriteLowerLeftCorner().y * sideViewScale, 0);

        var sideObject = Instantiate(item, sideGridPos, Quaternion.identity);
        sideObject.GetComponent<SpriteRenderer>().sortingOrder = (int)-gridPos.y;
        sideObject.transform.localScale = Vector3.one * sideViewScale * component.scale;

        gridObject.GetComponent<GridObject>().altViewObject = sideObject;

        if (isCrossGrid)
        {
            sideViewCrossGridObjects[gridIndex % playingFieldGridCount.x, pos.y] = sideObject;
            sideObject.transform.SetParent(sideMode.transform);
        }
        else
            sideObject.transform.SetParent(topDownSubGrids[gridIndex].transform);

        if (isSideModeEnabled)
        {
            sideObject.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            for (int x = 0; x < size.x; x++)
            {
                Destroy(sideViewMarkers[x + pos.x]);
                sideViewMarkers[x + pos.x] = null; 
            }
        }
    }

    // Clears the selected grid object from both top view and side view
    private void ClearObjectFromWorld(Vector3 gridPos)
    {
        Vector2Int pos = Vector2Int.RoundToInt(gridPos) - playingFieldOrigin;
        if (pos.x >= playingField.size.x || pos.x < 0 ||
            pos.y >= playingField.size.y || pos.y < 0)
            return;

        var item = playingField[pos.y, pos.x];
        if (item == null)
            return;

        var size = item.GetComponent<GridObject>().size;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                playingField[y + pos.y, x + pos.x] = null;
            }
        }

        Destroy(item.GetComponent<GridObject>().altViewObject);
        Destroy(item);
    }

    // Moves the camera (limits it to within the bounds of the playing field)
    private void MoveCamera(Vector2 displacement)
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos -= new Vector3(displacement.x, displacement.y, 0) * Time.deltaTime;

        if(isSideModeEnabled)
        {
            if (camPos.x > -playingFieldOrigin.x * gridCellSize * playingFieldCount)
                camPos.x = -playingFieldOrigin.x * gridCellSize * playingFieldCount;
            else if (camPos.x < playingFieldOrigin.x * gridCellSize)
                camPos.x = playingFieldOrigin.x * gridCellSize;
        }
        else
        {
            if (camPos.x > -playingFieldOrigin.x * gridCellSize)
                camPos.x = -playingFieldOrigin.x * gridCellSize;
            else if (camPos.x < playingFieldOrigin.x * gridCellSize)
                camPos.x = playingFieldOrigin.x * gridCellSize;
        }

        if (camPos.y > -playingFieldOrigin.y * gridCellSize)
            camPos.y = -playingFieldOrigin.y * gridCellSize;
        else if (camPos.y < playingFieldOrigin.y * gridCellSize)
            camPos.y = playingFieldOrigin.y * gridCellSize;

        Camera.main.transform.position = camPos;
    }

    // Returns the world position in side view for a given gridPos
    private Vector3 GetSideViewWorldPosition(Vector3 gridPos)
    {
        return sideGrid.WorldToCell(new Vector3(gridPos.x, gridPos.y / 2, gridPos.y)) + new Vector3(-2f, -1.5f, 0);
    }

    // Returns the scale of object for side view depending on their layer (y position)
    private float GetSideViewWorldScale(float yPos)
    {
        return Mathf.Lerp(sideViewMinScale, 1f, yPos / playingField.size.y);
    }
    
    // Makes the layer that we want to edit to be active in side view (invalid values will reset)
    private void SetCurrentSideViewLayer(int layer)
    {
        if(sideViewActiveLayer != -1)
            ClearPreviousActiveLayer(sideViewActiveLayer);

        sideViewActiveLayer = layer;
        if (layer < 0 || layer >= playingField.size.y || !isSideModeEnabled)
        {
            layer = -1;
            SetCurrentSubGridView(0);
            ResetSideViewShadings();
            return;
        }
        SetSideViewShadings(sideViewActiveLayer);
        SetCurrentSubGridView(sideViewActiveGrid);
        activeLayerText.SetText("Layer: "+layer.ToString());
    }

    // Clears the previous active layer
    private void ClearPreviousActiveLayer(int focusedLayer)
    {
        for (int x = 0; x < playingField.size.x; x++)
        {
            // Remove markers if they are present
            sideViewMarkers[x] = null;

            var currentItem = playingField[focusedLayer, x];
            if (currentItem == null)
                continue;

            // if currentItem doesn't have GridObject (for some reason) we clear it else we edit the side view's object's sorting layer to be default
            if (currentItem.GetComponent<GridObject>() == null)
                playingField[focusedLayer, x] = null;
            else
            {
                var sideObject = currentItem.GetComponent<GridObject>().altViewObject;
                sideObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            }    
        }

        while (sideViewMarkerHierarchy.transform.childCount > 0)
            DestroyImmediate(sideViewMarkerHierarchy.transform.GetChild(0).gameObject);
    }

    // Edits the sprites in side view to differentiate the focused layer from others
    private void SetSideViewShadings(int focusedLayer)
    {
        for (int y = playingField.size.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < playingField.size.x; x++)
            {
                var currentItem = playingField[y, x];

                // We check if the currentItem's origin is on another layer instead of the current one or if it is null
                if (currentItem == null || (currentItem.GetComponent<GridObject>().gridPosition.y - playingFieldOrigin.y) != y)
                    continue;

                // Apply color and transparency according to how far from the focused layer
                var altItem = currentItem.GetComponent<GridObject>().altViewObject;
                float shading = (playingField.size.y - Mathf.Abs(focusedLayer - y)) / (float)playingField.size.y;
                altItem.GetComponent<SpriteRenderer>().color = new Color(shading, shading, shading, shading);
                altItem.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

                x += (int)altItem.GetComponent<GridObject>().size.x - 1;
            }
        }

        for(int x = 0; x < playingField.size.x; x++)
        {
            var currentItem = playingField[focusedLayer, x];

            if(currentItem == null)
            {
                //Create a marker/highlight over this position to indicate that there is available space

                var pos = GetSideViewWorldPosition(new Vector3(x, focusedLayer, 0)) + new Vector3(-0.5f + sideViewActiveGrid * playingField.size.x, -0.5f, 0);
                sideViewMarkers[x] = Instantiate(sideViewMarker, pos, Quaternion.identity);
                sideViewMarkers[x].transform.SetParent(sideViewMarkerHierarchy.transform);
            }

            else
            {
                // This layer will he in the fore front

                var altItem = currentItem.GetComponent<GridObject>().altViewObject;

                altItem.GetComponent<SpriteRenderer>().color = Color.white;
                altItem.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";

                x += (int)altItem.GetComponent<GridObject>().size.x - 1;
            }
        }

        darkOverlay.SetActive(true);
    }

    // Sets the scaling of side view objects based on their grid's positioning
    private void SetSideViewScalings(int startingYPos)
    {
        for (int y = playingField.size.y - 1; y >= startingYPos; y--)
        {
            for (int x = 0; x < playingField.size.x; x++)
            {
                var currentItem = playingField[y, x];

                // We check if the currentItem's origin is on another layer instead of the current one or if it is null
                if (currentItem == null || (currentItem.GetComponent<GridObject>().gridPosition.y - playingFieldOrigin.y) != y)
                    continue;

                // Apply new scaling
                var component = currentItem.GetComponent<GridObject>();
                var altItem = component.altViewObject;
                float yPos = playingField.size.y - y + startingYPos;
                float sideViewScale = Mathf.Lerp(sideViewMinScale, 1f, yPos / playingField.size.y);
                altItem.transform.localScale = Vector3.one * sideViewScale * component.scale;

                x += (int)altItem.GetComponent<GridObject>().size.x - 1;
            }
        }
    }

    // Reset the side view shadings (make them solid)
    private void ResetSideViewShadings()
    {
        for (int y = playingField.size.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < playingField.size.x; x++)
            {
                var currentItem = playingField[y, x];

                // We check if the currentItem's origin is on another layer instead of the current one or if it is null
                if (currentItem == null || (currentItem.GetComponent<GridObject>().gridPosition.y - playingFieldOrigin.y) != y)
                    continue;

                // Apply color and transparency according to how far from the focused layer
                var altItem = currentItem.GetComponent<GridObject>().altViewObject;
                altItem.GetComponent<SpriteRenderer>().color = Color.white;
                altItem.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

                x += (int)altItem.GetComponent<GridObject>().size.x - 1;
            }
        }

        for (int x = 0; x < playingField.size.x; x++)
            sideViewMarkers[x] = null;

        while (sideViewMarkerHierarchy.transform.childCount > 0)
            DestroyImmediate(sideViewMarkerHierarchy.transform.GetChild(0).gameObject);

        darkOverlay.SetActive(false);
    }

    // Sets which grid is being used
    private void SetCurrentSubGridView(int grid)
    {
        if (isSideModeEnabled && sideViewActiveLayer >= 0)
            SetSideViewShadings(sideViewActiveLayer);

        for (int i = grid; i < playingFieldCount; i += playingFieldGridCount.x)
        {
            topDownSubGrids[i].SetActive(true);
        }

        int idx = grid % playingFieldGridCount.x;
        int idy = grid * playingFieldSubGridSize.y / playingFieldGridCount.x;

        if (idx == 0)
        {
            for (int i = idy; i < playingField.size.y; i++)
            {
                var obj = sideViewCrossGridObjects[idx, i];
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        else if (idx == playingFieldGridCount.x - 1)
        {
            --idx;
            for (int i = idy; i < playingField.size.y; i++)
            {
                var obj = sideViewCrossGridObjects[idx, i];
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        else
        {
            for (int i = idy; i < playingField.size.y; i++)
            {
                var obj = sideViewCrossGridObjects[idx - 1, i];
                if (obj != null)
                    obj.SetActive(true);
                obj = sideViewCrossGridObjects[idx, i];
                if (obj != null)
                    obj.SetActive(true);
            }
        }

        if(isSideModeEnabled)
        {            
            var gridPos = playingField.GetSubGrid(grid);
            var worldPos = new Vector3(gridPos.x * gridCellSize, gridPos.y * gridCellSize, Camera.main.transform.position.z);
            worldPos = sideGrid.CellToWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
            worldPos.z = Camera.main.transform.position.z;
            Camera.main.transform.position = worldPos;

            SetSideViewScalings(gridPos.y * playingFieldSubGridSize.y);
        }

        activeGridText.SetText("Grid: "+grid.ToString());
    }

    // Deactivates the objects of the previous sub grid
    private void ClearPreviousSubGridView(int grid)
    {
        for (int i = grid; i < playingFieldCount; i += playingFieldGridCount.x)
        {
            topDownSubGrids[i].SetActive(false);
        }

        string s = "";
        for (int i = 0; i < playingFieldGridCount.x - 1; i++)
        {
            for (int y = 0; y < playingField.size.y; y++)
            {
                var o = sideViewCrossGridObjects[i, y];
                s += (o == null ? "null" : o.name) + ", \t";
            }
            s += "\n";
        }

        int idx = grid % playingFieldGridCount.x;
        int idy = grid * playingFieldSubGridSize.y / playingFieldGridCount.x;

        if (idx == 0)
        {
            for (int i = idy; i < playingField.size.y; i++)
            {
                var obj = sideViewCrossGridObjects[idx, i];
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        else if (idx == playingFieldGridCount.x - 1)
        {
            --idx;
            for (int i = idy; i < playingField.size.y; i++)
            {
                var obj = sideViewCrossGridObjects[idx, i];
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        else
        {
            for (int i = idy; i < playingField.size.y; i++)
            {
                var obj = sideViewCrossGridObjects[idx - 1, i];
                if (obj != null)
                    obj.SetActive(false);
                obj = sideViewCrossGridObjects[idx, i];
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        if (!isSideModeEnabled || sideViewActiveLayer < 0)
            return;

        for (int x = 0; x < playingField.size.x; x++)
            sideViewMarkers[x] = null;

        while (sideViewMarkerHierarchy.transform.childCount > 0)
            DestroyImmediate(sideViewMarkerHierarchy.transform.GetChild(0).gameObject);
    }

    // Checks if cell(s) already contain an item (default: uses the dropdown menu item to determine all the cells that the item will occupy)
    private bool IsValidGridCells(Vector3 gridPos, bool useDropdownItem = true)
    {
        Vector2Int pos = Vector2Int.RoundToInt(gridPos) - playingFieldOrigin;

        if (pos.x >= playingField.size.x || pos.x < 0 ||
            pos.y >= playingField.size.y || pos.y < 0)
            return false;

        var selection = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetItem();

        var size = Vector2.one;
        if(selection != null && useDropdownItem)
            size = selection.GetComponent<GridObject>().size;

        if (pos.x + size.x > playingField.size.x || pos.y + size.y > playingField.size.y)
            return false;
        

        for (int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                if (playingField[y + pos.y, x + pos.x] != null)
                    return false;
            }
        }

        return true;
    }

    ////////////////////////////////////////////
    ///// General Map Operations
    ////////////////////////////////////////////

    private void CreateEmptyMap()
    {
        playingField = new PlayingField(playingFieldSubGridSize, playingFieldGridCount);
        playingFieldCount = playingFieldGridCount.x * playingFieldGridCount.y;
        playingFieldOrigin = -playingField.size / 2;

        topDownMode = Instantiate(topDownModePrefab, Vector3.zero, Quaternion.identity);
        topDownMode.SetActive(false);
        gridHighlight = topDownMode.transform.GetChild(1).gameObject;
        boundaryHighlight = topDownMode.transform.GetChild(2).gameObject;
        SetHighlight(previousTouchLocation, false);
        SetBoundary(previousTouchLocation);

        sideMode = Instantiate(sideModePrefab, Vector3.zero, Quaternion.identity);
        sideMode.SetActive(false);
        sideGrid = sideMode.GetComponentInChildren<Grid>();
        sideViewMarkerHierarchy = new GameObject("Markers");
        sideViewMarkerHierarchy.transform.SetParent(sideMode.transform);

        topDownSubGrids = new GameObject[playingFieldCount];
        for (int i = 0; i < playingFieldCount; i++)
        {
            topDownSubGrids[i] = new GameObject("SubGrid" + i);
            topDownSubGrids[i].transform.SetParent(sideMode.transform);
            topDownSubGrids[i].SetActive(false);
        }

        sideViewCrossGridObjects = new GameObject[playingFieldGridCount.x - 1, playingField.size.y];

        SetCurrentSubGridView(0);
    }

    private void SaveCurrentMap(string savePath)
    {
        string jsonString = "";
        SaveData saveData = new SaveData();
        saveData.gridCount = playingFieldCount;

        for (int field = 0; field < playingFieldCount; field++)
        {
            GridData gridData = new GridData();
            gridData.gridNo = field;

            for (int x = 0; x < playingField.size.x; x++)
            {
                for (int y = 0; y < playingField.size.y; y++)
                {
                    var item = playingField[y, x];
                    if (item != null)
                    {
                        var component = item.GetComponent<GridObject>();
                        Vector2Int pos = Vector2Int.FloorToInt(component.gridPosition) - playingFieldOrigin;

                        // Ignore multi-cell objects
                        if (pos.x != x || pos.y != y)
                            continue;

                        LoadableObjects saveObject = new LoadableObjects(component.id, pos);
                        gridData.objects.Add(saveObject);      
                    }
                }
            }
            saveData.data.Add(gridData);
        }
        
        jsonString += JsonUtility.ToJson(saveData);
        savePath = "E:/test.json";
        StreamWriter writer = new StreamWriter(savePath, false);
        writer.WriteLine(jsonString);
        writer.Close();
    }

    private void ClearCurrentMap()
    {
        Destroy(topDownMode);
        Destroy(sideMode);

        playingFieldCount = 0;
    }

    private void LoadNewMap(string mapFile)
    {
        string jsonString = File.ReadAllText(mapFile);
        var saveData = JsonUtility.FromJson<SaveData>(jsonString);
        playingFieldCount = saveData.gridCount;

        // Create new map
        CreateEmptyMap();

        // Populate map
        foreach (var grid in saveData.data)
        {
            //topViewActiveGrid = grid.gridNo;
            foreach(var item in grid.objects)
            {
                Vector3 gridPos = new Vector3(item.gridPosX, item.gridPosY);
                GameObject prefab = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetItem(item.id);
                if (prefab != null)
                    PlaceObjectInWorld(gridPos, prefab);
                else
                    Debug.LogError("Item Type is invalid!");
            }
        }

        sideViewActiveGrid = 0;
        topDownMode.SetActive(true);
    }

    ////////////////////////////////////////////
    ///// UI Operations
    ////////////////////////////////////////////

    public void SwitchViewMode()
    {
        transitionManager.onTransitionCutPointReached += OnTransitionEndSwitchViewMode;
        transitionManager.Transition(transition, 0f);
    }

    public void PickItem()
    {
        TMP_Dropdown dropdown = itemPickerDropdownContainer.GetComponent<TMP_Dropdown>();

        SetHighlight(previousTouchLocation);
    }

    public void SwitchNextLayer()
    {
        if (gridInteractionMode == GridInteractionMode.Select)
            return;

        sideViewActiveLayer++;
        if(sideViewActiveLayer >= playingField.size.y)
            sideViewActiveLayer = 0;
        SetCurrentSideViewLayer(sideViewActiveLayer);
    }

    public void SwitchPreviousLayer()
    {
        if (gridInteractionMode == GridInteractionMode.Select)
            return;

        sideViewActiveLayer--;
        if (sideViewActiveLayer < 0)
            sideViewActiveLayer = playingField.size.y - 1;
        SetCurrentSideViewLayer(sideViewActiveLayer);
    }

    public void SwitchNextGrid()
    {
        ClearPreviousSubGridView(sideViewActiveGrid);
        if(++sideViewActiveGrid >= playingFieldCount)
            sideViewActiveGrid = 0;
        SetCurrentSubGridView(sideViewActiveGrid);
    }

    public void SwitchPreviousGrid()
    {
        ClearPreviousSubGridView(sideViewActiveGrid);
        if (--sideViewActiveGrid < 0)
            sideViewActiveGrid = playingFieldCount - 1;
        SetCurrentSubGridView(sideViewActiveGrid);
    }

    public void SaveMap()
    {
        SaveCurrentMap("");
        ResumeGame();
    }

    public void LoadMap()
    {
        ClearCurrentMap();
        LoadNewMap("");
        ResumeGame();
    }

    public void NewMap()
    {
        ClearCurrentMap();
        CreateEmptyMap();
        ResumeGame();
    }

    public void PauseGame()
    {
        isGamePaused = true;
        gameUIPanelContainer.SetActive(false);
        pausePanelContainer.SetActive(true);
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        gameUIPanelContainer.SetActive(true);
        pausePanelContainer.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        transitionManager.Transition("MainMenuScene", transition, 0f);
    }

    public void SwitchInteractionMode(GameObject buttonGameObject)
    {
        TMP_Text text = buttonGameObject.GetComponentInChildren<TMP_Text>();

        switch(gridInteractionMode)
        {
            case GridInteractionMode.Select: gridInteractionMode = GridInteractionMode.Place;
                text.SetText("Place");
                SetHighlight(previousTouchLocation);
                if (isSideModeEnabled)
                    SetCurrentSideViewLayer(0);

                break;

            case GridInteractionMode.Place: gridInteractionMode = GridInteractionMode.Clear;
                text.SetText("Clear");
                SetHighlight(previousTouchLocation, false);
                if (isSideModeEnabled)
                    SetCurrentSideViewLayer(0);
                break;

            case GridInteractionMode.Clear: gridInteractionMode = GridInteractionMode.Select;
                text.SetText("Select");
                SetHighlight(previousTouchLocation, false);
                if (isSideModeEnabled)
                    SetCurrentSideViewLayer(-1);
                break;

            default: gridInteractionMode = GridInteractionMode.Select;
                text.SetText("Select");
                SetHighlight(previousTouchLocation, false);
                if (isSideModeEnabled)
                    SetCurrentSideViewLayer(-1);
                break;
        }
    }

    private void OnTransitionEndSwitchViewMode()
    {
        if (isSideModeEnabled)
        {
            isSideModeEnabled = false;
            sideMode.SetActive(false);
            topDownMode.SetActive(true);
            sideViewCameraPos = Camera.main.transform.position;
            Camera.main.transform.position = topDownViewCameraPos;
            Camera.main.orthographicSize = topDownViewZoom;

            boundsHierarchy.SetActive(true);
            cameraController.EnableGridLines();

            SetCurrentSideViewLayer(-1);
        }
        else
        {
            isSideModeEnabled = true;
            sideMode.SetActive(true);
            topDownMode.SetActive(false);
            topDownViewCameraPos = Camera.main.transform.position;
            Camera.main.transform.position = sideViewCameraPos;
            Camera.main.orthographicSize = sideViewZoom;

            boundsHierarchy.SetActive(false);
            cameraController.DisableGridLines();

            if(gridInteractionMode == GridInteractionMode.Place || gridInteractionMode == GridInteractionMode.Clear)
                SetCurrentSideViewLayer(0);

            SetCurrentSubGridView(sideViewActiveGrid);
        }
        transitionManager.onTransitionCutPointReached -= OnTransitionEndSwitchViewMode;
    }
}
