using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    public void BackToMainMenuScene()
    {
        SceneManager.LoadScene(2); // Load the main menu scene.
    }
}