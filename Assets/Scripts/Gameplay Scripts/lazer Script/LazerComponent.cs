using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class LazerComponent : MonoBehaviour
{ 
    
    private float damage = 75f;
   private float shininess;
   private void Awake()
   {
      GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward*1000);
   }

   private void OnCollisionEnter(Collision collision)
   {  
       shininess= collision.contacts[0].otherCollider.GetComponent<MeshRenderer>().materials[0].GetFloat("_Glossiness");
   }
}
