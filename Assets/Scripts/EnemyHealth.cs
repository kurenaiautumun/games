using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Animator playerAnimator;

    public int HP = 100;

    // Called to apply damage to the player
    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            Die();
        }
    }

    // Handles the death of the player
    void Die()
    {
        // Trigger the death animation
        playerAnimator.SetTrigger("Die");

        // Disable further collisions and interactions
        GetComponent<Collider>().enabled = false;

        // Optionally, you can disable other scripts or components here

        // Start a coroutine to destroy the player after the death animation
        StartCoroutine(DestroyAfterAnimation());
    }

    // Coroutine to destroy the player after the death animation
    IEnumerator DestroyAfterAnimation()
    {
        // Wait for the duration of the death animation
        yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Destroy the player GameObject after the animation
        Destroy(gameObject);
    }
}