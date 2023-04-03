using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TheKiwiCoder;
using Assets;


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
    [SerializeField] private float fieldOfView = 90;
    [SerializeField] private GameObject target;
    int patrolIndexCounter = 0;
    const int BaseHealth = 100;
    const int BaseDamage = 10;
    const int BaseSpeed = 3;

    private Animator animator;
 
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
        DefineTarget();
        GetComponent<NavMeshAgent>().speed = speed;
        GetComponent<NavMeshAgent>().destination = patrolLocations[patrolIndexCounter];
        animator = GetComponent<Animator>();
        animator.SetBool("walking", true);
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

    private void DefineTarget()
    {
        ZombieManager manager = FindObjectOfType<ZombieManager>();
        target = manager.player;
    }

    private void DefinePatrolSequence()
    {
      
        //trouver tout les GameObjects avec un EntryWaypoint ou DoorWaypoint
        EntryWaypoint[] windows = FindObjectsOfType<EntryWaypoint>();
        DoorWaypoint[] doors = FindObjectsOfType<DoorWaypoint>();

        //choisir un point d'entré random pour que le zombie entre dans la carte de jeux
        System.Random indexGenerator = new System.Random();

        //ajouter la postion de l'entrée pour dans la liste de waypoints
        if(windows.Length + doors.Length > 0)
        {

            patrolLocations.Add(windows[indexGenerator.Next(windows.Length)].entryWaypoint);

            //ajouter les points de patroulle pour le zombie en ordre aleatoire,
            //on veut remplir la liste avec toutes les positions de toutes portes de la carte et une fenêtre de la carte
            do
            {
                int randomIndex = indexGenerator.Next(doors.Length);
                if (!patrolLocations.Contains(doors[randomIndex].waypointLocation))
                    patrolLocations.Add(doors[randomIndex].waypointLocation);

                //comme je viens d'ajouter le point pour entrer du zombie je ne doit pas en tenir
                //compte pour ajouter toutes les portes dans le patrol du zombie
            } while (patrolLocations.Count-1 < doors.Length);
        }
    }

    public void ManagePatrol()
    {
       // GetComponent<NavMeshAgent>().destination = patrolLocations[patrolIndexCounter];
       // Debug.Log("patrol update was made");
        if ((transform.position - GetComponent<NavMeshAgent>().destination).magnitude < 2)
        {
            GetComponent<NavMeshAgent>().destination = patrolLocations[patrolIndexCounter++];
            if (patrolIndexCounter == patrolLocations.Count)
                patrolIndexCounter = 0;
        }
    }

    public void ManageChase()
    {
        GetComponent<NavMeshAgent>().destination = target.transform.position;
    }


    // pour l'instant cette function n'est pas finie
    public bool CanChangeState(float actionRange)
    {
        RaycastHit[] hits;

        Vector3 direction = Algos.GetVectorAbs(transform.position - target.transform.position);
        hits = Physics.RaycastAll(transform.position, direction, 4);
        bool isWithinRange = false;
        bool canSeeTarget = false;
        if (Vector3.Angle(direction, Algos.GetVectorAbs(transform.forward)) <= fieldOfView && direction.magnitude <= actionRange)
        {
            isWithinRange = true;
            if (hits.Length != 0 && hits[0].Equals(target) )
            {
                canSeeTarget = true;
            }
        }

        //Debug.Log(isWithinRange);
        if (isWithinRange && canSeeTarget)
            return true;

        //return false;
       return Vector3.Angle(direction, Algos.GetVectorAbs(transform.forward)) <= fieldOfView && direction.magnitude <= actionRange;
    }

    public void Attack()
    {
      

        target.GetComponent<PlayerHealth>().takeDamage(damage);
        Debug.Log("attack made");
       
         
        
       // animator.SetBool("attack",false);

        
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            ManageDeath();
        
    }

    public void ManageDeath()
    {
        isActive = false;
       
        animator.SetBool("walking",false);
        animator.SetBool("attack",false);
         animator.SetBool("dead",true);
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
