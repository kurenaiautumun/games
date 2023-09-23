using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmManager : MonoBehaviour
{
    public PlantItem selectPlant;
    public bool isPlanting = false;
    public int money= 500;
    public Text moneyTxt;

    public Color buyColor = Color.green;
    public Color cancelColor = Color.red;

    public bool isSelecting = false;
    public int selectedTool=0;
    // 1- water 2- Fertilizer 3- Buy plot

    public Image[] buttonsImg;
    public Sprite normalButton;
    public Sprite selectedButton;
    PlantObject pb;

    public static FarmManager instance;


    public SaveData saveData;
    
    [SerializeField] private string shopItemsPath = "Shop";

    private void Awake() {
        instance = this;
        SaveSystem.Initailize();
    }
    // Start is called before the first frame update
    void Start()
    {
        moneyTxt.text = "$" + money;
        
        LoadGame();
    }

    public void SelectPlant(PlantItem newPlant)
    {
        if(selectPlant == newPlant)
        {
            CheckSelection();
            
        }
        else
        {
            CheckSelection();
            selectPlant = newPlant;
            selectPlant.btnImage.color = cancelColor;
            selectPlant.btnTxt.text = "Cancel";
            isPlanting = true;
        }
    }

    public void SelectTool(int toolNumber)
    {
        if(toolNumber == selectedTool)
        {
            //deselect
            CheckSelection();
        }
        else
        {
            //select tool number and check to see if anything was also selected
            CheckSelection();
            isSelecting = true;
            selectedTool = toolNumber;
            buttonsImg[toolNumber - 1].sprite = selectedButton;
        }
    }

    void CheckSelection()
    {
        if (isPlanting)
        {
            isPlanting = false;
            if (selectPlant != null)
            {
                selectPlant.btnImage.color = buyColor;
                selectPlant.btnTxt.text = "Buy";
                selectPlant = null;
            }
        }
        if (isSelecting)
        {
            if (selectedTool > 0)
            {
                buttonsImg[selectedTool - 1].sprite = normalButton;
            }
            isSelecting = false;
            selectedTool = 0;
        }
    }

    public void Transaction(int value)
    {
        money += value;
        moneyTxt.text = "$" + money;
    }

    private void LoadGame()
    {
         Debug.Log("Cannot Load1");
        LoadPlaceableObjects();
    }
    private void LoadPlaceableObjects()
    {Debug.Log("Cannot Load2");
        Debug.Log(saveData.placeableobjectDatas.Count);
        foreach(var plObjData in saveData.placeableobjectDatas.Values)
        {Debug.Log("Cannot Load3");
            try
            {
                Debug.Log("Cannot Load4");
                PlantObject item = Resources.Load<PlantObject>(shopItemsPath + "/" + plObjData.assetName);
                PlotManager.instance.plant.gameObject.SetActive(true) ;
                GameObject obj = PlotManager.instance.plant.gameObject ;
                PlotManager plObj = obj.GetComponent<PlotManager>();
                plObj.Initailize(item, plObjData);
                //incase set active false nhi hots toh poora likh ke try krna yha pe 
            }
            catch (System.Exception)
            {
                Debug.Log("Cannot Load");
                //throw;
            }
        }
    }

    private void OnDisable() {
        SaveSystem.Save(saveData);
        
    }
}
