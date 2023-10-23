using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{

    public float maxHealth = 100f;
    public float currentHealth;

    public float maxStamina = 100f;
    public float currentStamina;

    public float staminaRegenRate = 5f; // The rate at which stamina regenerates per second

    public float speed = 5f;
    public float sprintMultiplier = 2f; // How much faster the player will sprint

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
        }
    }

    public void Attack()
    {
        // Create a hitbox in front of the player
        Vector3 attackPosition = transform.position + transform.forward * attackDistance;
        GameObject hitbox = Instantiate(attackHitboxPrefab, attackPosition, transform.rotation);

        // Destroy the hitbox after a short time (e.g., 0.5 seconds)
        Destroy(hitbox, 0.5f);

        // Add code to detect collisions with enemies, apply damage, etc.
    }


    public void Dodge(Vector2 direction)
    {
        if (isGrounded)
        {
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

    public void Sprint(bool isSprinting)
    {
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
        }
    }

    // Create a method to use stamina
    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }



}