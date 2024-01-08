using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopper : MonoBehaviour
{
    public int Egg_heart;
    public GameObject egg_swpner;
    public GameObject bowl;
    public Text Egg_heart_text;
    public GameObject restart_button;
    
    void Start()
    {
        Debug.Log("Start Stopper");
        Egg_heart_text.text = "x" + Egg_heart.ToString();
        
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Eggs"))
        {

            Egg_heart--;
            Destroy(collision.gameObject);

            Egg_heart_text.text = "x " + Egg_heart.ToString();
            if (Egg_heart == 0)
            {
                egg_swpner.SetActive(false);
                bowl.SetActive(false);

                restart_button.SetActive(true);
                
            }
        }
    }
  
}
