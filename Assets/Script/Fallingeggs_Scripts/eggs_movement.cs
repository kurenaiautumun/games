using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggs_movement : MonoBehaviour
{   public float falling_speed;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Eggs Movement");
        if (falling_speed >= 300)
        {
            falling_speed = 300;
        }

    }
    private void OnDestroy()
    {
       Eggs_Spwaner.get.egg_list.Remove(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
        this.transform.Translate(Vector3.down * falling_speed * Time.deltaTime) ;
    }
}
