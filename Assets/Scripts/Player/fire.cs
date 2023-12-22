using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    public float FireballmoveSpeed;
    public Animator anim;
    private void Start()
    {
        fireballPrefab = GameObject.FindGameObjectWithTag("fireball");
        anim=GetComponent<Animator>();
      
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("fire");
        }
        // Check for touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchPosition.z = 0;
            Fireball(touchPosition);
        }

        // Check for keyboard input (space key)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fireball(transform.position); // You can modify this to get a different position for keyboard input
        }
    }

    void Fireball(Vector3 position)
    {
        Instantiate(fireballPrefab, position, Quaternion.identity);
        Move();
    }
    void Move()
    {
          // Access the transform component of the fireball
        Transform fireballTransform = fireballPrefab.transform;

        // Move the fireball forward along the X-axis using Translate
        fireballTransform.Translate(Vector3.forward * FireballmoveSpeed * Time.deltaTime);
    }
}
