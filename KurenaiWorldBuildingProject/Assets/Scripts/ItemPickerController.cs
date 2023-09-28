using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemPickerController : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    [SerializeField] private List<GameObject> itemsList;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();

        foreach(GameObject item in itemsList)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            GridObject gridObject = item.GetComponent<GridObject>();

            optionData.text = gridObject.GetName();
            optionData.image = item.GetComponent<SpriteRenderer>().sprite;

            dropdown.options.Add(optionData);
        }

        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    public GameObject GetItem(int id = -1)
    {
        if(id >= 0 && id < itemsList.Count) return itemsList[id];
        if (dropdown.value == -1) return null;
        return itemsList[dropdown.value];
    }

    public int GetCurrentIndex()
    {
        return dropdown.value;
    }
}
