using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
 

public class PlayerHealth : MonoBehaviour
{
    public int vie;
    public Healthbar healthbar;
    private const int viemax = 100;
    private float timeSpendBeetweenregen = 0;

    void Awake()
    {
       // healthbar.SetMaxHealth(viemax);
        vie = viemax;
        
        

    }

    private void Update()
    {
       timeSpendBeetweenregen+= Time.deltaTime;
       if (vie < viemax)
           if(timeSpendBeetweenregen > 5f)
           {
               
               RegenHealth();
           }
       if (vie <= 0)
            Destroy(gameObject);
    }



    void RegenHealth()
    {
        if (vie + 5 > viemax)
            vie = viemax;
        vie += 5;
        vie = Mathf.Clamp(vie, 0, 100);
        healthbar.updateHealthbar(vie);
        timeSpendBeetweenregen = 0;
        
            
    }

    
     public void takeDamage(int damage)
     {  vie -= damage;
         timeSpendBeetweenregen = 0;
         healthbar.updateHealthbar(vie);
     }
}
