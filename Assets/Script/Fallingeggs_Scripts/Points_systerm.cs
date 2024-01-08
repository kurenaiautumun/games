using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Points_systerm : MonoBehaviour
{
    catching_eggs catching_;
    public Text Point_text;
    public Data_Handler data_Handler;
    public Resoures res;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Points");
        res = FindAnyObjectByType<Resoures>();
        data_Handler = FindFirstObjectByType<Data_Handler>();
        catching_ = FindFirstObjectByType<catching_eggs>();
        Point_text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        updating_points_text();
       
    }
    public void updating_points_text()
    {
        Point_text.text = catching_.points.ToString();
    }
    public void quit()
    {
      // loadpoints();
        //points_data points;
      //  res.coins += catching_.points / 2;
        //data_Handler.savetojson();


        SceneManager.LoadScene(00);
    }
    public void restartgame()
    {
        
        savepoints();
        SceneManager.LoadScene(01);
    }
    public void savepoints()
    {
        points_data points = new points_data()
        {
           saved_points = catching_.points
        };
        string json = JsonUtility.ToJson(points);
        File.WriteAllText(Application.dataPath + "/Points_data.txt", json);
        Debug.Log("saved");


    }
    public void  loadpoints()
    {
        string save = File.ReadAllText(Application.dataPath + "/Points_data.txt");
        points_data points = JsonUtility.FromJson<points_data>(save);
                   catching_.points = points.saved_points; 
    }
    public class points_data
    {
       public int saved_points;
    }
   

}
