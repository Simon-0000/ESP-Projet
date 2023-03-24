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

        public uint proceduralComponentAmount = 1;

        [NonSerialized]
        public uint proceduralComponentCount;
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
        {
            ProceduralRootNodes[i].Parent = parentNode;
            ProceduralRootNodes[i].valeur.proceduralComponentCount = ProceduralRootNodes[i].valeur.proceduralComponentAmount;
        }


        while (proceduralsQueue.Count > 0 && maxHierarchyVolume > 0)
        {
            proceduralObj = proceduralsQueue.Peek();
            while (proceduralObj.valeur.proceduralComponentCount > 0)
            {
                --proceduralObj.valeur.proceduralComponentCount;

                if (proceduralObj.valeur.likelyhood > 0 && Random.Range(0, 100) < proceduralObj.valeur.likelyhood)//Condition qui est vrai si on doit instancier la pièce
                {
                    proceduralObj.valeur.InstantiatedObj = proceduralObj.valeur.proceduralComponent.InstanciateProceduralObject(proceduralObj.Parent.valeur.InstantiatedObj.transform);
                    if (proceduralObj.valeur.InstantiatedObj != null)
                    {
                        proceduralObj.valeur.InstantiatedObj.name = proceduralObj.valeur.InstantiatedObj.name + Random.Range(0, 100);
                        for (int i = 0; i < proceduralObj.noeudsEnfants.Count; ++i)
                        {
                            proceduralObj.noeudsEnfants[i].Parent = proceduralObj;
                            proceduralsQueue.Enqueue(proceduralObj.noeudsEnfants[i]);
                            proceduralObj.noeudsEnfants[i].valeur.proceduralComponentCount = proceduralObj.noeudsEnfants[i].valeur.proceduralComponentAmount;
                        }
                        maxHierarchyVolume -= Algos.GetVector3Volume(Algos.GetRendererBounds(proceduralObj.valeur.InstantiatedObj).size);
                    }
                }
            }

            proceduralsQueue.Dequeue();
        }
        return maxHierarchyVolume;
    }
}
