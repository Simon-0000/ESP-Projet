using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    [SerializeField] private Image crosshair;
    private float xRotation = 0f;
    [SerializeField] private float xSensitivity = 30f;
    [SerializeField] private float ySensitivity = 30f;

    private void Awake()
    {
        SetCursor();
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        xRotation += (mouseY * Time.deltaTime * -ySensitivity);
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cam.transform.localRotation =Quaternion.Euler(xRotation,0,0) ;
        transform.Rotate(Vector3.up * (mouseX*Time.deltaTime * xSensitivity));


    }
    void SetCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        crosshair.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f);

    }
}
