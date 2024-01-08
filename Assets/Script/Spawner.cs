using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    [Header("Jobs")]
    public GameObject PanelPrefabJOBS;
    public int MaxJobs;
    public GameObject Prefab_Jobs;
    public List<GameObject> jobs;
    [Header("Fooditems")]
    public GameObject PanelPrefabFOODITEMS;
    public int Maxfooditems;
    public GameObject Prefab_fooditems;
    public List<GameObject> fooditems;
    

    public int i;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Spwaner");
        Spawner_job();
        Spawner_fooditems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void Spawner_job()
    {
        for ( i = 0; i < MaxJobs; i++)
        {
            if (jobs.Count == MaxJobs) return;
            GameObject jobspanels = Instantiate(Prefab_Jobs);
            jobspanels.transform.parent = PanelPrefabJOBS.transform;
            jobspanels.transform.localScale = Vector3.one;
            jobs.Add(jobspanels);
            
        }

    }
    public void Spawner_fooditems()
    {
        for(int i = 0; i < Maxfooditems; i++)
        {
            if (fooditems.Count == Maxfooditems) return;
            GameObject fooditmespanels = Instantiate(Prefab_fooditems);
            fooditmespanels.transform.SetParent(PanelPrefabFOODITEMS.transform, false);
            fooditmespanels.transform.localScale = Vector3.one;
            fooditems.Add(fooditmespanels);
        }
    }


}

