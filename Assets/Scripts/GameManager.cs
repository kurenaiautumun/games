using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI gameOverText;
    private bool gameStarted = false;
    private float gameTime = 60f; // Total game time in seconds
    private bool isGameOver = false;
    private bool isBackgroundSelected = false;

    private MovementTouchBased playerScript;
    private EnemyAI enemyScript;

    private void Start()
    {
        Time.timeScale = 0;
        playerScript = FindObjectOfType<MovementTouchBased>();
        enemyScript = FindObjectOfType<EnemyAI>();
        StartCoroutine(StartCountdown());
    }

    private void Update()
    {
        if (gameStarted && !isGameOver && isBackgroundSelected)
        {
            if (playerScript.playerHealth <= 0 || enemyScript.enemyHealth <= 0)
            {
                // Pause the game if either the player or enemy's health reaches 0
                gameTime = 0;
                EndGame();
            }
            else
            {
                gameTime -= Time.unscaledDeltaTime;

                if (gameTime <= 0)
                {
                    gameTime = 0;
                    EndGame("Time's up!");
                }

                int secondsRemaining = Mathf.CeilToInt(gameTime);
                countdownText.text = secondsRemaining.ToString();
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        countdownText.text = "Get Ready!";
        yield return new WaitForSecondsRealtime(5f);
        countdownText.text = "";
        isBackgroundSelected = true;
        ResumeGame();
        gameStarted = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    private void EndGame(string result = "It's a Draw!")
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);

        if (playerScript.playerHealth <= 0)
        {
            result = "Enemy Wins!";
        }
        else if (enemyScript.enemyHealth <= 0)
        {
            result = "Player Wins!";
        }

        gameOverText.text = "GAME OVER\n" + result;
    }
}
