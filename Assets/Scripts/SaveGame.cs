using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveGame : MonoBehaviour
{
    [SerializeField] private GameObject character;

    public void Submit()
    {
        // Check if the character is assigned before attempting to save
        if (character != null)
        {
            string prefabPath = "Assets/Prefabs/Player1Pos.prefab";

            // Create a prefab at the specified path
            PrefabUtility.SaveAsPrefabAsset(character, prefabPath);
            Debug.Log("Character prefab saved at: " + prefabPath);

            // Load the "Level 1" scene
            SceneManager.LoadScene("Level 1");
        }
        else
        {
            Debug.LogWarning("Character prefab is not assigned.");
        }
    }
}