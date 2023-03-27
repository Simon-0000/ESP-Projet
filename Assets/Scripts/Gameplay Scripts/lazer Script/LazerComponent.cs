// fait par Olivier Castonguay
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;
using Assets;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Rigidbody))]
public class LazerComponent : MonoBehaviour
{
    private float floatDamage = 75f;
    private int damage=75;
    private ZombieBehaviour zombie;
    private const float n1 = 1f;
   private float n2;
   private float angelInDeg;
   private float angelInRad;
   private Rigidbody rig;
   private float time;
   private const float speed = 500f;
   [SerializeField] private int[] layers;
   


   private Vector3 lastVel;
   private void Awake()
   {
       damage = (int)floatDamage;
       rig = GetComponent<Rigidbody>();
       rig.AddRelativeForce(Vector3.forward);
       rig.velocity = transform.forward*speed;

   }

   private void Update()
   {
       
       time += Time.deltaTime;
       if(time>10f || damage<=0.001)
           Destroy(gameObject);
       
   }

   private void LateUpdate()
   {
       lastVel = rig.velocity;
   }


   private void OnCollisionEnter(Collision collision)
   {
       for (int i = 0; i < layers.Length; i++)
       {
         if (collision.contacts[0].otherCollider.gameObject.layer == layers[i])
         { 
             Bounce(collision);
             AdjustDammageToShlick(collision);
             break;



         }  
       }
       
       if (collision.contacts[0].otherCollider.gameObject.layer == 7)
       {
           DoDamageToZombie( collision);
       }

     

   }

   void Bounce(Collision collision)
   { 
       
    
       var direction= Vector3.Reflect(lastVel.normalized, collision.contacts[0].normal)*speed;
       rig.velocity = direction;

   }

   void AdjustDammageToShlick(Collision collision)
   {
        n2 = 1+collision.contacts[0].otherCollider.GetComponent<MeshRenderer>().materials[0].GetFloat("_Glossiness");
        angelInDeg = MathF.Abs(90-Vector3.Angle(rig.velocity, collision.contacts[0].normal));
       floatDamage *= Schlick(n1, n2, angelInDeg); 
       damage = (int)floatDamage;
       Debug.Log((damage));
   }

   void DoDamageToZombie(Collision collision)
   {
       zombie= collision.contacts[0].otherCollider.GetComponent<ZombieBehaviour>();
       zombie.TakeDamage(damage);
       Destroy(gameObject);
   }
   public static float Schlick(float n1, float n2, float angle)
   {
            var cosTheta = Mathf.Cos(angle*Mathf.Deg2Rad);
           float r0 = Mathf.Pow((n1 - n2) / (n1 + n2),2);
           float x = 1 - cosTheta;
           float val = r0 + (1 - r0) * Mathf.Pow(x,5);
           return Mathf.Clamp(val*2,0,1) ;
           // on retourne la valuer x2 puisuqe selon shlick, le dommage aurait été minime et 
           // défie le principe de notre jeu 
       }
}
