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

    [Header("Input Threshold Section")]
    public float timeThreshold = 0.2f;
    public float distanceThreshold = 10.0f;
    public float velocityThreshold = 50.0f;

    private float initialDistance;

    private Vector2 startPosition;
    private float dragTimer;
    private bool isDragging = false;

    private void Update()
    {
        HandleTouchInput();
        HandleMouseInput();
    }

    private void HandleTouchInput()
    {
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
    }
}
