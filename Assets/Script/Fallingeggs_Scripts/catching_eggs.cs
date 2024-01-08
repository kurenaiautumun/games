using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;

public class catching_eggs : MonoBehaviour
{
    public int points;
    Data_Handler handler;
    private Eggs_Spwaner eggs_Spwaner;
    public eggs_movement eggs_Movement;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Catching Eggs");
       Points_systerm pointsysterm = FindFirstObjectByType<Points_systerm>();
        if (points == 0) { pointsysterm.loadpoints(); }
       eggs_Spwaner = FindAnyObjectByType<Eggs_Spwaner>();
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Eggs"))
        {    points++;
            Destroy(other.gameObject);
            eggs_Spwaner.Swpanrate  *= 1.02f;
            eggs_Movement.falling_speed *= 1.1f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    

}
