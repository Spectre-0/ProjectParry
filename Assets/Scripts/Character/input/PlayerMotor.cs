using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour
{

    private float lastStaminaUseTime = 0f; // The time when the player last used stamina

    public float staminaRegenDelay = 2.0f;  // 2-second delay before stamina starts regenerating


    public float SprintStaminaCost = 20f;

    public float DogeStaminaCost = 10f;

    public float ParryStaminaCost = 14f;

    public float AttackStaminaCost = 14f;

    public float staminaRegenRate = 100f; // The rate at which stamina regenerates per second

    public float JumpStaminaCost = 10f;

    public bool isParrying = false;
    private float parryCooldown = 2.0f; // 2-second cooldown for the parry
    private float lastParryTime = 0f;


    private bool isDodging = false;

    private bool hasAttackedBefore = false;

    public float attackCooldown = 5f;  // Set the cooldown time to 1 second (you can change this value)
    private float lastAttackTime = 0f;  // Stores the time when the last attack occurred

    public Animator swordAnimator; // Drag your sword's Animator here in the inspector

    public GameObject youDiedText; // Drag your "You Died" UI Text element here in the inspector


    private bool isSprinting = false;

    public float maxHealth = 100f;
    public float currentHealth;

    public float maxStamina = 100f;
    public float currentStamina;



    public float speed = 5f;
    public float sprintMultiplier = 4f; // How much faster the player will sprint

    public float dodgeSpeed = 500f;  // How fast the player will dodge
    private float currentSpeed;


    public GameObject attackHitboxPrefab;  // Attach a prefab for the hitbox in Unity Editor
    public float attackDistance = 1.0f;   // The distance in front of the player where the hitbox will appear

    public CharacterController controller;

    private Vector3 plaryVelocity;

    private bool isGrounded;
    public float gravity = -9.81f;


    public float jumpHeight = 3f;
    // Start is called before the first frame update
    void Start()
    { 
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
        plaryVelocity += Vector3.up * gravity * Time.deltaTime;
        if (isGrounded && plaryVelocity.y < 0)
        {
            plaryVelocity.y = -2f;
        }
        controller.Move(plaryVelocity * Time.deltaTime);
        // Debug.Log(plaryVelocity.y);
    }

    public void Jump()
    {
        if(isGrounded)
        {
            plaryVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
            UseStamina(DogeStaminaCost);
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

            StartCoroutine(SmoothDodge(dodgeDirection, 0.3f));
        }
    }

    IEnumerator SmoothDodge(Vector3 direction, float duration)
    {
        float elapsed = 0f;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + direction * dodgeSpeed * Time.deltaTime;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        isDodging = false;
    }

    public void Sprint(bool sprintState)
    {
        if (sprintState && currentStamina > 0)  // Only set sprinting to true if there's enough stamina
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
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
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
    


}