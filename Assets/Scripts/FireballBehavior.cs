using UnityEngine;

public class FireballBehavior : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 30;
    public float raycastDistance = 0.1f; // Adjust this distance as needed.

    private void Start()
    {
        // Activate the fireball GameObject when it's created.
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        // Move the fireball in a straight line
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Perform a raycast to check for collisions
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, raycastDistance);

        if (hit.collider != null)
        {
            // Check if the ray hit an enemy
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Fireball hit an enemy!");
                // Deal damage to the enemy and destroy the fireball
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log("Dealing damage to the enemy.");
                }
                Destroy(gameObject);
                Debug.Log("Destroying the fireball.");
            }
        }
    }
}
