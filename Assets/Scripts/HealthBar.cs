using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // Reference to the UI Image component.
    public TextMeshProUGUI healthText; // Reference to the TextMeshPro text element.
    public float maxHealth = 100f; // Maximum health value for the player.

    public Sprite fullHealthBarSprite; // Assign your full health bar sprite in the Inspector.
    public Sprite emptyHealthBarSprite; // Assign your empty health bar sprite in the Inspector.

    // Create a method to update the health bar based on the player's health.
    public void UpdateHealth(float currentHealth)
    {
        // Calculate the fill amount based on the player's health.
        float fillAmount = currentHealth / maxHealth;
        healthBarImage.fillAmount = fillAmount;

        // Update the health bar image based on the fill amount.
        if (fillAmount <= 0.0f)
        {
            // Player is out of health, set the empty bar sprite
            healthBarImage.sprite = emptyHealthBarSprite;
        }
        else
        {
            // Player has health, set the full bar sprite
            healthBarImage.sprite = fullHealthBarSprite;
        }

        // Update the health text with the current health value
        healthText.text = currentHealth.ToString("0");
    }
}
