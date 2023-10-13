using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{

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
        Debug.Log(plaryVelocity.y);


    }

    public void Jump()
    {
        if(isGrounded)
        {
            plaryVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}