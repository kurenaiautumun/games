using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;
using Doublsb.Dialog;
using UnityEngine.UI;

[System.Serializable]
class MapMarkerInfo
{
    public string name, mapFilePath;
    public Vector2 position;
}

public class MapManagerController : MonoBehaviour
{
    private Vector2 previousTouchLocation;
    private bool hasInputTouchDragged;
    private bool hasInputTouchOverUI;

    [Header("Input Section")]
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 10f;
    private float zoom = 1f;

    [Header("Map Section")]
    [SerializeField] private GameObject mapMarkerPrefab;
    [SerializeField] private List<MapMarkerInfo> mapMarkers = new List<MapMarkerInfo>();
    private GameObject mapMarkerHierarchy;

    [Header("Outline Section")]
    [SerializeField] private Material outlineMat;
    [SerializeField] private Material spriteMat;
    [SerializeField] private float outlinePulseSpeed = 1f, outlineMinThickness = 2f, outlineMaxThickness = 5f;
    private float outlineTimer = 0f;
    private GameObject currentSelectedRegion;
    private SpriteRenderer currentSelectedRegionSpriteRenderer;

    private DialogManager dialogManager;

    void Start()
    {
        zoom = Camera.main.orthographicSize;

        dialogManager = GameObject.Find("DialogAsset").GetComponent<DialogManager>();

        CreateMapMarkers();

        // Hook the functions to the correct events
        InputManager.ScrollEvent += ZoomCamera;
        InputManager.PinchEvent += ZoomCamera;
        InputManager.OnDragEvent += MoveCamera;
        InputManager.OnSingleTapEvent += HandleSelection;

        currentSelectedRegion = null;
        currentSelectedRegionSpriteRenderer = null;
    }

    private void OnDestroy()
    {
        // Unhook them because we don't need them anymore if this script does not exist
        InputManager.ScrollEvent -= ZoomCamera;
        InputManager.PinchEvent -= ZoomCamera;
        InputManager.OnDragEvent -= MoveCamera;
        InputManager.OnSingleTapEvent -= HandleSelection;
    }

    private void OnDisable()
    {

    }

    void Update()
    {
        // If a region is selected then we animate it's outline
        if(currentSelectedRegion != null )
        {
            HighlightSelectedRegion();
        }
    }

    // Handles the pulsing animation of the boundary
    private void HighlightSelectedRegion()
    {
        outlineTimer += Time.deltaTime;
        float lerpTime = (Mathf.Sin(outlineTimer * outlinePulseSpeed) + 1) * 0.5f;
        float thickness = Mathf.Lerp(outlineMinThickness, outlineMaxThickness, lerpTime);
        currentSelectedRegionSpriteRenderer.material.SetFloat("_Distance", thickness);
    }

    // If we do a single tap then we check if we selected anything
    private void HandleSelection(Vector2 pos)
    {
        // Using colliders we check if the point we tapped on also coincides with some object
        var touchWorldPos = Camera.main.ScreenToWorldPoint(pos);
        Collider2D collider = Physics2D.OverlapPoint(touchWorldPos);

        // No collider so no object was detected
        if(collider == null)
        {
            // Deselect any selected region
            if (currentSelectedRegion != null)
            {
                currentSelectedRegionSpriteRenderer.material = spriteMat;
                currentSelectedRegion = null;
            }
        }

        // Found an object with a collider
        else
        {
            // If we had selected the same region then we do not need to do anything else currently
            if (currentSelectedRegion == collider.gameObject)
                return;

            // A different region so we reset the material of the previously selected region
            if(currentSelectedRegion != null)
                currentSelectedRegionSpriteRenderer.material = spriteMat;

            // Now we apply the changes to the new region
            currentSelectedRegionSpriteRenderer = collider.gameObject.GetComponent<SpriteRenderer>();
            currentSelectedRegionSpriteRenderer.material = outlineMat;
            currentSelectedRegion = collider.gameObject;
            outlineTimer = 0;

            // If we selected a map marker then we should handle accordingly
            //SelectMapMarker(collider.gameObject);
        }
    }

