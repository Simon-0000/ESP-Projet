using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int vie;
    void Awake()
    {
        vie = 20;

    }

    private void Update()
    {
        if(vie<=0)
            Destroy(gameObject);
    }

    
     public void takeDamage(int damage) => vie -= damage;
}
