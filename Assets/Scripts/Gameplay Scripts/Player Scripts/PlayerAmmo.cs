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

    private Vector3 unAimedPosition = new Vector3(0.284f, -0.4f, 0.385f);
    private Vector3 aimedposition = new Vector3(0.00085f, -0.16f, 0.296f);
    private Vector3 deplacement;
    private  int ammo =30;
    private const int maxAmmo = 30;
    
    // Start is called before the first frame update
    void Awake()
    {
        Cursor.visible = false;
        text.text = ammo.ToString();
        deplacement = aimedposition - gun.transform.position;


    }

    // Update is called once per frame
    void Update()
    {
        text.text = ammo.ToString();
        if (Input.GetMouseButton(1))
        {
           gun.localPosition=(aimedposition);
        }
        else
        {
            gun.localPosition = unAimedPosition;

        }
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown("r"))
            Reload();

    }

   
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void Shoot()
    {
        if (ammo > 0)
        {
            Instantiate( bullet, boucheDeCanon.position, boucheDeCanon.rotation)
                .GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward*1000);

            ammo -= 1;
        }
           
    }

    void Reload()
    {

        var currentAmmo = ammo;
        var missingAmmo = maxAmmo - currentAmmo;
        ammo += missingAmmo;    

    }
    
}
