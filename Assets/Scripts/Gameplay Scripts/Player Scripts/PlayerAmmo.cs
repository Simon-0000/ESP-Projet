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

    private  int ammo =30;
    private const int maxAmmo = 30;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        
        text.text = ammo.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        text.text = ammo.ToString();
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
                .GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward*500);

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
