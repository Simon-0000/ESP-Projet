using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitializeGame : MonoBehaviour
{
    private DoorWaypoint[] doors;

    [SerializeField] private GameObject player;
    // Start is called before the first frame update

    public void SpawnPlayerInDoor()
    {
        doors = FindObjectsOfType<DoorWaypoint>();
        Debug.Log("porte  "+doors.Length); 
        var door  = doors[UnityEngine.Random.Range(0, doors.Length)];
        player.transform.position = door.transform.position;
        player.transform.rotation = door.transform.rotation;
    }
    public void UpdateNavMesh()
    {
        NavMeshSurface nm = GameObject.FindObjectOfType<NavMeshSurface>();
        nm.UpdateNavMesh(nm.navMeshData);
    }
}
