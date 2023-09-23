using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class PlotManager : MonoBehaviour
{
    public static PlotManager instance;
    private PlantObject item;
    //Slider
    public float Progress = 10f;

    bool isPlanted = false;
    public  SpriteRenderer plant;
    BoxCollider2D plantCollider;

    int plantStage = 0;
    float timer;

    public Color availableColor = Color.green;
    public Color unavailableColor = Color.red;

    SpriteRenderer plot;
    bool requirement = false;


    PlantObject selectedPlant;

    FarmManager fm;

    bool isDry = true;
    public Sprite drySprite;
    public Sprite normalSprite;
    public Sprite unavailableSprite;

    float speed=1f;
    public bool isBought=true;

    [ReadOnly()] public PlaceableObjectsData data = new PlaceableObjectsData();

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        plant = transform.GetChild(0).GetComponent<SpriteRenderer>();
        plantCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        fm = transform.parent.GetComponent<FarmManager>();
        plot = GetComponent<SpriteRenderer>();
        
        if (isBought)
        {
            plot.sprite = drySprite;
        }
        else
        {
            plot.sprite = unavailableSprite;
        }
        
    }

    public void Initailize(PlantObject pItem)
    {
        Debug.Log("Initailize");
        selectedPlant = pItem;
        data.assetName = selectedPlant.plantName;
        data.ID = SaveData.GenerateId();
    }

    public void Initailize(PlantObject pItem, PlaceableObjectsData objectsData)
    {
        selectedPlant = pItem;
        data = objectsData;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlanted && !isDry )//&& selectedPlant.buyPrice < 30)
        {
            timer -= speed*Time.deltaTime;

            if (timer < 0 && plantStage<selectedPlant.plantStages.Length-1 )
            {
                timer = selectedPlant.timeBtwStages;
                plantStage++;
                UpdatePlant();
            }
        }
    }

    private void OnMouseDown()
    {
        if (isPlanted)
        {
            
            if (plantStage == selectedPlant.plantStages.Length - 1 && !fm.isPlanting && !fm.isSelecting && selectedPlant.buyPrice < 30 )
            {
                Harvest();
            }
            else 
            {
                if(selectedPlant.plantName == "Cow")
                {
                    if(requirement)
                    {
                        Debug.Log("Milk");
                        requirement = false;
                        plantStage=0;
                        plant.sprite = selectedPlant.plantStages[plantStage];
                       
                    }
                    else{
                        Debug.Log("Food Require");
                        requirement = true;
                        plantStage=1;
                        plant.sprite = selectedPlant.plantStages[plantStage];
                    }
                }
                else if(selectedPlant.plantName == "Barn")
                {
                    if(requirement)
                    {
                        Debug.Log("Supply");
                        requirement = false;
                    }
                    else{
                        Debug.Log("Barn");
                        requirement = true;
                    }
                }
                
            }
            
        }
        else if(fm.isPlanting && fm.selectPlant.plant.buyPrice <= fm.money && (isBought ))
        {
            Plant(fm.selectPlant.plant);
        }
        if (fm.isSelecting)
        {
            switch (fm.selectedTool)
            {
                case 1:
                    if (isBought ) {
                        isDry = false;
                        plot.sprite = normalSprite;
                        if (isPlanted) UpdatePlant();
                    }
                    break;
                case 2:
                    if (fm.money>=10 && isBought)
                    {
                        fm.Transaction(-10);
                        if (speed < 2) speed += .2f;
                    }
                    break;
                case 3:
                    if (fm.money >= 100 && !isBought)
                    {
                        fm.Transaction(-100);
                        isBought = true;
                        plot.sprite = drySprite;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void OnMouseOver()
    {
        if (fm.isPlanting)
        {
            if(isPlanted || fm.selectPlant.plant.buyPrice > fm.money || !isBought)
            {
                //can't buy
                plot.color = unavailableColor;
            }
            else
            {
                //can buy
                plot.color = availableColor;
            }
        }
        if (fm.isSelecting)
        {
            switch (fm.selectedTool)
            {
                case 1:
                case 2:
                    if (isBought && fm.money>=(fm.selectedTool-1)*10)
                    {
                        plot.color = availableColor;
                    }
                    else
                    {
                        plot.color = unavailableColor;
                    }
                    break;
                case 3:
                    if (!isBought && fm.money>=100)
                    {
                        plot.color = availableColor;
                    }
                    else
                    {
                        plot.color = unavailableColor;
                    }
                    break;
                default:
                    plot.color = unavailableColor;
                    break;
            }
        }
    }

    private void OnMouseExit()
    {
        plot.color = Color.white;
    }


    void Harvest()
    {
        isPlanted = false;
        plant.gameObject.SetActive(false);
        fm.Transaction(selectedPlant.sellPrice);
        isDry = true;
        plot.sprite = drySprite;
        speed = 1f;
         
        
        ProgressBar.instance.targetProgress = ProgressBar.instance.slider.value + Progress;
       // if(ProgressBar.instance.slider.value < ProgressBar.instance.targetProgress)
            ProgressBar.instance.slider.value += ProgressBar.instance.FillSpeed * Time.deltaTime;
    }

    void Plant(PlantObject newPlant)
    {
        selectedPlant = newPlant;
        isPlanted = true;

        fm.Transaction(-selectedPlant.buyPrice);

        plantStage = 0;
        UpdatePlant();
        timer = selectedPlant.timeBtwStages;
        plant.gameObject.SetActive(true);
        var obj = plant.gameObject;
         obj.GetComponent<PlotManager>().Initailize(item);
    }

    void UpdatePlant()
    {
        if (isDry)
        {
            plant.sprite = selectedPlant.dryPlanted;
        }
        else
        {
            plant.sprite = selectedPlant.plantStages[plantStage];
        }
        plantCollider.size = plant.sprite.bounds.size;
        plantCollider.offset = new Vector2(0,plant.bounds.size.y/2);
    }

    private void OnApplicationQuit() {
        Debug.Log("OnApplicationQuit");
        data.position = transform.position;
        FarmManager.instance.saveData.AddData(data);
    }

}
