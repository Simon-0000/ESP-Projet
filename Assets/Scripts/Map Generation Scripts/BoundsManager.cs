//Auteurs: Simon Asmar
//Explication: BoundsManager a pour but de garder en m�moire le bounds initial d'un GameObject peut importe les enfants qu'on
//d�cide de lui ajout� suite � son instanciation. On peut aussi centrer le GameObject et donner un offset � sa grandeur.

using UnityEngine;
using Assets;
using System;

public class BoundsManager : MonoBehaviour
{
    [NonSerialized]
    public Bounds objectBounds;

    public bool centerMesh = false;

    public Bounds roomBoundsOffset;


    public void Awake()
    {
        RefreshBounds();
    }
    public Bounds RefreshBounds()
    {
        objectBounds = Algos.GetRendererBounds(gameObject);
        if (centerMesh == true)
            Algos.ChangePivotPosition(gameObject.transform, objectBounds.center);
        objectBounds.size += roomBoundsOffset.size;
        return objectBounds;
    }
    
}
