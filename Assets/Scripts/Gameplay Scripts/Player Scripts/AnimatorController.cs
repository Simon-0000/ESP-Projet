// fait par Olivier Castonguay
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;
    
    private bool isgrounded;
    
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsRight = Animator.StringToHash("isRight");
    private static readonly int IsLeft = Animator.StringToHash("isLeft");
    private static readonly int IsBackward = Animator.StringToHash("isBackward");


    void Start()
    {
        animator = GetComponent<Animator>();
       
    }

   

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey("w"))
        {
            animator.SetBool(IsWalking,true);
        }
        else
        {
            animator.SetBool(IsWalking,false);
        }
        if (Input.GetKey("space"))
        {
            animator.SetBool(IsJumping,true);
        }
        else
        {
            animator.SetBool(IsJumping,false);
        }
        if (Input.GetKey("a"))
        {
            animator.SetBool(IsLeft,true);
        }
        else
        {
            animator.SetBool(IsLeft,false);
        }
        if (Input.GetKey("d"))
        {
            animator.SetBool(IsRight,true);
        }
        else
        {
            animator.SetBool(IsRight,false);
        }
        if (Input.GetKey("s"))
        {
            animator.SetBool(IsBackward,true);
        }
        else
        {
            animator.SetBool(IsBackward,false);
        }


       
        
        
        
       
        
    }

}
