using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{

    public GameObject attackHitboxPrefab;  // Attach a prefab for the hitbox in Unity Editor
    public float attackDistance = 1.0f;   // The distance in front of the player where the hitbox will appear

    public CharacterController controller;

    private Vector3 plaryVelocity;

    private bool isGrounded;
    public float gravity = -9.81f;
    public float speed = 5f;

    public float jumpHeight = 3f;
    // Start is called before the first frame update
    void Start()
    {

        controller = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        
    }

    // recieve the inputs from the input manager

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        plaryVelocity  += Vector3.up * gravity * Time.deltaTime;
        if(isGrounded && plaryVelocity.y < 0)
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

}