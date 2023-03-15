using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TheKiwiCoder;

// sergio abreo alvarez
//gestion d'un zombie. gestion de la vie, �tat actif, temps de destruction, point d'entr�e 
[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBehaviour : MonoBehaviour
{
   // [SerializeField] public Zombie zombie;
    [SerializeField] public bool isActive;
    [SerializeField] public bool isOnTeam;
    [SerializeField] public bool isLeader;
    [SerializeField] private int health;
    [SerializeField] private int damage;
    [SerializeField] public int speed;
    [SerializeField] public List<ZombieBehaviour> Team;
    [SerializeField] public List<Vector3> patrolLocations;
    [SerializeField] private float inactiveTime;
    const int BaseHealth = 20;
    const int BaseDamage = 5;
    const int BaseSpeed = 10;
    private LazerComponent lazer;

    void Start()
    {
        Team = new List<ZombieBehaviour>(3);
        health = BaseHealth;
        damage = BaseDamage;
        speed = BaseSpeed;
        isActive = true;
        isOnTeam = false;
        isLeader = false;
        DefinePatrolSequence();
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

    

    private void DefinePatrolSequence()
    {
        //trouver tout les GameObjects avec un EntryWaypoint ou DoorWaypoint
        EntryWaypoint[] windows = FindObjectsOfType<EntryWaypoint>();
        DoorWaypoint[] doors = FindObjectsOfType<DoorWaypoint>();

        //choisir un point d'entré random pour que le zombie entre dans la carte de jeux
        System.Random indexGenerator = new System.Random();

        //ajouter la postion de l'entrée pour dans la liste de waypoints
        patrolLocations.Add(windows[indexGenerator.Next(windows.Length)].entryWaypoint);

        //ajouter les points de patroulle pour le zombie en ordre aleatoire,
        //on veut remplir la liste avec toutes les positions de toutes portes de la carte et une fenêtre de la carte
        do
        {
            int randomIndex = indexGenerator.Next(doors.Length);
            if (!patrolLocations.Contains(doors[randomIndex].waypointLocation))
                patrolLocations.Add(doors[randomIndex].waypointLocation);
        } while (patrolLocations.Count <(doors.Length + 1));

    }

    //besoin d'implementer le playerBehaviour
    public void Attack()
    {
        //GetComponent<>
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);
        if (health <= 0)
            ManageDeath();
        
    }

    public void ManageDeath()
    {
        isActive = false;
        Destroy(GetComponent<BehaviourTreeRunner>());
        GetComponent<NavMeshAgent>().isStopped=true;
        if (isLeader)
            ManageLeaderDeath();
    }

    public void ManageLeaderDeath()
    {
        Team.Remove(this);
        if (Team.Count != 0)
            Team[0].isLeader = true;
    }
}
