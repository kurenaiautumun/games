using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercontroller : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 10f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Joystick joystick; // Reference to the mobile joystick

    void Update()
    {
        // Keyboard controls
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Mobile joystick controls (only if joystick is active)
        if (joystick != null && joystick.isActiveAndEnabled)
        {
            horizontal = joystick.Horizontal;
            vertical = joystick.Vertical;
        }

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 Movedirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(Movedirection.normalized * speed * Time.deltaTime);
        }
    }
}