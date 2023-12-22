using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject werewolfPrefab;
    public GameObject dragonPrefab;
    public Transform player;
    public Transform werewolfSpawnPoint;
    public Transform dragonSpawnPoint;
    public Transform enemyParent; // Parent transform for the spawned enemies

    private GameObject spawnedEnemy;

    private void Start()
    {
        // Ensure the selected enemy is initially inactive in the Hierarchy
        GameObject selectedEnemy = Random.Range(0f, 1f) < 0.5f ? werewolfPrefab : dragonPrefab;
        selectedEnemy.SetActive(false);

        // Determine the spawn point based on the selected enemy
        Transform spawnPoint = selectedEnemy == werewolfPrefab ? werewolfSpawnPoint : dragonSpawnPoint;

        // Spawn the selected enemy at the chosen spawn point
        spawnedEnemy = Instantiate(selectedEnemy, spawnPoint.position, Quaternion.identity);

        // Set the tag of the spawned enemy to "Enemy"
        spawnedEnemy.tag = "Enemy";

        // Get the EnemyAI component from the spawned enemy
        EnemyAI enemyAI = spawnedEnemy.GetComponent<EnemyAI>();

        // Set the player reference for the enemy AI
        enemyAI.SetPlayer(player);

        // Make sure the local scale is not modified
        spawnedEnemy.transform.localScale = Vector3.one;

        // Set the parent for the spawned enemy to maintain its position and scale
        if (enemyParent != null)
        {
            spawnedEnemy.transform.parent = enemyParent;
        }

        // Enable the spawned enemy immediately
        spawnedEnemy.SetActive(true);
        enemyAI.enemy = spawnedEnemy;
    }
}
