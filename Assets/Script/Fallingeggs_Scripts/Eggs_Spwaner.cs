using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public  class Eggs_Spwaner : MonoBehaviour
{
    public GameObject canvas;
    public GameObject Eggs_prefab;
    public int randomnox;
    public  List<GameObject> egg_list = new List<GameObject>();
    public float time = 0;
    public float Swpanrate;
    public static Eggs_Spwaner get;
    public float max_swpnarate;
    // Start is called before the first frame update
    private void Awake()
    {
        get = this;
        
    }
    void Start()
    {
        //Eggs_Spwaning();
        SceneManager.LoadScene(00);
       
    }

    // Update is called once per frame
    void Update() 
    {
        time -= Time.deltaTime; 
        if(time <= 0)
        {
            time = 1 / Swpanrate;
            Eggs_Spwaning();
        }
    }
   public void Eggs_Spwaning()
    {
            Debug.Log("Eggs Spawing Start");
           if(Swpanrate >= max_swpnarate)
        {
            Swpanrate = 1.2f;
        }
           
            randomnox = Random.Range(-300, 400);
            Vector3 pos = new Vector3(randomnox, transform.position.y, transform.position.z);
            GameObject egg = Instantiate(Eggs_prefab, transform);
            egg.transform.Translate(randomnox, 0, 0);
            egg.transform.SetParent(canvas.gameObject.transform, false);
           egg_list.Add(egg);
    }
}
