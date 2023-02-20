using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sergio's script
//zombie manager, it keeps track of all active zombies and severs the purpose of instantiating new ones when needed.
//it will assign their spwan point, entry point and choose what kind of zombie to spwan based on their "rarity"

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private int nbWantedZombies = 50;
    private int nbActiveZombies;
    private List<Zombie> ActiveZombies;
    [SerializeField] private Walker Walker;
    [SerializeField] private List<Vector3> entryPoints;
    [SerializeField] private Vector3 spawnPoint;
    private Zombie CreateNewZombie()
    {
        return Instantiate(Walker, spawnPoint,new Quaternion(0,0,0,0));
    }

    private Vector3 ChoseWindow()
    {
        return entryPoints[Random.Range(0,entryPoints.Count)];
    }

    void Awake()
    {
        ActiveZombies = new List<Zombie>(nbWantedZombies);
    }

    // Update is called once per frame
    void Update()
    {
        if (ActiveZombies.Count == nbWantedZombies)
            return;

        do
        {
            ActiveZombies.Add(CreateNewZombie());
        } while (ActiveZombies.Count < nbWantedZombies);
    }
}
