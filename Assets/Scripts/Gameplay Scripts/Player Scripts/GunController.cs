// fait par Olivier Castonguay
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class GunController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform boucheDeCanon;

    [Header("aiming component")] [SerializeField]
    private Transform gun;

    private Vector3 unAimedPosition = new Vector3(0.284f, -0.4f, -0.07f);
    private Vector3 aimedposition = new Vector3(0.00085f, -0.16f, 0.296f);
    private Vector3 deplacement;
    private float zoomMultiplier = 2;
    private float defaultFov = 90;
    private float zoomDuration = 1 / 8f;
    private float timeelapse=0;

    [SerializeField] private Image crosshair;
    [SerializeField] private RawImage aimedcrosshair;


    private int ammo;
    private Camera cam;
    private const int maxAmmo = 30;

    void Awake()
    {

        cam = Camera.main;
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
            timeelapse = 0;
                
            
            ZoomCamera(defaultFov / zoomMultiplier); 
            gun.localPosition = Vector3.MoveTowards(gun.localPosition ,aimedposition, deplacement.magnitude/100);
            gun.rotation = new Quaternion(0f,0f,0f,0f);
           // crosshair.color = new Color(0, 0, 0, 0);
           crosshair.enabled = false;
           aimedcrosshair.enabled = true;
           gun.GetComponent<MeshRenderer>().enabled=false;
        } 


        else
        {
            if (timeelapse > 0.1f)
            {
                gun.GetComponent<MeshRenderer>().enabled = true;
               
            }
        else
            {
                timeelapse += Time.deltaTime; 
                 
            }

            gun.localPosition =
                Vector3.MoveTowards(gun.localPosition, unAimedPosition, deplacement.magnitude / 100);
            ZoomCamera(defaultFov);
            crosshair.enabled = true;
            aimedcrosshair.enabled = false;
           
            AimGun();
            crosshair.color = Color.green;
            
        }

        if (Input.GetMouseButtonDown(0))
            Shoot();
        if (Input.GetKeyDown("r"))
            Reload();

    }

    void ZoomCamera(float target)
    {
        float angle = Mathf.Abs((defaultFov / zoomMultiplier) - defaultFov);
        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, target, angle / zoomDuration * Time.deltaTime);
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
        aimedcrosshair.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f);
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