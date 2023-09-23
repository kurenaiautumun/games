using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization;

[Serializable] 
public class SaveData 
{
    public static int IdCount;

    public Dictionary<string, PlaceableObjectsData> placeableobjectDatas =
        new Dictionary<string, PlaceableObjectsData>();

    public static string GenerateId()
    {
        IdCount++;
        return IdCount.ToString();
    }

    public void AddData(Data data)
    {
        if(data is PlaceableObjectsData plObjData)
        {
            if(placeableobjectDatas.ContainsKey(plObjData.ID))
            {
                placeableobjectDatas[plObjData.ID] = plObjData;
            }
            else
            {
                placeableobjectDatas.Add(plObjData.ID, plObjData);
            }
        }
    }

    public void RemoveData(Data data)
    {
        if(data is PlaceableObjectsData plObjData)
        {
            if(placeableobjectDatas.ContainsKey(plObjData.ID))
            {
                placeableobjectDatas.Remove(plObjData.ID);
            }
        }
    }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            placeableobjectDatas ??= new Dictionary<string, PlaceableObjectsData>();
        }
    
}
