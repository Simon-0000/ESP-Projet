using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class DroneTurret : MonoBehaviour
{
    [SerializeField] private Transform pivotPoint, exitPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float fireDelay;
    private float timeSinceAttack;

    // Update is called once per frame
    void Update()
    {
    }
    
    public bool TryToAttack(Transform target)
    {

        RaycastHit hit;
        Physics.Raycast(pivotPoint.position, (target.position - pivotPoint.position).normalized, out hit,
            (target.position - pivotPoint.position).magnitude);
        if (hit.collider != null && hit.collider.gameObject.transform != target)
        {
            Debug.Log("Nope, object in the way");
            return false;
        }
        //Vector3 rotation = pivotPoint.rotation.eulerAngles;    
        Vector3 newDirection = Vector3.RotateTowards(pivotPoint.forward, target.position - pivotPoint.position, 4*Time.deltaTime, 0.0f);
        pivotPoint.rotation = Quaternion.LookRotation(newDirection);
        //pivotPoint.rotation = Quaternion.Euler(rotation);
        // Si oui, tourner selon pivotPoint, spawn bullet
        // Garder en mémoire le temps de la dernière attaque
        
        // Si non, return false
        
        if (Time.time - timeSinceAttack > fireDelay)
        {
            Instantiate(bullet, exitPoint);
            timeSinceAttack = Time.time;
        }

        return true;
    }
}
