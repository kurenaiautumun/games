using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class StoreManager : MonoBehaviour
{
    
    public GameObject plantItem;
    List<PlantObject> plantObjects = new List<PlantObject>();
    public GameObject planting;
    PlantObject plobj;

    
    private void Awake()
    {
        //Assets/Resources/Plants
        var loadPlants = Resources.LoadAll("Plants", typeof(PlantObject));
        foreach (var plant in loadPlants)
        {
            plantObjects.Add((PlantObject)plant);
        }
        plantObjects.Sort(SortByPrice);

        foreach (var plant in plantObjects)
        {
            plantItem.SetActive(true);
            PlantItem newPlant = Instantiate(plantItem, transform).GetComponent<PlantItem>();
            
            newPlant.plant = plant;
        }
    }
    private void Start() {
        planting = GameObject.Find("Plot(270)");
        var o = planting.GetComponent<PlotManager>();
        Debug.Log(FindObjectsOfType<PlantItem>().Length);
        var p1 = FindObjectsOfType<PlantItem>();
        foreach (var p in p1)
        {
            Debug.Log(p);
           
            Debug.Log(p.plant);
             o.Plant(selectedPlant.plant);
        }
        
        Debug.Log(planting);
    }

    

    
    int SortByPrice(PlantObject plantObject1, PlantObject plantObject2)
    {
        return plantObject1.buyPrice.CompareTo(plantObject2.buyPrice);
    }

    int SortByTime(PlantObject plantObject1, PlantObject plantObject2)
    {
        return plantObject1.timeBtwStages.CompareTo(plantObject2.timeBtwStages);
    }

    
}
