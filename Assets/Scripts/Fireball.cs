using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 30;

    private Transform target;
    private Transform startingPosition; // New variable to specify the starting position

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // New method to set the starting position externally
    public void SetStartingPosition(Transform newPosition)
    {
        startingPosition = newPosition;
        transform.position = startingPosition.position; // Set the fireball's position to the starting position
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
