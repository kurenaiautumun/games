using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;

    public float FillSpeed = 0.5f;
    public float targetProgress;

    public static ProgressBar instance;

    private void Awake() {
        slider = gameObject.GetComponent<Slider>();
        instance =this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
