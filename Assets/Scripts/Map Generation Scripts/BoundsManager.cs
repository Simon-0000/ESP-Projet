//Auteurs: Simon Asmar
//Explication: BoundsManager a pour but de garder en mémoire le bounds initial d'un GameObject peut importe les enfants qu'on
//décide de lui ajouté suite à son instanciation. On peut aussi centrer le GameObject et donner un offset à sa grandeur.

using UnityEngine;
using Assets;
using System;

public class BoundsManager : MonoBehaviour
{
    public Bounds objectBoundsWorld,objectBoundsLocal;

    public bool centerMesh = false;

    public Bounds roomBoundsOffset;


    public void Awake()
    {
        RefreshBounds();
    }
    public Bounds RefreshBounds()
    {
        Quaternion objRotation = transform.rotation;
        objectBoundsWorld = Algos.GetRendererBounds(gameObject);
        if (centerMesh == true)
            Algos.ChangePivotPosition(gameObject.transform, objectBoundsWorld.center);
        objectBoundsLocal.center = objectBoundsWorld.center;

        transform.rotation = Quaternion.identity;
        objectBoundsLocal = Algos.GetRendererBounds(gameObject);
        transform.rotation = objRotation;

        objectBoundsWorld.size = transform.TransformVector(transform.InverseTransformVector(objectBoundsWorld.size) + roomBoundsOffset.size);
        return objectBoundsWorld;
    }
}
