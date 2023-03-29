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
     public float xSensitivity ;
     public float ySensitivity ;

    private void Awake()
    {
        SetCursor();
        
    }
    public void XSetSensitivity (float x)
    {
        xSensitivity = x;
       

    }
    public void YSetSensitivity (float y)
    {
        ySensitivity = y;
       

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
