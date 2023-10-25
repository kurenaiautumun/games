using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthBarImage;
    public TextMeshProUGUI healthText;
    public float maxHealth = 100f;
    public Sprite fullHealthBarSprite;
    public Sprite emptyHealthBarSprite;
    private EnemyAI enemyAI; // Reference to the EnemyAI script.

    // Add a method to set the enemy AI.
    public void SetEnemyAI(EnemyAI enemy)
    {
        enemyAI = enemy;
    }

    public void UpdateHealth(float currentHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        healthBarImage.fillAmount = fillAmount;

        if (fillAmount <= 0.0f)
        {
            healthBarImage.sprite = emptyHealthBarSprite;
        }
        else
        {
            healthBarImage.sprite = fullHealthBarSprite;
        }

        healthText.text = currentHealth.ToString("0");
    }

}