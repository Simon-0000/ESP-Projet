using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// modification du script pour qu'il marche dans notre jeux, sergio abreo alvarez
namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        public GameObject gameObject;
        public ZombieBehaviour zombie;
        public DroneBehaviour drone;
        public Transform transform;
        public Animator animator;
        public Rigidbody physics;
        public NavMeshAgent agent;
        public SphereCollider sphereCollider;
        public BoxCollider boxCollider;
        public CapsuleCollider capsuleCollider;
        public CharacterController characterController;
        // Add other game specific systems here

        public static Context CreateFromGameObject(GameObject gameObject) {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            ZombieBehaviour z = gameObject.GetComponent<ZombieBehaviour>();
            if (z != null)
                context.zombie = z;
            DroneBehaviour d = gameObject.GetComponent<DroneBehaviour>();
            if (d != null)
                context.drone = d;
            context.transform = gameObject.transform;
            context.animator = gameObject.GetComponent<Animator>();
            context.physics = gameObject.GetComponent<Rigidbody>();
            context.agent = gameObject.GetComponent<NavMeshAgent>();
            context.sphereCollider = gameObject.GetComponent<SphereCollider>();
            context.boxCollider = gameObject.GetComponent<BoxCollider>();
            context.capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
            context.characterController = gameObject.GetComponent<CharacterController>();
            
            // Add whatever else you need here...

            return context;
        }
    }
}