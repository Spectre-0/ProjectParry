using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;  

public class PlayerMotor : MonoBehaviour
{

    
    // Gameplay UI References
    public GameObject pauseMenuUI; // Attach your pause menu UI in the inspector
    public GameObject Game; // Attach your Game object in the inspector
    public GameObject youDiedText; // Attach your "You Died" UI Text element here in the inspector

    // Player Status Flags
    private bool isGamePaused = false;
    private bool isParrying = false;
    private bool isDodging = false;
    private bool hasAttackedBefore = false;
    private bool isSprinting = false;
    private bool isGrounded;

    // Player Stats
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxStamina = 100f;
    public float currentStamina;

    // Player Movement & Actions
    public float speed = 5f;
    public float sprintMultiplier = 2f; // How much faster the player will sprint
    public float dodgeSpeed = 500f;  // How fast the player will dodge
    private float currentSpeed;
    public float jumpHeight = 3f;

    // Stamina Parameters
    public float staminaRegenRate = 100f; // Stamina regenerates per second
    public float staminaRegenDelay = 2.0f;  // Delay before stamina starts regenerating
    private float lastStaminaUseTime = 0f; // Time when the player last used stamina

    // Stamina Costs
    public float SprintStaminaCost = 20f;
    public float DodgeStaminaCost = 10f;
    public float ParryStaminaCost = 14f;
    public float AttackStaminaCost = 14f;
    public float JumpStaminaCost = 10f;

    // Cooldowns & Timers
    private float parryCooldown = 2.0f; // Cooldown for the parry
    private float lastParryTime = 0f;
    public float attackCooldown = 5f;  // Cooldown time for the attack
    private float lastAttackTime = 0f;

    // Attack Parameters
    public GameObject attackHitboxPrefab;  // Attach a prefab for the hitbox in Unity Editor
    public float attackDistance = 1.0f;   // Distance in front of the player for the hitbox

    // Dodge Parameters
    public float maxDodgeDistance = 5f;  // Max dodge distance

    // Physics & Movement
    public CharacterController controller; // Attach CharacterController in the inspector
    private Vector3 playerVelocity;
    public float gravity = -9.81f;

    // Animator References
    public Animator swordAnimator; // Drag your sword's Animator here in the inspector




    public void TogglePause()
    {
        isGamePaused = !isGamePaused; // Toggle the state of isGamePaused

        if (isGamePaused)
        {
            ActivatePauseMenu();
        }
        else
        {
            DeactivatePauseMenu();
        }
    }

    private void ActivatePauseMenu()
    {
        
        
        pauseMenuUI.SetActive(true);
        pauseMenuUI.GetComponent<PlayerMenu>().ShowPlayerMenu();
        //make sure that the pause menu is 
        Time.timeScale = 0f; // Pause the game

        // Unlock the cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DeactivatePauseMenu()
    {
       
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    

    [SerializeField]
    private int playerMoney = 0;

    [SerializeField]
    private TextMeshProUGUI moneyText;

    
    [SerializeField]


    private int NumberOfHeals = 5;

    [SerializeField]
    private TextMeshProUGUI NumberOfHealsText;

    public void Heal()
    {
        if (NumberOfHeals > 0 && currentHealth < maxHealth)
        {
            Debug.Log("Player is healing");
            currentHealth += 20;
            //currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); 
            NumberOfHeals -= 1;
            UpdateNumberOfHealsDisplay();
        }
    }

    private void UpdateNumberOfHealsDisplay()
    {
        NumberOfHealsText.text = NumberOfHeals.ToString();
    }


    private void UpdateMoneyDisplay()
    {
        moneyText.text = playerMoney.ToString();
    }

    // Start is called before the first frame update
    void Start()
    { 
        UpdateNumberOfHealsDisplay();

        UpdateMoneyDisplay();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        
        controller = GetComponent<CharacterController>();
        currentSpeed = speed; // Initialize currentSpeedb        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;

        if (Time.time - lastStaminaUseTime >= staminaRegenDelay)  // Check if enough time has passed
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }
        
        if (currentStamina <= 0)
        {
            isSprinting = false; // Turn off sprinting if stamina is zero
        }

        if (isSprinting && currentStamina > 0)
        {
            UseStamina(SprintStaminaCost * Time.deltaTime); // Drain 10 stamina units per second
        }

        if (isSprinting && currentStamina <= 0)
        {
            isSprinting = false;  // Add this line to stop sprinting if stamina reaches zero
            currentSpeed = speed; // Reset speed back to normal
        }
    }

    // recieve the inputs from the input manager

    public void ProcessMove(Vector2 input)
    {
        if (isDodging) return;
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);
        playerVelocity += Vector3.up * gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
        // Debug.Log(playerVelocity.y);
    }

