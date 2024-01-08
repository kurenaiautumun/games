using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class Bowl_movement : MonoBehaviour
{
    public Canvas canvas;
    


    void Start()
    {
        Debug.Log("Start Bowl");
    }
   
     
    public void draghandler(BaseEventData data)
    {
        PointerEventData pointerdata = (PointerEventData)data;
         Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,pointerdata.position,canvas.worldCamera,out position);

        transform.position = new Vector2 (canvas.transform.TransformPoint(position).x , 500);


    }
    void Update()
    {
        
    }
}
