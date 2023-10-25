using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public GameObject background1; // Reference to the first background
    public GameObject background2; // Reference to the second background

    public GameObject selectionPanel; // Reference to the selection panel

    public Button buttonBackground1; // Reference to the "Background 1" button
    public Button buttonBackground2; // Reference to the "Background 2" button

    private GameManager gameManager; // Reference to the GameManager script

    private void Start()
    {
        // Attach button click events to your functions
        buttonBackground1.onClick.AddListener(SelectBackground1);
        buttonBackground2.onClick.AddListener(SelectBackground2);

        // Find and store a reference to the GameManager script
        gameManager = FindObjectOfType<GameManager>();

        // Initially, set one of the backgrounds as active based on your choice.
        SetActiveBackground(1); // Set the first background as active

        // Hide the selection panel at the start
        selectionPanel.SetActive(true);
    }

    // Function to set the active background
    public void SetActiveBackground(int backgroundChoice)
    {
        // Deactivate both backgrounds to avoid showing both at the same time.
        background1.SetActive(false);
        background2.SetActive(false);

        // Activate the selected background based on the player's choice.
        if (backgroundChoice == 1)
        {
            background1.SetActive(true);
        }
        else if (backgroundChoice == 2)
        {
            background2.SetActive(true);
        }

        // Signal the GameManager to unpause the game
        if (gameManager != null)
        {
            gameManager.ResumeGame();
        }

        // Hide the selection panel
        selectionPanel.SetActive(false);
    }

    // Button click handlers
    public void SelectBackground1()
    {
        SetActiveBackground(1); // Set the first background as active when the button is clicked
    }

    public void SelectBackground2()
    {
        SetActiveBackground(2); // Set the second background as active when the button is clicked
    }
}
