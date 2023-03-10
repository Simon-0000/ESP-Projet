// fait par Olivier Castonguay
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class LazerComponent : MonoBehaviour
{
    private float floatDamage = 75f;
    public int damage;
    private ZombieBehaviour zombie;
    private float n1 = 1f;
   private float n2;
   private float angelInDeg;
   private float angelInRad;
   private Rigidbody rig;
   private float time;
   private void Awake()
   {
       rig = GetComponent<Rigidbody>();
       rig.AddRelativeForce(Vector3.forward*100);
   }

   private void Update()
   {
       time += Time.deltaTime;
       if(time>2f)
           Destroy(gameObject);
   }


   private void OnCollisionEnter(Collision collision)
   {
       if (collision.contacts[0].otherCollider.gameObject.layer == 6)
       {
           n2 = 1+collision.contacts[0].otherCollider.GetComponent<MeshRenderer>().materials[0].GetFloat("_Glossiness");
           angelInDeg = MathF.Abs(90-Vector3.Angle(rig.velocity, collision.contacts[0].normal));
           floatDamage *= Schlick(n1, n2, angelInDeg);
           damage = (int)floatDamage;
           
         
       }
       if (collision.contacts[0].otherCollider.gameObject.layer == 7)
       {
           zombie= collision.contacts[0].otherCollider.GetComponent<ZombieBehaviour>();
           zombie.TakeDamage(damage);
           Destroy(gameObject);
       }

     

   } 
   public static float Schlick(float n1, float n2, float angle)
   {
            var cosTheta = Mathf.Cos(angle*Mathf.Deg2Rad);
           float r0 = Mathf.Pow((n1 - n2) / (n1 + n2),2);
          
           float x = 1 - cosTheta;
          
           float val = r0 + (1 - r0) * Mathf.Pow(x,5);
           
           return Mathf.Clamp(val,0,1) ;
       }
}
