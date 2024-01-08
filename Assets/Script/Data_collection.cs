using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Data_collection : MonoBehaviour
{
    [Header("Jobs")]
    public TMP_Text Jobs_name_text;
    public TMP_Text Jobs_price_text;
    public TMP_Text Jobs_Health_Text;
    public TMP_Text Jobs_Glory_Text;
    [Header("Fooditems")]
    public TMP_Text Fooditems_name_text;
    public TMP_Text Fooditems_price_text;
    public TMP_Text Fooditems_health_text;
    [Header("Player profile")]
    public TMP_Text Player_name;
    public TMP_Text Player_level;
    public TMP_Text Player_days;
    public TMP_Text Player_health;
    public TMP_Text Player_coins;
    public TMP_Text Health_Text;
    public TMP_Text Glory_Text;
    public TMP_Text Coins_Text;
    [Header("script")]
    public int minushealth;
    public int plusglory;
    public int plusprice;
    public int plushealth;
    public int minusprice;
    public Button button;
    private Spawner spawner;
    public  Data_Handler data_handler;
    public Resoures res;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Data Collection");
        res = FindFirstObjectByType<Resoures>();
        data_handler = FindFirstObjectByType<Data_Handler>();
        button = GetComponent<Button>();
        Health_Text = GameObject.FindGameObjectWithTag("Health").GetComponent<TMP_Text>();
        Glory_Text = GameObject.FindGameObjectWithTag("Gloary").GetComponent<TMP_Text>();
        Coins_Text = GameObject.FindGameObjectWithTag("Coins").GetComponent<TMP_Text>();
        spawner = FindAnyObjectByType<Spawner>();
        TopRow_();
        load_playerprofile();
        jobs_assigner();
        Fooditems_assigner();
        

        if (button != null)
        {
            button.onClick.AddListener(onclickk);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public void jobs_assigner()
    {
        int a = 0;  
        
        for (a = 0; a <= spawner.jobs.Count; a++)
        {
            
            if (Jobs_name_text != null)
            {
                
                if (spawner.jobs.IndexOf(this.gameObject) <= a )
                {
                    Jobs_name_text.text = res.Job1_name[a];
                    break;
                }
            }
        }
        for (a = 0; a <= spawner.jobs.Count; a++)
        {

            if (Jobs_name_text != null)
            {
                
                if (spawner.jobs.IndexOf(this.gameObject) <= a)
                {
                   Jobs_Health_Text.text = res.Job1_Health[a].ToString();
                    minushealth = res.Job1_Health[a];
                    break;
                }
            }
        }
        for (a = 0; a <= spawner.jobs.Count; a++)
        {

            if (Jobs_name_text != null)
            {
                if (spawner.jobs.IndexOf(this.gameObject) <= a)
                {
                    Jobs_price_text.text = res.Job1_price[a].ToString();
                    plusprice = res.Job1_price[a];
                    break;
                }
            }
        }
        for (a = 0; a <= spawner.jobs.Count; a++)
        {

            if (Jobs_name_text != null)
            {
                
                if (spawner.jobs.IndexOf(this.gameObject) <= a)
                {
                    Jobs_Glory_Text.text = res.Job1_Glory[a].ToString();
                    plusglory = res.Job1_Glory[a];
                    break;
                }
            }
        }
       
    }
    public void Fooditems_assigner()
    {
        int i;
        
        for (i = 0; i < spawner.Maxfooditems; i++)
        {
            if (Fooditems_name_text != null)
            {
                if (spawner.fooditems.IndexOf(this.gameObject) <= i)
                {
                    Fooditems_name_text.text = res.Fooditems_name[i].ToString();
                    break;

                }
            }
        }
        for (i = 0; i < spawner.Maxfooditems; i++)
        {
            if (Fooditems_name_text != null)
            {
                if (spawner.fooditems.IndexOf(this.gameObject) <= i)
                {
                    Fooditems_price_text.text = res.Fooditems_price[i].ToString();
                    minusprice = res.Fooditems_price[i];
                    break;

                }
            }
        }
        for (i = 0; i < spawner.Maxfooditems; i++)
        {
            if (Fooditems_name_text != null)
            {
                if (spawner.fooditems.IndexOf(this.gameObject) <= i)
                {
                    Fooditems_health_text.text = res.Fooditems_Health[i].ToString();
                    plushealth = res.Fooditems_Health[i];
                    break;

                }
            }
        }
       
    }


    public void load_playerprofile()
    {
       data_handler.savetojson();
        data_handler.loaddata();
        Debug.Log("button is pressed");
        if (Player_name != null)
        {
                Player_name.text = res.username;
            
            
        }
        if (Player_health != null)
        {
            Debug.Log("ahjsiogfsioghoskginAOGHvsuhF");

            Player_health.text = res.health.ToString();
               
            
            
        }
        if (Player_coins != null)
        {
            Player_coins.text = res.coins.ToString();
        }
        if (Player_days != null)
        {
            Player_days.text = res.days.ToString();
        }
        TopRow_();
        

    }
    public void onclickk()
    {
        res.health += minushealth;
        res.coins += plusprice;
        res.gloary += plusglory;
       // data_handler.savetojson();
        TopRow_();
       


        Debug.Log("hello");

    }
    public void onclickk1()
    {
        res.health += plushealth;
        res.coins += minusprice;
       
       data_handler.savetojson();
        TopRow_();



        Debug.Log("hello1");

    }
    public void TopRow_()
    {
       // data_handler.savetojson();
        data_handler.loaddata();
        if (Health_Text != null)
        {
            Debug.Log("data is ");
            
            Health_Text.text = res.health.ToString();
            
            

        }
        else { return; }
        if (Coins_Text != null)
        {
            Coins_Text.text = res.coins.ToString();

        }else { return; }
        if (Glory_Text != null)
        {
            Glory_Text.text = res.gloary.ToString();

        }
        else { return; }
    }
  public void Clock()
    {
        
        Debug.Log("helooooooooooooooooo");
        res.days++;
        res.health = 100; 
       // data_handler.savetojson();
        TopRow_();
    }
    private void OnApplicationQuit()
    {
       data_handler.savetojson();
    }
    private void OnDisable()
    {
       data_handler.savetojson();
    }
}
