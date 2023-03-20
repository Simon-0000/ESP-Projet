using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets;

public class ProceduralHierarchyInfo : MonoBehaviour
{
    [Range(0, 100)]
    float likelyHood = 0;

    [Serializable]
    private class ChildProceduralObject
    {
        public ProceduralObject proceduralObj;

        [Range(-100, 100)]
        public float likelyHoodOffset = 0;
    }
    [SerializeField]
    Noeud<ChildProceduralObject> Obj;
}
