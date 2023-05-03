using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//sergio abreo alvarez
//zombie manager est en charge de la gestion des zombies. Il garde en memoire tout les zombies qui sont actif.
//quand un zombie n'est pas actif il est "removed" de la liste

public class ZombieManager : MonoBehaviour
{
    [Range(0,99)]
    [SerializeField] private int nbWantedZombies = 50;
    private int nbActiveZombies;
    [SerializeField] private List<GameObject> ActiveZombies;
    public List<GameObject> AttackingZombies;
    [SerializeField] private GameObject zombie;
    [SerializeField] private SpawnLocation[] spawnPoints;
    [SerializeField] public GameObject player;
    private float ellapsedTime = 0;
    private int nextInstanciatePriority = 1;

    private GameObject CreateNewZombie()
    {
        return Instantiate(zombie, spawnPoints[Random.Range(0,spawnPoints.Length)].spawnLocation,new Quaternion(0,0,0,0));
    }

    void Awake()
    {
        ActiveZombies = new List<GameObject>(nbWantedZombies);
        spawnPoints = FindObjectsOfType<SpawnLocation>();
    }

    void Update()
    {
        for(int i = 0; i<ActiveZombies.Count; ++i)
        {
            if (!ActiveZombies[i].GetComponent<ZombieBehaviour>().isActive) 
            {
                ActiveZombies.Remove(ActiveZombies[i]);
            }
               
        }

        for(int i = 0; i < ActiveZombies.Count; ++i)
        {
            if (ActiveZombies[i].GetComponent<ZombieBehaviour>().isChasingTarget && !AttackingZombies.Contains(ActiveZombies[i]))
                AddAttackingZombie(ActiveZombies[i]);
        }

        ellapsedTime += Time.deltaTime;
        if (ActiveZombies.Count < nbWantedZombies && ellapsedTime >= 4f)
        {
            nbActiveZombies++;
            ActiveZombies.Add(CreateNewZombie());
            ActiveZombies.Last().GetComponent<NavMeshAgent>().avoidancePriority = nextInstanciatePriority;
            nextInstanciatePriority++;
            if (nextInstanciatePriority == 100)
                nextInstanciatePriority = 1;
            ellapsedTime = 0;
        }
    }

    public void RemoveAttackingZombie(GameObject zombieToRemove)
    {
        AttackingZombies.Remove(zombieToRemove);
    }
    public void AddAttackingZombie(GameObject zombieToAdd)
    {
        
        AttackingZombies.Add(zombieToAdd);
        zombieToAdd.GetComponent<NavMeshAgent>().avoidancePriority = 0;
    }
}
