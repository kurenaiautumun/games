using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform attackPoint;
    public Animator playerAnimator; // Reference to the player's Animator component.

    public void ShootFireball()
    {
        if (fireballPrefab != null && attackPoint != null)
        {
            // Trigger the "isShooting" animation
            playerAnimator.SetTrigger("isShooting");

            // Instantiate the fireball at the attack point
            GameObject fireball = Instantiate(fireballPrefab, attackPoint.position, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();

            // Set any additional properties for the fireball (e.g., speed, damage)
            if (fireballBehavior != null)
            {
                // Customize fireball properties if needed
            }
        }
    }
}
