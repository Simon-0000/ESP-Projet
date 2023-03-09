using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sergio abreo alvarez
public class EntryWaypoint : MonoBehaviour
{
    public Vector3 entryWaypoint;
    void Start()
    {
        entryWaypoint = transform.position;
    }
}
