using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacterPrefab : MonoBehaviour
{
    public string prefabPath = "Prefabs/Player1Pos"; // Path to your prefab within the Resources folder.
    public Transform spawnPosition; // The position where you want to instantiate the prefab.

    private void Start()
    {
        // Load the saved character prefab from the Resources folder.
        GameObject savedCharacterPrefab = Resources.Load<GameObject>(prefabPath);

        if (savedCharacterPrefab != null)
        {
            // Instantiate the saved character prefab at the specified position.
            Instantiate(savedCharacterPrefab, spawnPosition.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab not found at path: " + prefabPath);
        }
    }
}
