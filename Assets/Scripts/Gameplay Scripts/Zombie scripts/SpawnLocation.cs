using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sergio abreo alvarez
public class SpawnLocation : MonoBehaviour
{
    public Vector3 spawnLocation;

    void Start()
    {
        spawnLocation = transform.position;
    }

}
