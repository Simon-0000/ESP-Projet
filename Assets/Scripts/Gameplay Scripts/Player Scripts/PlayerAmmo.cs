// fait par Olivier Castonguay
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

    [Header("aiming component")] [SerializeField]
    private Transform gun;

    [SerializeField] private Image crosshair;

    private Vector3 unAimedPosition = new Vector3(0.284f, -0.4f, 0.385f);
    private Vector3 aimedposition = new Vector3(0.00085f, -0.16f, 0.296f);
    private Vector3 deplacement;
    private int ammo;
    private const int maxAmmo = 30;

    void Awake()
    {
        SetCursor();
        SetAmmo();
        deplacement = aimedposition - gun.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        if (Input.GetMouseButton(1))
        {
            gun.localPosition = (aimedposition);
            gun.rotation = new Quaternion(0f,0f,0f,0f);
            crosshair.color = new Color(0, 0, 0, 0);

        }
        else
        {
            AimGun();
            gun.localPosition = unAimedPosition;
            crosshair.color = Color.green;
        }

        if (Input.GetMouseButtonDown(0))
            Shoot();
        if (Input.GetKeyDown("r"))
            Reload();

    }



    // ReSharper disable Unity.PerformanceAnalysis
     void Shoot()
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

    void SetCursor()
    {
        crosshair.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void AimGun()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out hit))
        {
            gun.LookAt(hit.point);
           boucheDeCanon.LookAt(hit.point);
        }


        else
            gun.rotation = new Quaternion(0f,0f,0f,0f);

    }

    void SetAmmo()
    { ammo = maxAmmo;
        text.text = ammo.ToString();}

}