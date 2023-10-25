using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // You can assign these in the Unity Inspector.
    public string arenaSceneName = "Scene1";            // The name of your Arena scene.
    public string characterCreatorSceneName = "Scene0"; // The name of your Character Creator scene.

    public void LoadSavedCharacterScene()
    {
        SceneManager.LoadScene(3); // Load the SavedCharacterScene.
    }

    public void LoadArenaScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadCharacterCreatorScene()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
