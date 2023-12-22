using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3.0f;
    public float attackRange = 4f;
    public float attackCooldown = 2.0f;
    public int enemyHealth = 100;
    public GameObject enemy;
    public Animator animator;
    private bool canAttack = true;
    private bool playerInRange = false;
    public bool isDead = false;
    private bool isHurt = false;
    public bool isAttacking = false;
    public EnemyHealthBar enemyHealthBar;
    private float hurtAnimationLength = 2.0f;
    private float attackAnimationLength = 0.81666667f;
    private MovementTouchBased playerScript;

    public bool isSpecialAttackTriggered = false;
    private float specialAttackDelay = 2.0f;
    private bool canPerformSpecialAttack = true;
    private bool isSpecialAttackPlaying = false;
    private float specialAttackAnimationLength = 1.15f;

    private bool usedSpecialAttack = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Health", enemyHealth);
        animator.SetBool("isMoving?", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isSpecialAttacking", false);
        isAttacking = false;
        playerScript = FindObjectOfType<MovementTouchBased>();
        enemyHealthBar = FindObjectOfType<EnemyHealthBar>();
        // Set this enemy as the target for the health bar.
        if (enemyHealthBar != null)
        {
            enemyHealthBar.SetEnemyAI(this);
        }
    }




    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        animator.SetBool("isMoving?", false);
        canAttack = true;
    }

    private IEnumerator SpecialAttackCooldown()
    {
        yield return new WaitForSeconds(specialAttackDelay);
        canPerformSpecialAttack = true;
    }

    private void FixedUpdate()
    {
        if (isDead || player == null || (playerScript != null && playerScript.playerHealth <= 0))
        {
            isAttacking = false;
            isSpecialAttackPlaying = false;
            animator.SetBool("isAttacking", isAttacking);
            animator.SetBool("isSpecialAttacking", isSpecialAttackPlaying);

            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        playerInRange = distanceToPlayer <= attackRange;

        if (playerInRange)
        {
            if (canAttack && !isSpecialAttackTriggered)
            {
                if (!isAttacking && enemyHealth > 0.5f * 100)
                {
                    Attack();
                }
                else if (!isAttacking && enemyHealth <= 0.5f * 100 && canPerformSpecialAttack && !usedSpecialAttack)
                {
                    PerformSpecialAttack();
                }
            }
        }
        else
        {
            if (!isAttacking && !isSpecialAttackPlaying)
            {
                MoveTowardsPlayer();
            }
        }

        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isSpecialAttacking", isSpecialAttackPlaying);
        Debug.Log("Distance to player: " + distanceToPlayer);


    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        transform.Translate(direction * moveSpeed * Time.deltaTime);

        if (!isAttacking && !isSpecialAttackPlaying)
        {
            animator.SetBool("isMoving?", true);
        }
    }

    private void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("AttackTrigger");

        canAttack = false;
        StartCoroutine(AttackCooldown());

        // Deal damage to the player
        MovementTouchBased playerScript = FindObjectOfType<MovementTouchBased>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(20);
        }

        StartCoroutine(ResetAttackState());
    }

    public void PerformSpecialAttack()
    {
        usedSpecialAttack = true;

        // Trigger the special attack animation
        isSpecialAttackTriggered = true;
        isSpecialAttackPlaying = true;

        canPerformSpecialAttack = false;
        StartCoroutine(SpecialAttackCooldown());

        // Deal damage to the player with the special attack based on the trigger
        MovementTouchBased playerScript = FindObjectOfType<MovementTouchBased>();
        if (playerScript != null)
        {
            int damage = 0;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpecialAttack1"))
            {
                damage = 25; // First trigger
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpecialAttack2"))
            {
                damage = 35; // Second trigger
            }

            playerScript.TakeDamage(damage);
        }

        StartCoroutine(ResetSpecialAttackState());
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(attackAnimationLength);
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
    }

    private IEnumerator ResetSpecialAttackState()
    {
        yield return new WaitForSeconds(specialAttackAnimationLength);

        isSpecialAttackTriggered = false;
        isSpecialAttackPlaying = false;

        // After the special attack animation finishes, wait for 2 seconds before attacking normally
        yield return new WaitForSeconds(2.0f);
        usedSpecialAttack = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isHurt)
            return;

        enemyHealth -= damage;
        animator.SetFloat("Health", enemyHealth);

        // Update the enemy health bar.
        if (enemyHealthBar != null)
        {
            enemyHealthBar.UpdateHealth(enemyHealth);
        }

        if (enemyHealth <= 0 && !isDead)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            StartCoroutine(DestroyAfterDeathAnimation());
        }
        else
        {
            isHurt = true;
            animator.SetTrigger("Hurt");
            StartCoroutine(ResetHurtState());
        }
    }


    private IEnumerator ResetHurtState()
    {
        yield return new WaitForSeconds(hurtAnimationLength);
        isHurt = false;
    }

    private IEnumerator DestroyAfterDeathAnimation()
    {
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.transform;
            playerInRange = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void OnAnimationExit()
    {
        StartCoroutine(ResetHurtState());
        StartCoroutine(ResetAttackState());
    }


}
