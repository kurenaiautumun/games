using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GameManagerController : MonoBehaviour
{
    [Header("Game View Section")]
    [SerializeField] private GameObject topDownMode;
    [SerializeField] private GameObject sideMode;
    private bool isSideModeEnabled;

    [Header("Grid Section")]
    public float gridCellSize = 1f;
    public Vector2Int playingFieldSize = Vector2Int.one;
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

    // We will need to store the grid location and type of object rather than the GameObject itself
    private List<GameObject> userPlacedPbjects;

    private bool[,] isCellOccupied;

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
        SetHighlight(previousTouchLocation);
        SetBoundary(previousTouchLocation);
        
        userPlacedPbjects = new List<GameObject>();

        previousTouchLocation = Vector2.zero;
        hasInputTouchDragged = false;
        hasInputTouchOverUI = false;

        isCellOccupied = new bool[playingFieldSize.x, playingFieldSize.y];
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
                    SetHighlight(touch.position);
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
                            if (!IsValidGridCells(gridPos))
                                SetHighlightColor(Color.red);
                            else
                            {
                                SetHighlightColor(new Color(255, 128, 0));
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

    private Vector3 GetGridPosition(Vector3 worldPos)
    {
        return new Vector3(Mathf.Floor(worldPos.x / gridCellSize), Mathf.Floor(worldPos.y / gridCellSize), 0);
    }

    private void SetHighlight(Vector3 screenPos)
    {
        var item = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetItem();
        var gridSize = Vector2.one;
        if (item != null) 
            gridSize = item.gameObject.GetComponent<GridObject>().size;

        gridHighlight.transform.position = GetGridPosition(Camera.main.ScreenToWorldPoint(screenPos)) * gridCellSize;
        gridHighlight.GetComponent<LineRenderer>().SetPosition(1, Vector3.right * gridCellSize * gridSize.x);
        gridHighlight.GetComponent<LineRenderer>().SetPosition(2, new Vector3(gridSize.x, gridSize.y, 0) * gridCellSize);
        gridHighlight.GetComponent<LineRenderer>().SetPosition(3, Vector3.up * gridCellSize * gridSize.y);
    }

    private void SetBoundary(Vector3 screenPos)
    {
        boundaryHighlight.transform.position = (GetGridPosition(Camera.main.ScreenToWorldPoint(screenPos)) + new Vector3(playingFieldOrigin.x, playingFieldOrigin.y, 0)) * gridCellSize;
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(1, Vector3.right * gridCellSize * playingFieldSize.x);
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(2, new Vector3(playingFieldSize.x, playingFieldSize.y, 0) * gridCellSize);
        boundaryHighlight.GetComponent<LineRenderer>().SetPosition(3, Vector3.up * gridCellSize * playingFieldSize.y);
    }

    private void SetHighlightColor(Color color)
    {
        gridHighlight.GetComponent<LineRenderer>().startColor = color;
        gridHighlight.GetComponent<LineRenderer>().endColor = color;
    }

    private void PlaceObjectInWorld(Vector3 gridPos)
    {
        var item = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetItem();
        var size = item.GetComponent<GridObject>().size;
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
                isCellOccupied[x + pos.x, y + pos.y] = true;
            }
        }

        userPlacedPbjects.Add(gridObject);

        // Side View
        var sideObject = Instantiate(item, new Vector3(gridPos.x + (item.GetComponent<GridObject>().size.x - 1) * 0.5f, item.transform.localScale.y * 0.5f, 0), Quaternion.identity);
        sideObject.transform.SetParent(sideMode.transform);
        sideObject.GetComponent<SpriteRenderer>().sortingOrder = (int)-gridPos.y;
    }

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

    private void PrintCells()
    {
        string s = "";
        for(int i = 0; i < playingFieldSize.x; i++)
        {
            for (int j = 0; j < playingFieldSize.y; j++)
                s += isCellOccupied[j, i] + ", ";
            s += "\n";
        }
            
        Debug.Log(s);
    }

    private bool IsValidGridCells(Vector3 gridPos)
    {
        Vector2Int pos = Vector2Int.RoundToInt(gridPos) - playingFieldOrigin;

        if (pos.x >= playingFieldSize.x || pos.x < 0 ||
            pos.y >= playingFieldSize.y || pos.y < 0)
            return false;

        var selection = itemPickerDropdownContainer.GetComponent<ItemPickerController>().GetItem();
        var size = selection.GetComponent<GridObject>().size;

        if (pos.x + size.x > playingFieldSize.x || pos.y + size.y > playingFieldSize.y)
            return false;

        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                if (isCellOccupied[x + pos.x, y + pos.y])
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

    private void OnTransitionEndSwitchViewMode()
    {
        if (isSideModeEnabled)
        {
            isSideModeEnabled = false;
            sideMode.SetActive(false);
            topDownMode.SetActive(true);

            cameraController.EnableGridLines();
        }
        else
        {
            isSideModeEnabled = true;
            sideMode.SetActive(true);
            topDownMode.SetActive(false);

            cameraController.DisableGridLines();
        }
        transitionManager.onTransitionCutPointReached -= OnTransitionEndSwitchViewMode;
    }
}
