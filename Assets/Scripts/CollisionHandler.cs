using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Player"))
        {
            // Stop the player's and enemy's Rigidbody 2D components
            Rigidbody2D playerRigidbody = GetComponent<Rigidbody2D>();
            Rigidbody2D enemyRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

            playerRigidbody.velocity = Vector2.zero;
            enemyRigidbody.velocity = Vector2.zero;
        }
    }
}