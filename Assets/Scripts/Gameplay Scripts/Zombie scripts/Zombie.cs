using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sergio's script 
//base defintion of a zombie and of it's methods

public abstract class Zombie : MonoBehaviour
{
    public int Health { get; private set; }
    public int Damage { get; private set; }
    public int Speed { get; private set; }
    public bool IsLeader { get; private set; }
    public bool IsOnTeam { get; private set; }
    public List<Zombie> Team { get; private set; }

    public Zombie(int health, int damage, int speed)
    {
        Health = health;
        Damage = damage;
        Speed = speed;
        IsLeader = false;
        IsOnTeam = false;
        Team = new List<Zombie>(3);
    }

    abstract public void Attack();
    public void ManageLeaderDeath(Zombie leader)
    {
       Team.Remove(leader);
       if (Team.Count != 0)
           Team[0].IsLeader = true;
    }
}