using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sergio abreo alvarez
//zombie manager est en charge de la gestion des zombies. Il garde en memoire tout les zombies qui sont actif.
//quand un zombie n'est pas actif il est "removed" de la liste

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private int nbWantedZombies = 50;
    private int nbActiveZombies;
    private List<GameObject> ActiveZombies;
    [SerializeField] private GameObject zombie;
    [SerializeField] private List<Vector3> entryPoints;
    [SerializeField] private Vector3 spawnPoint;
    private float ellapsedTime = 0;
    private GameObject CreateNewZombie()
    {
        return Instantiate(zombie, spawnPoint,new Quaternion(0,0,0,0));
    }

    private Vector3 ChoseWindow()
    {
        return entryPoints[Random.Range(0,entryPoints.Count)];
    }

    void Awake()
    {
        ActiveZombies = new List<GameObject>(nbWantedZombies);
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i<ActiveZombies.Count; ++i)
        {
            if (!ActiveZombies[i].GetComponent<ZombieBehaviour>().isActive)
                ActiveZombies.Remove(ActiveZombies[i]);
        }

        ellapsedTime += Time.deltaTime;
        if (ActiveZombies.Count < nbWantedZombies && ellapsedTime >= 4f)
        {
            ActiveZombies.Add(CreateNewZombie());
            ellapsedTime = 0;
        }
    }
}
