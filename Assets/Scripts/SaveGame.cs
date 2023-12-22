using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveGame : MonoBehaviour
{
    [SerializeField] private GameObject character;

    public void Submit()
    {
        // Check if the character is assigned before attempting to save
        if (character != null)
        {
            string prefabPath = "Assets/Prefabs/Player1Pos.prefab";

            // Check if we are in the Unity Editor before using PrefabUtility
#if UNITY_EDITOR
            PrefabUtility.SaveAsPrefabAsset(character, prefabPath);
            Debug.Log("Character prefab saved at: " + prefabPath);
#else
            Debug.LogWarning("Prefab saving is only available in the Unity Editor.");
#endif

            // Load the "Level 1" scene
            SceneManager.LoadScene("Level 1");
        }
        else
        {
            Debug.LogWarning("Character prefab is not assigned.");
        }
    }
}