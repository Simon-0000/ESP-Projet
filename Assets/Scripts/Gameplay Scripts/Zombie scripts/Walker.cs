using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sergio's script
//base definition of a walker

public class Walker : Zombie
{
    const int BaseHealth = 20;
    const int BaseDamage = 5;
    const int BaseSpeed = 10;

    [SerializeField] private Zombie zombie;

    public override void Attack()
    {
       // add the attack logic...
    }

    public Walker CreateNewWalker()
    {
        return new Walker(BaseHealth, BaseDamage, BaseSpeed);
    }

    public Walker(int health, int damage, int speed) : base(health, damage, speed) { }
    // Start is called before the first frame update
    void Start()
    {
        zombie = CreateNewWalker();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
