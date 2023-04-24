using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPlayer : MonoBehaviour
{
    private DoorWaypoint[] doors;

    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnPlayerinGame()
    {
        doors = FindObjectsOfType<DoorWaypoint>();
        Debug.Log("porte  "+doors.Length); 
        var door  = doors[UnityEngine.Random.Range(0, doors.Length)];
       var newPlayer =Instantiate(player, door.transform.position, transform.rotation);
       FindObjectOfType<ZombieManager>().player = newPlayer;
        player.gameObject.SetActive(false);
    }
}
