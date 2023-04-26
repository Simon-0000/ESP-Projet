//Auteurs: Simon Asmar,  Michaël Bruneau
//Explication: BoundsManager a pour but de garder en m�moire le bounds initial d'un GameObject peut importe les enfants qu'on
//d�cide de lui ajout� suite � son instanciation. On peut aussi centrer le GameObject et donner un offset � sa grandeur.

using UnityEngine;
using Assets;
using System;

public class BoundsManager : MonoBehaviour
{
    public Bounds objectBoundsParent,objectBoundsLocal, objectBoundsWorld;

    public bool centerMesh = false;

    public Bounds roomBoundsOffset;


    public void Awake()
    {
        RefreshBounds();
        if (centerMesh == true)
        {
            Algos.ChangePivotPosition(gameObject.transform, Algos.GetRendererBounds(gameObject).center);
        }
    }
    public Bounds RefreshBounds()
    {
        Quaternion objRotationWorld = transform.rotation;
        Quaternion objRotationParent = transform.localRotation;


        //Prendre le Bounds locale
        transform.rotation = Quaternion.identity;
        objectBoundsLocal = Algos.GetRendererBounds(gameObject);
        objectBoundsLocal.size += roomBoundsOffset.size;
        objectBoundsLocal.center = transform.position;

        //Prendre le Bounds relatif au parent 
        transform.rotation = objRotationParent;
        objectBoundsParent = TransformBounds(transform, objectBoundsLocal);

        //Retourner à la rotation initiale
        transform.rotation = objRotationWorld;
        objectBoundsWorld = TransformBounds(transform, objectBoundsLocal);

        return objectBoundsParent;
    }
    public static Bounds TransformBounds( Transform _transform, Bounds _localBounds)//La fonction à été prise et modifiée de http://answers.unity.com/answers/1114000/view.html
    {
 
        // transform the local extents' axes
        var extents = _localBounds.extents;
        var axisX = _transform.TransformVector(extents.x, 0, 0) ;
        var axisY = _transform.TransformVector(0, extents.y, 0) ;
        var axisZ = _transform.TransformVector(0, 0, extents.z) ;
 
        // sum their absolute value to get the world extents
        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        for (int i = 0; i < 3; ++i)//Cette partie a été ajoutée
            extents[i] /= _transform.localScale[i];
        return new Bounds { center = _localBounds.center, extents = extents };
    }
}
/*//Auteurs: Simon Asmar
//Explication: BoundsManager a pour but de garder en m�moire le bounds initial d'un GameObject peut importe les enfants qu'on
//d�cide de lui ajout� suite � son instanciation. On peut aussi centrer le GameObject et donner un offset � sa grandeur.

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

}
*/

/*
    public Bounds RefreshBounds()
    {
        Quaternion objRotation = transform.rotation;
        objectBoundsWorld = Algos.GetRendererBounds(gameObject);
        if (centerMesh == true)
        {
            Algos.ChangePivotPosition(gameObject.transform, objectBoundsWorld.center);
        }
        objectBoundsLocal.center = objectBoundsWorld.center;
        
        transform.rotation = Quaternion.identity;
        objectBoundsLocal = Algos.GetRendererBounds(gameObject);
        transform.rotation = objRotation;

        objectBoundsWorld.size = transform.TransformVector(transform.InverseTransformVector(objectBoundsWorld.size) + roomBoundsOffset.size);
        
        return objectBoundsWorld;
    }
*/

/*
 *     {
        Quaternion objRotationWorld = transform.rotation;
        Quaternion objRotationParent = transform.localRotation;

        objectBoundsWorld = Algos.GetRendererBounds(gameObject);
        if (centerMesh == true)
        {
            Algos.ChangePivotPosition(gameObject.transform, objectBoundsWorld.center);
        }
        objectBoundsLocal.center = objectBoundsWorld.center;

        //objectBoundsLocal.size = Algos.GetVectorAbs(rotationMatrix.MultiplyVector(objectBoundsWorld.size));
        
        
        
        transform.rotation = Quaternion.identity;
        objectBoundsLocal = Algos.GetRendererBounds(gameObject);
        objectBoundsLocal.size += roomBoundsOffset.size;
        transform.rotation = objRotationParent;
        objectBoundsWorld = TransformBounds(transform, objectBoundsLocal);
        transform.rotation = objRotationWorld;


        //objectBoundsWorld.size = transform.TransformVector(transform.InverseTransformVector(objectBoundsWorld.size) + roomBoundsOffset.size);
        
        return objectBoundsWorld;
    }





LATEST: 

    {
        Quaternion objRotationWorld = transform.rotation;
        Quaternion objRotationParent = transform.localRotation;


        transform.rotation = Quaternion.identity;
        objectBoundsLocal = Algos.GetRendererBounds(gameObject);
        objectBoundsLocal.size += roomBoundsOffset.size;
        transform.rotation = objRotationParent;
        objectBoundsWorld = TransformBounds(transform, objectBoundsLocal);
        transform.rotation = objRotationWorld;


        if (centerMesh == true)
        {
            Algos.ChangePivotPosition(gameObject.transform, objectBoundsWorld.center);
        }
        
        //objectBoundsWorld.size = transform.TransformVector(transform.InverseTransformVector(objectBoundsWorld.size) + roomBoundsOffset.size);
        
        return objectBoundsWorld;
    }
WORKING WITH BATHROOM AND LIVING: 
    public Bounds RefreshBounds()
    {
        Quaternion objRotationWorld = transform.rotation;
        Quaternion objRotationParent = transform.localRotation;


        transform.rotation = Quaternion.identity;
        objectBoundsLocal = Algos.GetRendererBounds(gameObject);
        objectBoundsLocal.size += roomBoundsOffset.size;
        transform.rotation = objRotationParent;
        objectBoundsWorld = TransformBounds(transform, objectBoundsLocal);
        transform.rotation = objRotationWorld;


        if (centerMesh == true)
        {
            Algos.ChangePivotPosition(gameObject.transform, Algos.GetRendererBounds(gameObject).center);
        }

        //objectBoundsWorld.size = transform.TransformVector(transform.InverseTransformVector(objectBoundsWorld.size) + roomBoundsOffset.size);

        return objectBoundsWorld;
    }
 */