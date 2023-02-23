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
   private float angelInDeg;
   private float angelInRad;
   private Rigidbody rig;
   private void Awake()
   {
       rig = GetComponent<Rigidbody>();
       rig.AddRelativeForce(Vector3.forward*100);
   }
   
  

   private void OnCollisionEnter(Collision collision)
   {
       if (collision.contacts[0].otherCollider.gameObject.layer == 6)
       {
           n2 = 1- collision.contacts[0].otherCollider.GetComponent<MeshRenderer>().materials[0].GetFloat("_Glossiness");
           angelInDeg = (Vector3.Angle(rig.velocity, collision.contacts[0].normal));
           angelInRad = Mathf.Deg2Rad * angelInDeg;
           damage *= Schlick(n1, n2, angelInDeg);
           Debug.Log(damage);
           //  Debug.Log(Schlick(n1, n2, angelInRad));
       }

   } 
   public static float Schlick(float n1, float n2, float angle)
   {
            var cosTheta = -Mathf.Cos(angle*Mathf.Deg2Rad);
            //Debug.Log("cost"+cosTheta);
           float r0 = Mathf.Pow((n1 - n2) / (n1 + n2),2);
          // Debug.Log("r0"+r0);
          
           float x = 1 - cosTheta;
          // Debug.Log("x"+x);
          
           float val = r0 + (1 - r0) * Mathf.Pow(x,5);
           
           return Mathf.Clamp(val*2,0,1) ;
       }
}
