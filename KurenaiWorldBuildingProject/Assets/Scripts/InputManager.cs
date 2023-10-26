using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{

    #region EVENTS

    public delegate void DragHandler(Vector2 delta);
    public static event DragHandler OnDragEvent;

    public delegate void BeginDragHandler();
    public static event BeginDragHandler BeginDragEvent;

    public delegate void EndDragHandler();
    public static event EndDragHandler EndDragEvent;

    public delegate void PinchHandler(float delta);
    public static event PinchHandler PinchEvent;

    public delegate void ScrollHandler(float delta);
    public static event ScrollHandler ScrollEvent;

    public delegate void TapHandler(Vector2 pos);
    public static event TapHandler OnSingleTapEvent;
    public static event TapHandler OnDoubleTapEvent;

    #endregion

    public static bool isCanvasElementInteracted;

    [Header("Input Threshold Section")]
    public float timeThreshold = 0.2f;
    public float distanceThreshold = 10.0f;
    public float velocityThreshold = 50.0f;
    public float doubleTapTimeout = 0.1f;

    [Header("Interaction Section")]
    public bool ignoreCanvasInteraction;

    private float initialDistance;

    private Vector2 startPosition;
    private float dragTimer;
    private bool isDragging;

    private bool isCursorInsideViewport;

    private float doubleTapTimer;
    private bool isDoubleTapTimerActivated;

    private void Start()
    {
        isDragging = false;
        isCursorInsideViewport = false;
        isDoubleTapTimerActivated = false;
        isCanvasElementInteracted = false;

        if (!ignoreCanvasInteraction && EventSystem.current == null)
            Debug.LogError("Please add an EventSystem!");
    }

    private void Update()
    {
        if(!ignoreCanvasInteraction)
        {
            // Check if UI is being interacted
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                isCanvasElementInteracted = true;
                //Debug.Log("UI element is being interacted with: " + EventSystem.current.currentSelectedGameObject.name);
                return;
            }
        }
        
        HandleTouchInput();
        HandleMouseInput();     

        if (isDoubleTapTimerActivated)
        {
            doubleTapTimer += Time.deltaTime;
            if(doubleTapTimer > doubleTapTimeout)
            {
                doubleTapTimer = 0;
                isDoubleTapTimerActivated = false;
            }
        }

        isCanvasElementInteracted = false;
    }

    private void HandleTouchInput()
    {
        if(!ignoreCanvasInteraction)
        {
            if(EventSystem.current.alreadySelecting)
            {
                Debug.Log("Already selecting");
                return;
            }
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
                dragTimer = 0;
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                dragTimer += Time.deltaTime;
                HandleDrag(touch.position);
            }

            else if (touch.phase == TouchPhase.Ended)
            {
                if (!isDragging)
                    HandleTap(touch.position);
                else
                {
                    if(EndDragEvent != null)
                        EndDragEvent();
                }
                isDragging = false;
                dragTimer = 0;
            }
        }

        if (Input.touchCount == 2)
        {
            if(isDragging)
            {
                isDragging = false;
                dragTimer = 0;
            }

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
            }
            else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);
                float deltaDistance = currentDistance - initialDistance;
                if (PinchEvent != null)
                    PinchEvent(deltaDistance);
            }
        }
    }

    private void HandleMouseInput()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0 && ScrollEvent != null)
            ScrollEvent(scroll);

        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            dragTimer = 0;
        }

        else if (Input.GetMouseButton(0))
        {
            dragTimer += Time.deltaTime;
            HandleDrag(Input.mousePosition);                   
        }

        else if(Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
                HandleTap(Input.mousePosition);
            else
            {
                if (EndDragEvent != null)
                    EndDragEvent();
            }
            isDragging = false;
            dragTimer = 0;
        }

        // Check if cursor is inside the application
        isCursorInsideViewport = true;
        Vector3 viewportPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (viewportPoint.x > 1 || viewportPoint.y > 1 || viewportPoint.x < 0 || viewportPoint.y < 0)
            isCursorInsideViewport = false;      
    }

    private void HandleDrag(Vector2 pos)
    {
        float distance = Vector2.Distance(startPosition, pos);
        float velocity = distance / dragTimer;

        if (dragTimer > timeThreshold || distance > distanceThreshold || velocity > velocityThreshold)
        {
            if(!isDragging)
            {
                if (BeginDragEvent != null)
                    BeginDragEvent();
                isDragging = true;
            }

            //Debug.Log("Drag detected.");
            Vector2 delta = pos - startPosition;
            if (OnDragEvent != null)
                OnDragEvent(delta);
            startPosition = pos;
        }
    }

    private void HandleTap(Vector2 pos)
    {
        //Debug.Log("Tap detected.");
        if (OnSingleTapEvent != null)
            OnSingleTapEvent(pos);

        if(isDoubleTapTimerActivated)
        {
            if(doubleTapTimer < doubleTapTimeout)
            {
                //Debug.Log("DoubleTap detected.");
                if (OnDoubleTapEvent != null)
                    OnDoubleTapEvent(pos);
            }
            
            isDoubleTapTimerActivated = false;
            doubleTapTimer = 0;
        }
        else
        {
            doubleTapTimer = 0;
            isDoubleTapTimerActivated = true;
        }
    }
}
