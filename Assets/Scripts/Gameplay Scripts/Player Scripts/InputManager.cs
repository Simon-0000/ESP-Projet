// fait par Olivier Castonguay
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerinput;
    private PlayerInput.OnFootActions onfoot;

    private PlayerMotor motor;

    private PlayerLook look;
    // Start is called before the first frame update
    void Awake()
    {
        playerinput = new PlayerInput();
        onfoot = playerinput.onFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        onfoot.Jump.performed += ctx => motor.Jump();
    }

    private void FixedUpdate()
    {
        motor.ProcessMove(onfoot.movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        look.ProcessLook(onfoot.Look.ReadValue<Vector2>());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
         onfoot.Enable();
    }
    private void OnDisable()
    {
        onfoot.Disable();
    }
}
