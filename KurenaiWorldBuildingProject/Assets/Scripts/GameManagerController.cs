using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

public enum GridInteractionMode {Select, Place, Clear };

public class GameManagerController : MonoBehaviour
{
    [Header("Game View Section")]
    [SerializeField] private GameObject topDownMode;
    [SerializeField] private GameObject sideMode;
    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private float sideViewScaleMult = 1f;
    private bool isSideModeEnabled;
    private Vector3 topDownViewCameraPos, sideViewCameraPos;
    private float topDownViewZoom, sideViewZoom;
    private int sideViewActiveLayer;

    [Header("Grid Section")]
    public float gridCellSize = 1f;
    public Vector2Int playingFieldSize = Vector2Int.one;
    public GridInteractionMode gridInteractionMode = GridInteractionMode.Select;
    [SerializeField] private GameObject gridHighlight;
    [SerializeField] private GameObject boundaryHighlight;
    [SerializeField] private Vector2Int playingFieldOrigin = Vector2Int.zero;


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

    [Header("Misc Section")]
    public CameraController cameraController;

    private Vector2 previousTouchLocation;
    private bool hasInputTouchDragged;
    private bool hasInputTouchOverUI;

    private PlayingField playingField;

    private Grid sideGrid;

    void Start()
    {
        if (isDebugEnabled)
            debugCanvasObject.SetActive(true);
        else
            debugCanvasObject.SetActive(false);

        if(playingFieldSize.x <= 0 ||  playingFieldSize.y <= 0)
        {
            Debug.LogError("Wrong size for playing field");
            return;
        }
        playingFieldOrigin = -playingFieldSize / 2;

        topDownMode.SetActive(true);
        sideMode.SetActive(false);
        isSideModeEnabled = false;

        transitionManager = TransitionManager.Instance();

        previousTouchLocation = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        SetHighlight(previousTouchLocation, false);
        SetBoundary(previousTouchLocation);

        previousTouchLocation = Vector2.zero;
        hasInputTouchDragged = false;
        hasInputTouchOverUI = false;

        playingField = new PlayingField(playingFieldSize.y, playingFieldSize.x);

        topDownViewCameraPos = Camera.main.transform.position;
        sideViewCameraPos = Camera.main.transform.position;
        topDownViewZoom = 6;
        sideViewZoom = 3;

        sideViewActiveLayer = -1;

        sideGrid = sideMode.GetComponentInChildren<Grid>();
    }

    void Update()
    {
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
                        else
                        {
                            gridPos = sideGrid.WorldToCell(touchWorldPos);
                            gridPos.y = sideViewActiveLayer + playingFieldOrigin.y;

                            Debug.Log(gridPos + ", " + IsValidGridCells(gridPos));

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
        boundaryHighlight.transform.position = (GetGridPosition(Camera.main.ScreenToWorldPoint(screenPos)) + new Vector3(playingFieldOrigin.x, playingFieldOrigin.y, 0)) * gridCellSize;
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(1, Vector3.right * gridCellSize * playingFieldSize.x);
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(2, new Vector3(playingFieldSize.x, playingFieldSize.y, 0) * gridCellSize);
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(3, Vector3.up * gridCellSize * playingFieldSize.y);
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
        var size = Vector2.one;
        if(item.TryGetComponent<GridObject>(out var component))
            size = component.size;
        Vector2Int pos = Vector2Int.RoundToInt(gridPos) - playingFieldOrigin;

        // Top Down View
        Vector3 sizeOffset = (Vector3)(size - Vector2.one) * 0.5f;
        var gridObject = Instantiate(item, (gridPos + new Vector3(0.5f, 0.5f, 0) + sizeOffset ) * gridCellSize, Quaternion.identity);
        gridObject.transform.SetParent(topDownMode.transform);
        gridObject.GetComponent<GridObject>().gridPosition = gridPos;
        gridObject.transform.localScale = size;

        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                playingField[y + pos.y, x + pos.x] = gridObject;
            } 
        }

        // Side View
        var sideGridPos = sideGrid.WorldToCell(new Vector3(gridPos.x, gridPos.y/2, gridPos.y)) + new Vector3((item.GetComponent<GridObject>().size.x - 1) * 0.5f, - item.GetComponent<GridObject>().GetSpriteLowerLeftCorner().y, 0);

        var sideObject = Instantiate(item, new Vector3(gridPos.x + (item.GetComponent<GridObject>().size.x - 1) * 0.5f , gridPos.y * 0.5f - item.GetComponent<GridObject>().GetSpriteLowerLeftCorner().y, gridPos.y), Quaternion.identity);
        sideObject.transform.position = sideGridPos;
        sideObject.transform.SetParent(sideMode.transform);
        sideObject.GetComponent<SpriteRenderer>().sortingOrder = (int)-gridPos.y;

        gridObject.GetComponent<GridObject>().altViewObject = sideObject;

