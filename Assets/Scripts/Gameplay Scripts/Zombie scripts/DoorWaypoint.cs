using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sergio abreo alvarez
public class DoorWaypoint : MonoBehaviour
{
    public Vector3 waypointLocation;
    void Start()
    {
        waypointLocation = transform.position;
    }
}