    public void Jump()
    {
        if(isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            UseStamina(JumpStaminaCost); 
        }
    }

    public void Attack()
    {
        float currentTime = Time.time;
        if (currentStamina > 1 && (!hasAttackedBefore || currentTime - lastAttackTime >= attackCooldown))  // Check if there is enough stamina and if the cooldown has passed
        {
            Debug.Log("Player attacking");
            lastAttackTime = currentTime;  // Update the last attack time
            hasAttackedBefore = true;  // Update the flag since the player has now attacked
            
            UseStamina(AttackStaminaCost);  // Use stamina

            // Trigger the sword swinging animation
            swordAnimator.SetTrigger("Swing");

            // Create a hitbox in front of the player
            Vector3 attackPosition = transform.position + transform.forward * attackDistance;
            GameObject hitbox = Instantiate(attackHitboxPrefab, attackPosition, transform.rotation);
            
            // Destroy the hitbox after a short time (e.g., 0.5 seconds)
            Destroy(hitbox, 0.5f);
        }
    }


    public void Dodge(Vector2 direction)
    {
        if (isGrounded && currentStamina > 4 && !isDodging)
        {
            UseStamina(DodgeStaminaCost);
            Vector3 dodgeDirection = Vector3.zero;
            isDodging = true;

            if (direction == Vector2.zero)
            {
                dodgeDirection = -transform.forward;
            }
            else
            {
                dodgeDirection = new Vector3(direction.x, 0, direction.y).normalized;
                dodgeDirection = transform.TransformDirection(dodgeDirection);
            }

            StartCoroutine(SmoothDodge(dodgeDirection, 0.1f));
        }
    }

    IEnumerator SmoothDodge(Vector3 direction, float duration)
    {
        float elapsed = 0f;
        float totalDodgeDistance = dodgeSpeed * duration;
        totalDodgeDistance = Mathf.Min(totalDodgeDistance, maxDodgeDistance);  // Limit the dodge distance

        while (elapsed < duration)
        {
            float fractionOfTime = Time.deltaTime / duration;
            float distanceThisFrame = fractionOfTime * totalDodgeDistance;

            controller.Move(direction * distanceThisFrame);
            elapsed += Time.deltaTime;
            
            yield return null;
        }

        isDodging = false;
    }
    public void Sprint(bool sprintState)
    {
        if (sprintState && currentStamina > 0 && isGrounded == true)  // Only set sprinting to true if there's enough stamina
        {
            isSprinting = true;
            currentSpeed = speed * sprintMultiplier;
        }
        else
        {
            isSprinting = false;
            currentSpeed = speed;
        }
    }

    public void Parry()
    {
        float currentTime = Time.time;
        if (currentTime - lastParryTime >= parryCooldown)
        {
            swordAnimator.SetTrigger("Swing");
            Debug.Log("Player is parrying");
            isParrying = true;
            lastParryTime = currentTime;
            StartCoroutine(StopParryAfterDelay(1)); // 1 second of parry window
        }
    }

    IEnumerator StopParryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Parry window closed");
        isParrying = false;
    }

    // Modify TakeDamage method
    public void TakeDamage(float amount)
    {
        if (isParrying)
        {
            Debug.Log("Attack parried, no damage taken");
            return;
        }
        
        Debug.Log("Player losing health, attack blocked");
        currentHealth -= amount * 0.2f; // 80% damage reduction if missed parry but blocked
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp health to 0 and maxHealth
        RedTintEffect.Instance.PlayerHit();
        if (currentHealth <= 0f)
        {
            Debug.Log("Player is dead");
            HandlePlayerDeath();
        }
    }



    void HandlePlayerDeath()
    {
        youDiedText.SetActive(true); // Show "You Died" text
        Time.timeScale = 0f; // Freeze the game
        StartCoroutine(ReloadSceneAfterDelay(3)); // Reload the scene after 3 seconds
    }

    IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Realtime delay, as timeScale is 0
        Time.timeScale = 1f; // Reset time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    // Create a method to use stamina
    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        lastStaminaUseTime = Time.time;  // Record the last time stamina was used
    }
    

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyDisplay();
        Debug.Log("Player has " + playerMoney + " money");
    }



}