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
    void Update()
    {
    }
    
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
        //Vector3 rotation = pivotPoint.rotation.eulerAngles;    
        //Vector3 newDirection = Vector3.RotateTowards(pivotPoint.forward, target.position - pivotPoint.position, 4*Time.deltaTime, 0.0f);
        //pivotPoint.rotation = Quaternion.LookRotation(newDirection);
        //pivotPoint.rotation = Quaternion.Euler(rotation);
        var rotation = Quaternion.LookRotation(targetPosition - transform.position);
        pivotPoint.rotation = rotation;
        // Si non, return false
        if (Time.time - timeSinceAttack > fireDelay)
        {
            ShootInFront();
           /* rotatingHashArgs.Clear();
            rotatingHashArgs.Add("rotation", target.position);
            rotatingHashArgs.Add("time", 0.1f);//changer pour rad par sec
            rotatingHashArgs.Add("oncomplete", "ShootInFront");

            iTween.RotateTo(gameObject, rotatingHashArgs);
           */

        }
        return true;
    }
    void ShootInFront()
    {
        
        Instantiate(bullet, exitPoint);
        timeSinceAttack = Time.time;
    }
}
