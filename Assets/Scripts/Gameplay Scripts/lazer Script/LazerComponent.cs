// fait par Olivier Castonguay
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;
using Assets;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;
using System.Linq;
[RequireComponent(typeof(Rigidbody))]
public class LazerComponent : MonoBehaviour
{
    public float floatDamage = 75f;
    private int damage=75;
    
    private ZombieBehaviour zombie;
    private TrailRenderer trail;

    private const float n1 = 1f;
   private float n2;
   
   private float angelInDeg;
   private float angelInRad;
   
   private Rigidbody rig;
   
   private float time;
   
   
   private const float speed = 500f;
   
   [SerializeField] private int[] layers;
   int bounce = 0;
   private int framecount = 0;
   


   private Vector3 lastVel;
   private void Awake()
   {
       damage = (int)floatDamage;
       rig = GetComponent<Rigidbody>();
       rig.AddRelativeForce(Vector3.forward);
       rig.velocity = transform.forward*speed;
       trail = GetComponentInChildren<TrailRenderer>();
    }


    private void Update()
   {
       
       time += Time.deltaTime;
      
       if(time>10f || damage < 1)
        {
            DestroyLazer();
        }
        framecount++; 
      // if(framecount>1) 
          // GetComponentInChildren<SphereCollider>().enabled = true;
                       

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

            float shlickPercentage = AdjustDammageToShlick(collision);
             Debug.Log(++bounce);
             framecount = 0;
             if (trail != null)
             {
                trail.AddPosition(transform.position);
                trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, trail.startColor.a * shlickPercentage);
             }
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

   float AdjustDammageToShlick(Collision collision)
   {
       float shlickPercentage;
       var smoothness=collision.contacts[0].otherCollider.GetComponentInChildren<MeshRenderer>().materials[0].GetFloat("_Glossiness");
       if (smoothness <= GameConstants.ACCEPTABLE_ZERO_VALUE)
       { damage = 0;
           return 0; }
       n2 = 1 + smoothness;
        angelInDeg = MathF.Abs(90-Vector3.Angle(rig.velocity, collision.contacts[0].normal));
        Debug.Log(angelInDeg);
        shlickPercentage = Schlick(n1, n2, angelInDeg);
       floatDamage *= shlickPercentage; 
       damage = (int)floatDamage;
       return shlickPercentage;

   }

   void DoDamageToZombie(Collision collision)
   {
       zombie= collision.contacts[0].otherCollider.GetComponentInParent<ZombieBehaviour>();
       zombie.TakeDamage(damage);
       DestroyLazer();
   }
   public static float Schlick(float n1, float n2, float angle)
   {
            var cosTheta = Mathf.Cos(angle*Mathf.Deg2Rad);
           float r0 = Mathf.Pow((n1 - n2) / (n1 + n2),2);
           float x = 1 - cosTheta;
           float val = r0 + (1 - r0) * Mathf.Pow(x,5);
           return Mathf.Clamp(val*2,0,1) ;
           // on retourne la valeur x2 puisque selon shlick, le dommage aurait été minime et 
           // défie le principe de notre jeu 
   }
    void DestroyLazer()
    {
        gameObject.layer = GameConstants.INVISIBLE_LAYER;
        rig.velocity = Vector3.zero ;
        GetComponentInChildren<Renderer>().enabled = false;
        rig.isKinematic = false;
        Destroy(gameObject,trail.time);
    }
}
