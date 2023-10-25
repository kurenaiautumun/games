using UnityEngine;

public class WeaponSelectionPanel : MonoBehaviour
{
    private MovementTouchBased playerController; // Reference to the player controller script

    private void Start()
    {
        // Find the player controller script in the scene
        playerController = FindObjectOfType<MovementTouchBased>();

        // Hide the weapon selection panel initially
        gameObject.SetActive(false);
    }

    public void OpenWeaponSelection()
    {
        // Open the weapon selection panel
        gameObject.SetActive(true);

        // Pause the game or disable player controls while the panel is open if needed
        playerController.enabled = false;
    }

    public void CloseWeaponSelection()
    {
        // Close the weapon selection panel
        gameObject.SetActive(false);

        // Resume the game or enable player controls when the panel is closed
        playerController.enabled = true;
    }
}