//Auteurs: Michaël Bruneau
//Explication: Cette classe a pour but d'attaquer un zombie avec le drone. Le drone va attaquer le zombie qui est
//le plus près du joueur en mode poursuite et qui est visible pour le drone. 

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
    [SerializeField] private VectorRange aimOffset;
    private float timeSinceAttack;
    Hashtable rotatingHashArgs = new();
    // Update is called once per frame
    
    public bool TryToAttack(Transform target)
    {
        Vector3 targetPosition = target.position + aimOffset.GetRandomVector();
        RaycastHit hit;
        Physics.Raycast(pivotPoint.position, (targetPosition - pivotPoint.position).normalized, out hit,
            (targetPosition - pivotPoint.position).magnitude);
        if (hit.collider != null && hit.collider.gameObject.transform != target && Algos.FindFirstParentInstance(hit.collider.gameObject, t=> t == target) != target)
        {
            return false;
        }
        
        var rotation = Quaternion.LookRotation(targetPosition - transform.position);
        pivotPoint.rotation = rotation;
        // Si non, return false
        if (Time.time - timeSinceAttack > fireDelay)
        {
            ShootInFront();
        }
        return true;
    }
    void ShootInFront()
    {
        
        GameObject obj = Instantiate(bullet, exitPoint.transform.position, exitPoint.transform.rotation);

        timeSinceAttack = Time.time;
    }
}
