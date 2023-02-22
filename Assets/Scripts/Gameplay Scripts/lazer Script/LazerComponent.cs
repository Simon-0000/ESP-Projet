using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class LazerComponent : MonoBehaviour
{ 
    
    private float damage = 75f;
    private float n1 = 1f;
   private float n2;
   private float angle;
   private Rigidbody rig;
   private void Awake()
   {
       rig = GetComponent<Rigidbody>();
       rig.AddRelativeForce(Vector3.forward*1000);
   }
   
  

   private void OnCollisionEnter(Collision collision)
   {  
       n2= collision.contacts[0].otherCollider.GetComponent<MeshRenderer>().materials[0].GetFloat("_Glossiness");
       angle = Vector3.Angle(rig.velocity, collision.contacts[0].normal);
       Debug.Log(angle);
   } 
   public static float Schlick(float n1, float n2, float cosTheta) 
       {
           float r0 = (n1 - n2) / (n1 + n2);
           r0 = r0 * r0;
           float x = 1 - cosTheta;
           return r0 + (1 - r0) * x * x * x * x * x;
       }
}
