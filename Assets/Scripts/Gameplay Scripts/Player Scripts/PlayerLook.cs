using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    [SerializeField] private Image crosshair;
    private float yRotation = 0f;
     public float xSensitivity =30;
     public  float ySensitivity =30;
     [SerializeField] private Slider xslider;
     [SerializeField] private Slider yslider;
   

    private void Awake()
    {
        SetCursor();
        
        
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            xSensitivity = xslider.value/3;
            ySensitivity = yslider.value/3;
        }
        if (!Input.GetMouseButton(1))
        {
            xSensitivity = xslider.value;
            ySensitivity = yslider.value;
        }
    }

    
   

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        yRotation += (mouseY * Time.deltaTime * -ySensitivity);
        yRotation = Mathf.Clamp(yRotation, -80f, 51);
        cam.transform.localRotation =Quaternion.Euler(yRotation,0,0) ;
        transform.Rotate(Vector3.up * (mouseX*Time.deltaTime * xSensitivity));


    }
    void SetCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        crosshair.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f);

    }
}
