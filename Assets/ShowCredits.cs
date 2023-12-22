using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCredits : MonoBehaviour
{
    public GameObject creditsPanel;
    public Button showCreditsButton;

    private bool isCreditsVisible = false;

    private void Start()
    {
        creditsPanel.SetActive(false);
        showCreditsButton.onClick.AddListener(ToggleCredits);
    }

    private void ToggleCredits()
    {
        isCreditsVisible = !isCreditsVisible;
        creditsPanel.SetActive(isCreditsVisible);
        if (isCreditsVisible)
        {
            // Set the text to your credits content
            Text creditsText = creditsPanel.GetComponentInChildren<Text>();
            creditsText.text = "Cheshta Khanna - Graphics Designer\n" +
                               "Ankita Maity - Graphics Designer\n" +
                               "Abhinav Thakur - Game Programmer, Game Designer";
        }
    }
}
