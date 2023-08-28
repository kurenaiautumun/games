using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementTouchBased : MonoBehaviour
{
    public float speed = 8f;
    public Animator anim;
    public GameObject PlayerPos;
    public Transform groundCheckObject;
    public LayerMask groundLayer;
    public GameObject weaponSprite;

    private Vector3 target;
    private bool isHurt = false;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isPlayerDead = false; // Flag to track if the player is dead
    private bool isWeaponActive = false;

    [SerializeField] private Button punchButton;
    [SerializeField] private Button upperCutButton;

    public int playerHealth = 100;

    private void Start()
    {
        target = PlayerPos.transform.position;
        weaponSprite.SetActive(false);
        punchButton.onClick.AddListener(TriggerPunch);
        upperCutButton.onClick.AddListener(TriggerUpperCut);
        anim.SetBool("isWeaponActive", isWeaponActive);
    }


    private void FixedUpdate()
    {
        if (isPlayerDead)
        {
            // Player is dead, stop all animations and actions
            return;
        }

        if (isMoving && !isAttacking)
        {
            if (IsGrounded())
            {
                float step = speed * Time.fixedDeltaTime;
                PlayerPos.transform.position = Vector3.MoveTowards(PlayerPos.transform.position, target, step);
            }

            if (Vector3.Distance(PlayerPos.transform.position, target) < 0.01f)
            {
                isMoving = false;
            }
        }

        if (playerHealth <= 0 && !isDead)
        {
            anim.SetBool("isDead", true);
            isMoving = false;
            isDead = true;
            isPlayerDead = true;
            return;
        }

        if (isHurt)
        {
            anim.SetBool("isHurt", true);
            isHurt = false;
        }

        if (!isAttacking && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.z = PlayerPos.transform.position.z;
                isMoving = true;
            }

            anim.SetBool("isMoving?", isMoving);
        }
    }

    private void ToggleWeaponVisibility()
    {
        isWeaponActive = !isWeaponActive;
        weaponSprite.SetActive(isWeaponActive);
        anim.SetBool("isWeaponActive", isWeaponActive);

        if (isWeaponActive)
        {
            punchButton.onClick.RemoveAllListeners();
            punchButton.onClick.AddListener(TriggerAxeSwing);
            upperCutButton.onClick.RemoveAllListeners();
            upperCutButton.onClick.AddListener(TriggerAxeCut);
        }
        else
        {
            punchButton.onClick.RemoveAllListeners();
            punchButton.onClick.AddListener(TriggerPunch);
            upperCutButton.onClick.RemoveAllListeners();
            upperCutButton.onClick.AddListener(TriggerUpperCut);
        }
    }

    private void TriggerAxeSwing()
    {
        if (isPlayerDead)
            return;

        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("AxeSwingTrigger");
            StartCoroutine(WaitForAttackAnimation("AxeSwingTrigger"));
            EnemyAI enemyAI = FindObjectOfType<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(25);
            }
        }
    }

    private void TriggerAxeCut()
    {
        if (isPlayerDead)
            return;

        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("AxeCutTrigger");
            StartCoroutine(WaitForAttackAnimation("AxeCutTrigger"));
            EnemyAI enemyAI = FindObjectOfType<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(35);
            }
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckObject.position, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private void TriggerPunch()
    {
        if (isPlayerDead)
            return;

        string attackTrigger = isWeaponActive ? "AxeSwingTrigger" : "PunchTrigger";

        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger(attackTrigger);
            StartCoroutine(WaitForAttackAnimation(attackTrigger));
            EnemyAI enemyAI = FindObjectOfType<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(isWeaponActive ? 25 : 15);
            }
        }
    }

    private void TriggerUpperCut()
    {
        if (isPlayerDead)
            return;

        string attackTrigger = isWeaponActive ? "AxeCutTrigger" : "UpperCutTrigger";

        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger(attackTrigger);
            StartCoroutine(WaitForAttackAnimation(attackTrigger));
            EnemyAI enemyAI = FindObjectOfType<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(isWeaponActive ? 35 : 25);
            }
        }
    }

    private IEnumerator WaitForAttackAnimation(string triggerName)
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
        anim.ResetTrigger(triggerName);
    }

    public void TakeDamage(int damage)
    {
        if (isPlayerDead)
            return;

        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            anim.SetBool("isDead", true);
            StartCoroutine(DestroyAfterAnimation());
        }
        else
        {
            anim.SetTrigger("HurtTrigger");
            StartCoroutine(WaitAndPlayHurtAnimation());
        }
    }

    private IEnumerator WaitAndPlayHurtAnimation()
    {
        if (!isPlayerDead)
        {
            yield return new WaitForSeconds(1.5f);
            anim.SetTrigger("HurtTrigger");
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
