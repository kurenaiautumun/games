using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject DoubleFire;
    public GameObject FlashHit;
    public GameObject GensugaTensho;
    public GameObject FrameWheel;
    public GameObject PowerSlash;
    public GameObject AxeHit;
    public GameObject Contact;
    public Transform attackPoint;
    public Animator playerAnimator; // Reference to the player's Animator component.
     public float attackCooldown = 2f; // Cooldown between general attacks (seconds)
    public float specificAttackCooldown = 3f; // Cooldown between specific attacks (seconds)

      private bool CanAttack(float cooldown)
    {
        if (Time.time - cooldown >= 1f) // Adjust cooldown duration as needed (5 seconds in this example)
        {
            return true;
        }
        else
        {
            Debug.Log("Attack is on cooldown.");
            return false;
        }
    }
    private void ResetCooldown(ref float cooldown, float cooldownDuration)
    {
        cooldown = Time.time + cooldownDuration; // Set the cooldown to the current time plus the duration
    }
    public void ShootFireball()
    {
        if (CanAttack(attackCooldown))
        {
        if (fireballPrefab != null && attackPoint != null)
        {
            // Trigger the "isShooting" animation
            playerAnimator.SetTrigger("isShooting");

            // Instantiate the fireball at the attack point
            GameObject fireball = Instantiate(fireballPrefab, attackPoint.position, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();
        }
         ResetCooldown(ref attackCooldown,2f);
        }
    }
    public void DoubleFiring()
    {
          if (CanAttack(specificAttackCooldown))
        {
         // Trigger the "isShooting" animation
            playerAnimator.SetTrigger("fire");

            // Instantiate the fireball at the attack point
            GameObject fireball = Instantiate(DoubleFire, attackPoint.position, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();

           
              ResetCooldown(ref attackCooldown,2f);
        }
    }
     public void FlashHitting()
    {
         if (CanAttack(specificAttackCooldown))
        {
         // Trigger the "isShooting" animation
            playerAnimator.SetTrigger("fire");

            // Instantiate the fireball at the attack point
            GameObject fireball = Instantiate(FlashHit, attackPoint.position, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();

             ResetCooldown(ref attackCooldown,2f);
        }    
    }
    public void Getsuga()
    {
         if (CanAttack(specificAttackCooldown))
        {
         // Trigger the "isShooting" animation
            playerAnimator.SetTrigger("fire");

            // Instantiate the fireball at the attack point
            GameObject fireball = Instantiate(GensugaTensho, attackPoint.position, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();

             ResetCooldown(ref attackCooldown,2f);
        }
    }
     public void Frame()
    {
         if (CanAttack(specificAttackCooldown))
        {
         // Trigger the "isShooting" animation
            playerAnimator.SetTrigger("fire");

            // Instantiate the fireball at the attack point
            GameObject fireball = Instantiate(FrameWheel, attackPoint.position, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();
            ResetCooldown(ref attackCooldown,2f);
        }
           
    }
     public void Power()
    {
         if (CanAttack(specificAttackCooldown))
        {
         // Trigger the "isShooting" animation
            playerAnimator.SetTrigger("fire");

            // Instantiate the fireball at the attack point
            GameObject fireball = Instantiate(PowerSlash, attackPoint.position, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();
            ResetCooldown(ref attackCooldown,5f);
        }
    }
     public void Axe()
    {
         if (CanAttack(specificAttackCooldown))
        {
         // Trigger the "isShooting" animation
            playerAnimator.SetTrigger("fire");

            // Instantiate the fireball at the attack point
            GameObject fireball = Instantiate(AxeHit, attackPoint.position, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();
            ResetCooldown(ref attackCooldown,5f);
        }
    }
    
}
