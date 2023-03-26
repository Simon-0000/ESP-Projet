using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class BoundsManager : MonoBehaviour
{
    public Bounds objectBounds;

    public bool centerMesh = false;

    public Vector3 roomBoundsSizeOffset = Vector3.zero;


    public void Awake()
    {
        objectBounds = Algos.GetRendererBounds(gameObject);
        if (centerMesh == true)
            Algos.ChangePivotPosition(gameObject.transform, objectBounds.center);
        objectBounds.size += roomBoundsSizeOffset;
    }
}
