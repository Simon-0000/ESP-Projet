using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sergio
//gestion d'un zombie. gestion de la vie, état actif, temps de destruction, point d'entrée 
public class ZombieBehaviour : MonoBehaviour
{
   // [SerializeField] public Zombie zombie;
    [SerializeField] public bool isActive;
    [SerializeField] public bool isOnTeam;
    [SerializeField] public bool isLeader;
    [SerializeField] private int health;
    [SerializeField] private int damage;
    [SerializeField] private int speed;
    [SerializeField] public List<ZombieBehaviour> Team;
    [SerializeField] private float inactiveTime;
    const int BaseHealth = 20;
    const int BaseDamage = 5;
    const int BaseSpeed = 10;

    void Start()
    {
        Team = new List<ZombieBehaviour>(3);
        health = BaseDamage;
        damage = BaseDamage;
        speed = BaseSpeed;
        isActive = true;
        isOnTeam = false;
        isLeader = false;
    }

    private void Update()
    {
        if (!isActive)
        {
            inactiveTime += Time.deltaTime;
            if (inactiveTime >= 3)
                Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
            ManageDeath();
    }

    public void ManageDeath()
    {
        isActive = false;
        if (isLeader == true)
            ManageLeaderDeath();
    }

    public void ManageLeaderDeath()
    {
        Team.Remove(this);
        if (Team.Count != 0)
            Team[0].isLeader = true;
    }
}
