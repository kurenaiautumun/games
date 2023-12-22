using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appear : MonoBehaviour
{
   // Start is called before the first frame update
    void Start()
    {
        // Ensure the GameObject is initially inactive
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the Tab key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle the active state of the GameObject
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }

 
}
