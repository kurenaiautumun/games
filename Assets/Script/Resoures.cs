using Unity.VisualScripting;
using UnityEngine;
using System.IO;
[System.Serializable]
public static class DATA_Elements
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves";
    public static void init()
    {
        Debug.Log("Start Resources init()");
       if (!Directory.Exists(SAVE_FOLDER))
        {
            Debug.LogError("No manners");
      
            Directory.CreateDirectory(SAVE_FOLDER);
        }
      
    }
    public static void save(string Save_string)
    {
        
        File.WriteAllText(SAVE_FOLDER + "/save.txt", Save_string);
        
    }

    public static string load()
    {
        if(File.Exists(SAVE_FOLDER + "/save.txt")) { 
            string savestring = File.ReadAllText(SAVE_FOLDER + "/save.txt");
            return savestring;
        }else { return null; }
        
    }

} 




public class Resoures : MonoBehaviour
{
    public int health;
    public int gloary;
    public int coins;
    public string username ;
    public int days;
    

       public string[] Job1_name;
        public int[] Job1_price;
        public int[] Job1_Health;
        public int[] Job1_Glory;



    public string[] Fooditems_name;
    public int[] Fooditems_price;
    public int[] Fooditems_Health;

   


}



