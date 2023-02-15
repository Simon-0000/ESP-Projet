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

    private List<int> ammo =new List<int>(new int[30]);
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        
        text.text = ammo.Count.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        text.text = ammo.Count.ToString();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown("r"))
            Reload();

    }

    private void Shoot()
    {
        if (ammo.Count > 0)
        {
             //Instantiate(direction,rotation,bullet)
            ammo.RemoveAt(ammo.Count-1);
        }
           
    }

    void Reload()
    {
        
        for (int i = 0; ammo.Count < 30; i++)
        {
            ammo.Add(0);
            
        }
    }
    
}