        if(isSideModeEnabled)
        {
            sideObject.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";

        }
    }

    // Clears the selected grid object from both top view and side view
    private void ClearObjectFromWorld(Vector3 gridPos)
    {
        Vector2Int pos = Vector2Int.RoundToInt(gridPos) - playingFieldOrigin;
        if (pos.x >= playingFieldSize.x || pos.x < 0 ||
            pos.y >= playingFieldSize.y || pos.y < 0)
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

        if (camPos.x > -playingFieldOrigin.x * gridCellSize)
            camPos.x = -playingFieldOrigin.x * gridCellSize;
        else if (camPos.x < playingFieldOrigin.x * gridCellSize)
            camPos.x = playingFieldOrigin.x * gridCellSize;

        if (camPos.y > -playingFieldOrigin.y * gridCellSize)
            camPos.y = -playingFieldOrigin.y * gridCellSize;
        else if (camPos.y < playingFieldOrigin.y * gridCellSize)
            camPos.y = playingFieldOrigin.y * gridCellSize;

        Camera.main.transform.position = camPos;
    }

    private Vector3 GetSideViewWorldPosition(Vector3 gridPos)
    {
        return sideGrid.WorldToCell(new Vector3(gridPos.x, gridPos.y / 2, gridPos.y)) + new Vector3(-1f, -1.5f, 0);
    }
    
    private void SetCurrentSideViewLayer(int layer)
    {
        if(sideViewActiveLayer != -1)
            ClearPreviousActiveLayer(sideViewActiveLayer);

        sideViewActiveLayer = layer;
        if (layer < 0 || layer >= playingFieldSize.y)
        {
            layer = -1;
            darkOverlay.SetActive(false);
            return;
        }    
        SetSideViewShadings(sideViewActiveLayer);
    }

    private void ClearPreviousActiveLayer(int focusedLayer)
    {
        for (int x = 0; x < playingFieldSize.x; x++)
        {
            var currentItem = playingField[focusedLayer, x];
            if (currentItem == null)
                continue;

            if (currentItem.GetComponent<GridObject>() == null)
                playingField[focusedLayer, x] = null;
        }
    }

    private void SetSideViewShadings(int focusedLayer)
    {
        for (int y = playingFieldSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < playingFieldSize.x; x++)
            {
                var currentItem = playingField[y, x];

                if (currentItem == null || (currentItem.GetComponent<GridObject>().gridPosition.y - playingFieldOrigin.y) != y)
                    continue;

                var altItem = currentItem.GetComponent<GridObject>().altViewObject;
                float shading = (playingFieldSize.y - Mathf.Abs(focusedLayer - y)) / (float)playingFieldSize.y;
                altItem.GetComponent<SpriteRenderer>().color = new Color(shading, shading, shading, shading);
                altItem.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

                x += (int)altItem.GetComponent<GridObject>().size.x - 1;
            }
        }

        for(int x = 0; x < playingFieldSize.x; x++)
        {
            var currentItem = playingField[focusedLayer, x];

            if(currentItem == null)
            {
                var pos = GetSideViewWorldPosition(new Vector3(x, focusedLayer, 0));

                // TODO: Create a marker/highlight over this position to indicate that there is available space

                //var empty = Instantiate(filler, pos, Quaternion.identity);
                //empty.transform.SetParent(sideMode.transform);
            }

            else
            {
                var altItem = currentItem.GetComponent<GridObject>().altViewObject;

                altItem.GetComponent<SpriteRenderer>().color = Color.white;
                altItem.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";

                x += (int)altItem.GetComponent<GridObject>().size.x - 1;
            }
        }

        darkOverlay.SetActive(true);
    }

    // Checks if cell(s) already contain an item (default: uses the dropdown menu item to determine all the cells that the item will occupy)
    private bool IsValidGridCells(Vector3 gridPos, bool useDropdownItem = true)
    {
        Vector2Int pos = Vector2Int.RoundToInt(gridPos) - playingFieldOrigin;

        if (pos.x >= playingFieldSize.x || pos.x < 0 ||
            pos.y >= playingFieldSize.y || pos.y < 0)
            return false;

        var selection = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetItem();

        var size = Vector2.one;
        if(selection != null && useDropdownItem)
            size = selection.GetComponent<GridObject>().size;

        if (pos.x + size.x > playingFieldSize.x || pos.y + size.y > playingFieldSize.y)
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
        sideViewActiveLayer++;
        if(sideViewActiveLayer >= playingFieldSize.y)
            sideViewActiveLayer = 0;
        SetCurrentSideViewLayer(sideViewActiveLayer);
    }

    public void SwitchPreviousLayer()
    {
        sideViewActiveLayer--;
        if (sideViewActiveLayer < 0)
            sideViewActiveLayer = playingFieldSize.y - 1;
        SetCurrentSideViewLayer(sideViewActiveLayer);
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

            cameraController.DisableGridLines();

            if(gridInteractionMode == GridInteractionMode.Place || gridInteractionMode == GridInteractionMode.Clear)
                SetCurrentSideViewLayer(0);
        }
        transitionManager.onTransitionCutPointReached -= OnTransitionEndSwitchViewMode;
    }
}
