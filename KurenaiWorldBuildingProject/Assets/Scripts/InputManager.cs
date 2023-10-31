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
            // Check if UI is being interacted, then ignore the rest as EventSystem handles UI interactions
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                isCanvasElementInteracted = true;
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
        // No need to process further  if the EventSystem is still active
        if(!ignoreCanvasInteraction)
        {
            if(EventSystem.current.alreadySelecting)
            {
                Debug.Log("Already selecting");
                return;
            }
        }

        // Single/Double tap and drag are single touch events here
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            // Set values at the start
            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
                dragTimer = 0;
            }
            
            // If we moved then we check if it is a valid drag action
            else if(touch.phase == TouchPhase.Moved)
            {
                dragTimer += Time.deltaTime;
                HandleDrag(touch.position);
            }

            // If we finish, then we check if single/double tap if it is not drag action
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

        // Pinch action requires 2 touches (might require further adjustments)
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
        // No need to process further  if the EventSystem is still active
        if (!ignoreCanvasInteraction)
        {
            if (EventSystem.current.alreadySelecting)
            {
                Debug.Log("Already selecting");
                return;
            }
        }

        // Get the value for scrolling event
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0 && ScrollEvent != null)
            ScrollEvent(scroll);

        // We just clicked the mouse so set the values
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            dragTimer = 0;
        }

        // If we are still holding down the mouse button then check for drag action
        else if (Input.GetMouseButton(0))
        {
            dragTimer += Time.deltaTime;
            HandleDrag(Input.mousePosition);                   
        }

        // If we release the button then check for single/double tap if it is not drag action
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

        // Check if cursor is inside the application (Currently not used)
        isCursorInsideViewport = true;
        Vector3 viewportPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (viewportPoint.x > 1 || viewportPoint.y > 1 || viewportPoint.x < 0 || viewportPoint.y < 0)
            isCursorInsideViewport = false;      
    }

    // Check for drag action
    private void HandleDrag(Vector2 pos)
    {
        float distance = Vector2.Distance(startPosition, pos);
        float velocity = distance / dragTimer;

        // Determine if drag based on threshold
        if (dragTimer > timeThreshold || distance > distanceThreshold || velocity > velocityThreshold)
        {
            if(!isDragging)
            {
                if (BeginDragEvent != null)
                    BeginDragEvent();
                isDragging = true;
            }

            // We send the change in the positions (Might need to adjust this further)
            Vector2 delta = pos - startPosition;
            if (OnDragEvent != null)
                OnDragEvent(delta);
            startPosition = pos;
        }
    }

    // Check if single or double tap
    private void HandleTap(Vector2 pos)
    {
        if (OnSingleTapEvent != null)
            OnSingleTapEvent(pos);

        if(isDoubleTapTimerActivated)
        {
            if(doubleTapTimer < doubleTapTimeout)
            {
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
