using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sergio's script
//base definition of a walker

public class Walker : Zombie
{
    const int BaseHealth = 20;
    const int BaseDamage = 5;

    [SerializeField] private Zombie zombie;

    public override void Attack()
    {
       // add the attack logic...
    }

    public Walker(int health, int damage) : base(health, damage) { }
    // Start is called before the first frame update
    void Start()
    {
        zombie = new Walker(BaseHealth,BaseDamage);  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
