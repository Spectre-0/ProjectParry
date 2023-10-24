using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour
{

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

    public float staminaRegenRate = 5f; // The rate at which stamina regenerates per second

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
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }
        
        // Add this block to handle constant stamina drain while sprinting
        if (isSprinting && currentStamina > 0)
        {
            UseStamina(10 * Time.deltaTime); // Drain 10 stamina units per second
        }
    }

    // recieve the inputs from the input manager

    public void ProcessMove(Vector2 input)
    {
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
            UseStamina(10); 
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
            
            UseStamina(2);  // Use 2 units of staminaa

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
        if (isGrounded && currentStamina > 4)
        {
            UseStamina(5);
            Vector3 dodgeDirection = Vector3.zero;

            // If player is stationary, dodge backward
            if (direction == Vector2.zero)
            {
                dodgeDirection = -transform.forward; 
            }
            else
            {
                // Use the direction of movement for dodging
                dodgeDirection = new Vector3(direction.x, 0, direction.y).normalized;
                dodgeDirection = transform.TransformDirection(dodgeDirection);
            }

            controller.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
        }
    }

    public void Sprint(bool sprintState)
    {
        isSprinting = sprintState;

        if (isSprinting)
        {
            currentSpeed = speed * sprintMultiplier;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Player losing health");
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (currentHealth <= 0f)
        {
            // Handle player death
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
    }



}