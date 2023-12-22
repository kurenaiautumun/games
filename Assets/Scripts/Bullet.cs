using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBullet : MonoBehaviour
{
    public float life = 1;
    public int damageAmount = 100;
    KillCount killcounterScript;

    private void Start()
    {
        killcounterScript = GameObject.Find("KCO").GetComponent<KillCount>();
    }

    void Awake()
    {
        Destroy(gameObject, life);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if colliding with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            PlayerHealth enemyHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }

            // Increment kill count
            killcounterScript.Addkill();
        }

        // Destroy the bullet in all cases
        Destroy(gameObject);
    }
}