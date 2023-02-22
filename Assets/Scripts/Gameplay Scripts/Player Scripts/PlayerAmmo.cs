using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerAmmo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform boucheDeCanon;

    [Header("aiming component")]
    [SerializeField]
    private Transform gun;

    [SerializeField] private Image crosshair;

    private Vector3 unAimedPosition = new Vector3(0.284f, -0.4f, 0.385f);
    private Vector3 aimedposition = new Vector3(0.00085f, -0.16f, 0.296f);
    private Vector3 deplacement;
    private  int ammo;
    private const int maxAmmo = 30;
    
    void Awake()
    {
        setCursor();
        ammo = maxAmmo;
        text.text = ammo.ToString();
        deplacement = aimedposition - gun.transform.position;


    }

    // Update is called once per frame
    void Update()
    {
        aimGun();
        
        if (Input.GetMouseButton(1))
        {
           gun.localPosition=(aimedposition);
           crosshair.color = new Color(0, 0, 0, 0);
        }
        else
        {
            gun.localPosition = unAimedPosition;
            crosshair.color = Color.green;

        }
        if (Input.GetMouseButtonDown(0))
            Shoot();
        if (Input.GetKeyDown("r"))
            Reload();

    }

   
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void Shoot()
    {
        if (ammo > 0)
        {
            Instantiate(bullet, boucheDeCanon.position, boucheDeCanon.rotation);
            ammo -= 1;
        }
        text.text = ammo.ToString();
           
    }

    void Reload()
    {

        var currentAmmo = ammo;
        var missingAmmo = maxAmmo - currentAmmo;
        ammo += missingAmmo;   
        text.text = ammo.ToString();

    }

    void setCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void aimGun()
    {
        RaycastHit hit = default;
        Ray ray  = Camera.main.ScreenPointToRay (new Vector3(Screen.width/2f,Screen.height/2f));
        if (Physics.Raycast (ray,out hit))
            gun.LookAt(hit.point);
    }
    
}
