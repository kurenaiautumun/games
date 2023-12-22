using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playeranimation : MonoBehaviour
{
    Animator animator;
    public Joystick joystick;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Keyboard controls
        bool forwardPressed = Input.GetKey("w");
        bool backwardpressed = Input.GetKey("s");
        bool runPressed = Input.GetKey("left shift");

        // Joystick controls
        float verticalJoystick = joystick.Vertical;
        bool isWalking = Mathf.Abs(verticalJoystick) >= 0.1f;

        if (isWalking)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isWalkingBackwards", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isWalkingBackwards", false);
        }

        if (verticalJoystick < 0)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isWalkingBackwards", true);
        }

        if (!isWalking && runPressed)
        {
            animator.SetBool("isRunning", true);
        }

        if (isWalking || backwardpressed)
        {
            animator.SetBool("isRunning", false);
        }
    }
}