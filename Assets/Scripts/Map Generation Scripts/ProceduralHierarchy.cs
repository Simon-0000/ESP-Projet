using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralHierarchy : MonoBehaviour
{
    [Serializable]
    public class ProceduralHierarchyNodeValue
    {
        [Range(0, 100)]
        public float likelyhood;

        public Procedural proceduralComponent;

        [NonSerialized]
        public GameObject InstantiatedObj;
    }

    [SerializeField]
    public Noeud<ProceduralHierarchyNodeValue>[] ProceduralRootNodes;

    public float InstantiateProceduralHierarchy(GameObject rootParentObj,float maxHierarchyVolume)
    {
        Queue<Noeud<ProceduralHierarchyNodeValue>> proceduralsQueue = new(ProceduralRootNodes);
        Noeud<ProceduralHierarchyNodeValue> proceduralObj;

        Noeud<ProceduralHierarchyNodeValue> parentNode = new(null,new ProceduralHierarchyNodeValue());
        parentNode.valeur.InstantiatedObj = rootParentObj;
        for (int i = 0; i < ProceduralRootNodes.Length; ++i)
            ProceduralRootNodes[i].Parent = parentNode;


        while (proceduralsQueue.Count > 0 && maxHierarchyVolume > 0)
        {
            proceduralObj = proceduralsQueue.Dequeue();
            if(Random.Range(0,101) < proceduralObj.valeur.likelyhood)//Condition qui est vrai si on doit instancier la pièce
            {
                proceduralObj.valeur.InstantiatedObj = proceduralObj.valeur.proceduralComponent.InstanciateProceduralObject(proceduralObj.Parent.valeur.InstantiatedObj.transform);
                for (int i = 0; i < proceduralObj.noeudsEnfants.Count; ++i)
                {
                    proceduralObj.noeudsEnfants[i].Parent = proceduralObj;
                    proceduralsQueue.Enqueue(proceduralObj.noeudsEnfants[i]);

                }

                maxHierarchyVolume -= Algos.GetVector3Volume(Algos.GetRendererBounds(proceduralObj.valeur.InstantiatedObj).size);
            }
        }
        return maxHierarchyVolume;
    }
}
