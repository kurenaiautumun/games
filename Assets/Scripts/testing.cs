using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class testing : MonoBehaviour
{
    public float speed = 8f;
    public Animator anim;
    public GameObject PlayerPos;
    public GameObject enemy; // Declare the enemy variable
    private Vector3 target;
     private bool isHurt = false;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isPlayerDead = false;
    private int currentWeaponIndex = -1; // Track the current weapon index (-1 for no weapon)
    private bool isWeaponSelectionOpen = false;
    public HealthBar healthBar;
    [SerializeField] private Button punchButton;
    [SerializeField] private Button upperCutButton;
    [SerializeField] private Button weaponButton;
    [SerializeField] private GameObject weaponSelectionPanel; // Reference to the weapon selection panel
    [SerializeField] private Button deselectButton; // Reference to the "Deselect" button
    public int playerHealth = 100;
      public Transform groundCheckObject;
    public LayerMask groundLayer;
    public GameObject[] weaponSprites; // Array to hold weapon sprites
    public string[] attackTriggers;    // Array to hold attack triggers for each weapon
    private bool isGamePaused = false;
     public float attackRange = 1.0f;

     private void Start()
    {
        target = PlayerPos.transform.position;
        punchButton.onClick.AddListener(TriggerPunch);
        upperCutButton.onClick.AddListener(TriggerUpperCut);
        weaponButton.onClick.AddListener(ToggleWeaponSelection);
        deselectButton.onClick.AddListener(DeselectWeapon);


        // Center the weapon selection panel
        CenterWeaponSelectionPanel();

        // Initialize the weapon selection panel as closed
        CloseWeaponSelection();

        // Initialize attack triggers
        attackTriggers = new string[]
        {
            "PunchTrigger", "UpperCutTrigger",             // Default, no weapon
            "AxeCutTrigger", "AxeSwingTrigger",           // Axe
            "ShortSwordJabTrigger", "ShortSwordImpalerTrigger", // Sword
            "HammerTriggerOne", "HammerTriggerTwo",      // Hammer
            "GutsTriggerOne", "GutsTriggerTwo"           // Guts
        };
        SetWeapon(-1);
    }
    private void Update()
    {
        if (isGamePaused)
        {
            return; // If the game is paused, do not process input or movement
        }
        // Check if the player is dead
        if (isPlayerDead)
        {
            // Player is dead, stop all animations and actions
            return;
        }
        
        if (isMoving && !anim.GetCurrentAnimatorStateInfo(0).IsName("AttackAnimation"))
        {
            float step = speed * Time.deltaTime;
            PlayerPos.transform.position = Vector3.MoveTowards(PlayerPos.transform.position, target, step);

            if (Vector3.Distance(PlayerPos.transform.position, target) < 0.01f)
            {
                isMoving = false;
                anim.SetBool("isMoving?", false);
            }
        }

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                target = Camera.main.ScreenToWorldPoint(touch.position);
                target.z = PlayerPos.transform.position.z;
                isMoving = true;
                anim.SetBool("isMoving?", true);
            }
        }

        // Check for mouse input
        if (Input.GetKeyDown(KeyCode.W) && !isMoving)
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = PlayerPos.transform.position.z;
            isMoving = true;
            anim.SetBool("isMoving?", true);
        }
       if (isMoving && !isAttacking)
        {
            if (IsGrounded())
            {
                float step = speed * Time.deltaTime;
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
    
 public void NextWeapon()
    {
        // Increment the current weapon index and wrap around if needed
        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponSprites.Length)
        {
            currentWeaponIndex = 0;
        }

        // Set the current weapon
        SetWeapon(currentWeaponIndex);

        // Reset the attack triggers when changing weapons
        anim.ResetTrigger("PunchTrigger");
        anim.ResetTrigger("UpperCutTrigger");
    }

    private void CloseWeaponSelection()
    {
        isWeaponSelectionOpen = false;
        weaponSelectionPanel.SetActive(false);
    }

    private void SetWeapon(int weaponIndex)
    {
        // Disable all weapon sprites
        for (int i = 0; i < weaponSprites.Length; i++)
        {
            weaponSprites[i].SetActive(false);
        }

        // Enable the selected weapon sprite
        if (weaponIndex >= 0 && weaponIndex < weaponSprites.Length)
        {
            weaponSprites[weaponIndex].SetActive(true);
        }

        currentWeaponIndex = weaponIndex;
    }

    private void TriggerPunch()
    {
        if (isPlayerDead)
            return;

        if (!isAttacking)
        {
            isAttacking = true;

            // Determine the correct trigger based on the current weapon or the default punch
            int triggerIndex = (currentWeaponIndex >= 0 && currentWeaponIndex < weaponSprites.Length) ? currentWeaponIndex * 2 : 0;
            string attackTrigger = attackTriggers[triggerIndex];

            // Enable the Trail Renderer component for the current weapon or the default punch
            if (currentWeaponIndex >= 0 && currentWeaponIndex < weaponSprites.Length)
            {
                ToggleWeaponTrail(currentWeaponIndex, true);
            }

            // Check if the enemy is close enough before dealing damage
            float distanceToEnemy = Vector3.Distance(PlayerPos.transform.position, enemy.transform.position);
            if (distanceToEnemy <= attackRange)
            {
                anim.SetTrigger(attackTrigger);
                StartCoroutine(WaitForAttackAnimation(attackTrigger));
                EnemyAI enemyAI = FindObjectOfType<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(15); // Adjust the damage value as needed
                }
            }
            else
            {
                // The enemy is too far, so the attack should not deal damage
            }
        }
    }

    private void TriggerUpperCut()
    {
        if (isPlayerDead)
            return;

        if (!isAttacking)
        {
            isAttacking = true;

            // Determine the correct trigger based on the current weapon or the default uppercut
            int triggerIndex = (currentWeaponIndex >= 0 && currentWeaponIndex < weaponSprites.Length) ? currentWeaponIndex * 2 + 1 : 1;
            string attackTrigger = attackTriggers[triggerIndex];

            // Enable the Trail Renderer component for the current weapon or the default uppercut
            if (currentWeaponIndex >= 0 && currentWeaponIndex < weaponSprites.Length)
            {
                ToggleWeaponTrail(currentWeaponIndex, true);
            }

            // Check if the enemy is close enough before dealing damage
            float distanceToEnemy = Vector3.Distance(PlayerPos.transform.position, enemy.transform.position);
            if (distanceToEnemy <= attackRange)
            {
                anim.SetTrigger(attackTrigger);
                StartCoroutine(WaitForAttackAnimation(attackTrigger));
                EnemyAI enemyAI = FindObjectOfType<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(25); // Adjust the damage value as needed
                }
            }
            else
            {
                // The enemy is too far, so the attack should not deal damage
            }
        }
    }



    // Helper method to toggle the Trail Renderer component for the current weapon
    private void ToggleWeaponTrail(int weaponIndex, bool enable)
    {
        TrailRenderer weaponTrail = weaponSprites[weaponIndex].GetComponent<TrailRenderer>();
        if (weaponTrail != null)
        {
            weaponTrail.enabled = enable;
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
        healthBar.UpdateHealth(playerHealth);
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

    private void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f; // Set the time scale to 0 to pause the game
                             // Optionally, you can also disable other game-related components or input handling here
    }

    private void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f; // Set the time scale back to 1 to resume the game
                             // Optionally, you can re-enable any disabled components or input handling here
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckObject.position, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    // Center the weapon selection panel on the screen
    private void CenterWeaponSelectionPanel()
    {
        // Get the screen width and height
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Get the panel's RectTransform component
        RectTransform panelRectTransform = weaponSelectionPanel.GetComponent<RectTransform>();

    }

    // Open or close the weapon selection panel
    private void ToggleWeaponSelection()
    {
        isWeaponSelectionOpen = !isWeaponSelectionOpen;
        weaponSelectionPanel.SetActive(isWeaponSelectionOpen);

        if (isWeaponSelectionOpen)
        {
            // Pause the game when the weapon selection panel is open
            PauseGame();
            PopulateWeaponButtons();
        }
        else
        {
            // Resume the game when the weapon selection panel is closed
            ResumeGame();
        }
    }

    // Populate the weapon selection panel with weapon buttons
    private void PopulateWeaponButtons()
    {
        foreach (Transform child in weaponSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < weaponSprites.Length; i++)
        {
            int weaponIndex = i;
            string weaponName = weaponSprites[i].name;

            GameObject weaponButton = new GameObject("WeaponButton_" + weaponName);
            weaponButton.transform.SetParent(weaponSelectionPanel.transform);
            weaponButton.AddComponent<RectTransform>();
            Button buttonComponent = weaponButton.AddComponent<Button>();
            Text buttonText = weaponButton.AddComponent<Text>();
            buttonText.text = weaponName;
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonComponent.onClick.AddListener(() => SelectWeapon(weaponIndex));

            RectTransform buttonRect = weaponButton.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(100f, 30f);
            buttonRect.anchoredPosition = new Vector2(0f, -i * 40f);
        }

        GameObject deselectButton = new GameObject("DeselectButton");
        deselectButton.transform.SetParent(weaponSelectionPanel.transform);
        deselectButton.AddComponent<RectTransform>();
        Button deselectButtonComponent = deselectButton.AddComponent<Button>();
        Text deselectButtonText = deselectButton.AddComponent<Text>();
        deselectButtonText.text = "Deselect";
        deselectButtonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        deselectButtonText.alignment = TextAnchor.MiddleCenter;
        deselectButtonComponent.onClick.AddListener(DeselectWeapon);

        RectTransform deselectButtonRect = deselectButton.GetComponent<RectTransform>();
        deselectButtonRect.sizeDelta = new Vector2(100f, 30f);
        deselectButtonRect.anchoredPosition = new Vector2(0f, -weaponSprites.Length * 40f - 40f);
    }

    // Select a weapon when a weapon button is clicked
    private void SelectWeapon(int weaponIndex)
    {
        SetWeapon(weaponIndex);
        ToggleWeaponSelection();
    }

    // Deselect the weapon when the "Deselect" button is clicked
    private void DeselectWeapon()
    {
        SetWeapon(-1);
    }
}
