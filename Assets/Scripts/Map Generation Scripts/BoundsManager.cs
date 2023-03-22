using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class BoundsManager : MonoBehaviour
{
    public Bounds objectBounds;
    public void Awake()
    {
        objectBounds = Algos.GetRendererBounds(gameObject);
    }
}
