using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// Data structure to store grid location and the object type (for now)
class LocationAndTypeData
{
    public LocationAndTypeData(Vector3 location, GameObject type)
    {
        gridLocation = location;
        objectType = type;
    }

    public Vector3 gridLocation;
    public GameObject objectType;
}


public class GameManagerController : MonoBehaviour
{
    [Header("Game View Section")]
    [SerializeField] private GameObject topDownMode;
    [SerializeField] private GameObject sideMode;
    private bool isSideModeEnabled;

    [Header("Debug Only Section")]
    [SerializeField] bool isDebugEnabled;
    [SerializeField] private GameObject debugCanvasObject;
    [SerializeField] private TMP_Text mouseCoordinateScreenText;
    [SerializeField] private TMP_Text mouseCoordinateWorldText;
    [SerializeField] private TMP_Text gridCoordinateText;
    [SerializeField] private GameObject gridHighlight;
    [SerializeField] private GameObject filler;

    [Header("Transition Section")]
    [SerializeField] private TransitionSettings transition;
    private TransitionManager transitionManager;

    private GridMesh gridMesh;
    private MeshRenderer gridMeshRenderer;

    // We will need to store the grid location and type of object rather than the GameObject itself
    private List<LocationAndTypeData> userPlacedPbjects;


    void Start()
    {
        if (isDebugEnabled)
            debugCanvasObject.SetActive(true);
        else
            debugCanvasObject.SetActive(false);

        topDownMode.SetActive(true);
        sideMode.SetActive(false);
        isSideModeEnabled = false;

        transitionManager = TransitionManager.Instance();
        gridMesh = GetComponent<GridMesh>();

        gridHighlight.GetComponent<LineRenderer>().SetPosition(1, Vector3.right * gridMesh.CellSize);
        gridHighlight.GetComponent<LineRenderer>().SetPosition(2, new Vector3(1,1,0) * gridMesh.CellSize);
        gridHighlight.GetComponent<LineRenderer>().SetPosition(3, Vector3.up * gridMesh.CellSize);

        gridMeshRenderer = GetComponent<MeshRenderer>();

        userPlacedPbjects = new List<LocationAndTypeData>();
    }

    void Update()
    {
        if (isDebugEnabled)
        {
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            var gridPos = new Vector3(Mathf.Floor(mouseWorldPos.x / gridMesh.CellSize), Mathf.Floor(mouseWorldPos.y / gridMesh.CellSize), 0);

            // Debug display coords
            mouseCoordinateScreenText.SetText("MouseScreenPos: " + mouseScreenPos.ToString());
            mouseCoordinateWorldText.SetText("MouseWorldPos: " + mouseWorldPos.ToString());
            gridCoordinateText.SetText("GridPos: " + gridPos.ToString());

            gridHighlight.transform.position = gridPos * gridMesh.CellSize;

            // We will also need to prevent data to be added to an already filled location

            if(Input.GetMouseButtonDown(0))
            {
                var gridObject = Instantiate(filler, (gridPos + new Vector3(0.5f,0.5f,0)) * gridMesh.CellSize, Quaternion.identity);
                gridObject.transform.SetParent(topDownMode.transform);
                var data = new LocationAndTypeData(gridPos, gridObject);
                userPlacedPbjects.Add(data);
            }
        }

        Vector3 move = Vector3.up * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
        Camera.main.transform.position += move * Time.deltaTime;
    }

    ////////////////////////////////////////////
    ///// UI Operations
    ////////////////////////////////////////////

    public void SwitchViewMode()
    {
        transitionManager.onTransitionCutPointReached += OnTransitionEndSwitchViewMode;
        transitionManager.Transition(transition, 0f);
    }

    private void OnTransitionEndSwitchViewMode()
    {
        if (isSideModeEnabled)
        {
            isSideModeEnabled = false;
            sideMode.SetActive(false);
            topDownMode.SetActive(true);

            gridMeshRenderer.enabled = true;
        }
        else
        {
            isSideModeEnabled = true;
            sideMode.SetActive(true);
            topDownMode.SetActive(false);

            gridMeshRenderer.enabled = false;
        }
        transitionManager.onTransitionCutPointReached -= OnTransitionEndSwitchViewMode;
    }
}