    // Unused (InputManager handles this now)
    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    hasInputTouchOverUI = true;
                else
                {
                    previousTouchLocation = touch.position;
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
                        hasInputTouchDragged = true;
                        MoveCamera(displacement);
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
                        Collider2D collider = Physics2D.OverlapPoint(touchWorldPos);

                        if (collider != null)
                        {
                            SelectMapMarker(collider.gameObject);
                        }
                    }
                }
                hasInputTouchOverUI = false;
                hasInputTouchDragged = false;
            }
        }

        else if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            // Zoom in/out
            ZoomCamera(deltaMagnitudeDiff);
        }
    }

    // Unused (InputManager handles this now)
    private void HandleMouseKeyboardInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                hasInputTouchOverUI = true;
            else
            {
                previousTouchLocation = Input.mousePosition;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!hasInputTouchOverUI)
            {
                var displacement = (Vector2)Input.mousePosition - previousTouchLocation;
                previousTouchLocation = Input.mousePosition;
                if (displacement.magnitude > 25)
                {
                    hasInputTouchDragged = true;
                    MoveCamera(displacement);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!hasInputTouchOverUI)
            {
                if (!hasInputTouchDragged)
                {
                    var touchScreenPos = Input.mousePosition;
                    var touchWorldPos = Camera.main.ScreenToWorldPoint(touchScreenPos);
                    Collider2D collider = Physics2D.OverlapPoint(touchWorldPos);

                    if (collider != null)
                    {
                        SelectMapMarker(collider.gameObject);
                    }
                }
            }
            hasInputTouchOverUI = false;
            hasInputTouchDragged = false;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll);
    }

    // Handles the zooming functionality (value is provided by InputManager)
    private void ZoomCamera(float deltaValue)
    {
        if (deltaValue < 0)
        {
            zoom = Mathf.Min(zoom + zoomSpeed, maxZoom);
            Camera.main.orthographicSize = zoom;
        }
        else if (deltaValue > 0)
        {
            zoom = Mathf.Max(zoom - zoomSpeed, minZoom);
            Camera.main.orthographicSize = zoom;
        }
    }

    // Handles the camera movement functionality (value is provided by InputManager)
    private void MoveCamera(Vector2 displacement)
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos -= new Vector3(displacement.x, displacement.y, 0) * Time.deltaTime;

        Camera.main.transform.position = camPos;
    }

    // Dynamically create map markers (Only used for demonstration purposes right now)
    private void CreateMapMarkers()
    {
        if(mapMarkerHierarchy != null)
            Destroy(mapMarkerHierarchy);
        mapMarkerHierarchy = GameObject.Find("Map Markers");
        if(mapMarkerHierarchy == null)
            mapMarkerHierarchy = new GameObject("Map Markers");

        // Create markers from given List (or even from a provided file in future)
        for (int i = 0; i < mapMarkers.Count; i++)
        {
            var info = mapMarkers[i];
            if (!File.Exists(info.mapFilePath))
            {
                Debug.Log("Cannot find the file: " + info.mapFilePath);
                continue;
            }

            var marker = Instantiate(mapMarkerPrefab, mapMarkerHierarchy.transform);
            var infoObject = marker.GetComponent<MapMarkerInfoObject>();

            marker.transform.position = info.position;
            marker.name = info.name;
            infoObject.position = info.position;
            infoObject.mapFilePath = info.mapFilePath;
        }
    }
 
    // If we selected a map marker, then we should display some information
    private void SelectMapMarker(GameObject mapMarker)
    {
        // Show the text
        var component = mapMarker.GetComponent<MapMarkerInfoObject>();
        DialogData dialog = new DialogData("/speed:0/" + Path.GetFileName(component.mapFilePath), "Unnamed");
        dialogManager.Show(dialog);

        // Box and text adjustments
        var mapMarkerPos = mapMarker.transform.position;
        var mapMarkerSize = mapMarker.GetComponent<SpriteRenderer>().bounds.extents * 2;
        var pos = Camera.main.WorldToScreenPoint(new Vector3(mapMarkerPos.x, mapMarkerPos.y - mapMarkerSize.y, 0));
        pos.y -= 200;

        RectTransform t = dialogManager.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        t.sizeDelta = new Vector2(800, 200);
        t.gameObject.transform.parent.position = pos;
       
        var text = dialogManager.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
    }

}
