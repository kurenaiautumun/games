using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Data_Handler : MonoBehaviour
{
    
   
    public static Data_Handler data_Handler;
    public static Resoures res;
    // Start is called before the first frame update
    public void Awake()
    {
        Debug.Log("Awake Data Handler");
        //SceneManager.LoadScene(00);
        DATA_Elements.init();
        if(res == null)
        {
           res = FindFirstObjectByType<Resoures>(); 
        } 
        loaddata();
    }
    

    void Start()
    {
        Debug.Log("Start Data Handler");
        data_Handler = this;
        savetojson();
        loaddata();
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void savetojson()
    {
        data1 data = new data1()
        {

            Health = res.health,
            Glory = res.gloary,
            Coins = res.coins,
            username = res.username,
            days = res.days  

        };
        string json = JsonUtility.ToJson(data);

        DATA_Elements.save(json);

        
        
        
        
     }
        
        

        
   
    public void loaddata()
    {
        string savestring = DATA_Elements.load();
        if(savestring != null) { 
        data1  data = JsonUtility.FromJson<data1>(savestring);
            Debug.LogError(savestring);
            
            res.health = data.Health;
            res.gloary = data.Glory;
            res.coins = data.Coins;
            res.username = data.username;
            res.days = data.days;
            
        }
        else
        {
            Debug.Log("no save");
        }

    }
}
public class data1
{
    public  int Health;
    public  int Glory;
    public int Coins;
    public  int level;
    public int days;
    public int points;
    public  string username;
}
