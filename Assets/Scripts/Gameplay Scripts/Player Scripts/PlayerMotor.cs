// fait par Olivier Castonguay
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 velocity;
    private bool isgrounded;
    private float gravity = -9.8f;

     public float speed = 5f;
     public float jumpHeigth = 1f;
    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        isgrounded = controller.isGrounded;
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection * (speed * Time.deltaTime)));
         velocity.y += gravity * Time.deltaTime;
         if (isgrounded && velocity.y < 0)
            velocity.y = -2f;
         controller.Move(velocity * Time.deltaTime);
         


    }

    public void Jump()
    {
        if(isgrounded)
            velocity.y = Mathf.Sqrt(-1*gravity);
          
        
    }

}
