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
[RequireComponent(typeof(BehaviourTreeRunner))]
public class ZombieBehaviour : MonoBehaviour
{
    [SerializeField] public bool isActive = true;
    public bool isChasingTarget = false;
    private int health;
    private int damage;
    public int speed;
    public List<Vector3> patrolLocations;
    public EntryWaypoint entryLocation;
    private float inactiveTime;
    private float fieldOfView = 45;
    private GameObject target;
    private Vector3 entryDestination;
    public Animator animator;
    public NavMeshAgent agent;

    int patrolIndexCounter = 0;
    const int BaseHealth = 100;
    const int BaseDamage = 10;
    const int BaseSpeed = 3;
    public const float AttackRange = 1.5f;
    public const float SightRange = 6f;


    void Start()
    {
        isActive = true;
        health = BaseHealth;
        damage = BaseDamage;
        speed = BaseSpeed;
        EntryWaypoint[] entryLocations = FindObjectsOfType<EntryWaypoint>();
        entryLocation = entryLocations[UnityEngine.Random.Range(0, entryLocations.Length)];
        entryDestination = entryLocation.entryWaypoint.position;
        DefinePatrolSequence();
        DefineTarget();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.destination = entryDestination;
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
      
        //trouver tout les GameObjects avec un DoorWaypoint component
        DoorWaypoint[] doors = FindObjectsOfType<DoorWaypoint>();

        //choisir un point d'entré random pour que le zombie entre dans la carte de jeux
        System.Random indexGenerator = new System.Random();

        if(doors.Length > 0)
        {
            do
            {
                int randomIndex = indexGenerator.Next(doors.Length);
                if (!patrolLocations.Contains(doors[randomIndex].waypointLocation))
                    patrolLocations.Add(doors[randomIndex].waypointLocation);

            } while (patrolLocations.Count < doors.Length);
        }
    }

    public void ManagePatrol()
    {
        if ((transform.position - agent.destination).magnitude < 2)
        {
            agent.destination = patrolLocations[patrolIndexCounter++];
            if (patrolIndexCounter == patrolLocations.Count)
                patrolIndexCounter = 0;
        }
    }

    public void ManageChase()
    {
        agent.destination = target.transform.position;
    }


    public bool HasReachedPath()
    {
        return !agent.pathPending && agent.remainingDistance <= 0.01f;
    }

    
    public bool CanChangeState(float actionRange)
    {
        RaycastHit hit;
        bool isWithinRange = false;
        bool canSeeTarget = false;

        Vector3 direction = transform.position - target.transform.position;
        Vector3 offset = new(0, 1, 0);
        if(Physics.Raycast(transform.position, -direction.normalized, out hit, actionRange,7))
        {
            Debug.Log(hit.collider.gameObject);
            Debug.Log(direction.normalized);
            Debug.Log(transform.position);

            if (hit.collider.gameObject.Equals(target) || Algos.FindFirstParentInstance(hit.collider.gameObject, p => p == target.transform) == target.transform)
                canSeeTarget = true;
            if (Vector3.Angle(direction, Algos.GetVectorAbs(transform.forward)) <= fieldOfView && direction.magnitude <= actionRange)
                isWithinRange = true;
        }

        if (isWithinRange && canSeeTarget)
        {
            FindObjectOfType<ZombieManager>().AddAttackingZombie(gameObject);
            return true;
        }

        return false;
    }

    public void Attack()
    {
        target.GetComponent<PlayerHealth>().takeDamage(damage);
        Debug.Log("attack made"); 
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
        agent.isStopped=true;
        FindObjectOfType<ZombieManager>().RemoveAttackingZombie(gameObject);
        gameObject.layer = GameConstants.INVISIBLE_LAYER;
        for (int i = 0; i < GetComponentsInChildren<BoxCollider>().Length; i++)
            GetComponentsInChildren<BoxCollider>()[i].enabled = false;
    }
}
