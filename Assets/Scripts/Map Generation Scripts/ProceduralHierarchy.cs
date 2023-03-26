using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralHierarchy : Procedural
{
    [Serializable]
    public class ProceduralHierarchyNodeValue
    {
        [Range(0, 100)]
        public float likelyhood;

        public Procedural proceduralComponent;

        public uint proceduralComponentAmount = 1;


        [NonSerialized]
        public GameObject InstantiatedObj;

        [NonSerialized]
        public uint proceduralComponentCount;
    }

    [SerializeField]
    public Noeud<ProceduralHierarchyNodeValue>[] ProceduralRootNodes;


    [NonSerialized]
    public ProceduralHierarchy parentHierarchy;
    [NonSerialized]
    public float hierarchyVolume;

    private void Awake()
    {
        parentHierarchy = GetComponentInParent<ProceduralHierarchy>();
    }
    public bool TryConnectToNewParentHierarchy(Transform rootParent)
    {
        parentHierarchy = rootParent.gameObject.GetComponentInParent<ProceduralHierarchy>();
        if (parentHierarchy == null)
            return false;
        return true;
    }
    public override GameObject InstanciateProcedural(Transform rootParent)
    {
        //Trouver le parent de la hi�rarchie s'il n'est pas d�j� d�fini, s'il n'y a pas de parent,
        //il n'y a pas de "hierarchyVolume" donc on ne peut pas g�n�rer la hi�rarchie

        if (!TryConnectToNewParentHierarchy(rootParent))
            return null;

        //Le volume max de cette hi�rarchie correspond au volume du parent, lorsque la g�n�ration de la hi�rarchie sera termin�,
        //on met � jour la valeur de volume du parent
        hierarchyVolume = parentHierarchy.hierarchyVolume;
        InstantiateHierarchy(rootParent);
        parentHierarchy.hierarchyVolume = hierarchyVolume;
        return null;
    }
    private void InstantiateHierarchy(Transform rootParent)
    {
        //Cr�er un Queue de noeuds utilis� dans la surcharge/overload pour g�n�rer la hi�rarchie 
        Stack<Noeud<ProceduralHierarchyNodeValue>> proceduralsQueue = new(ProceduralRootNodes);

        //Cr�er un noeud parent qui va englober notre hi�rarchie, et pr�parer les noeuds enfants � �tre utilis�s
        Noeud<ProceduralHierarchyNodeValue> parentNode = new(null, new ProceduralHierarchyNodeValue());
        parentNode.valeur.InstantiatedObj = rootParent.gameObject;
        for (int i = 0; i < ProceduralRootNodes.Length; ++i)
        {
            ProceduralRootNodes[i].Parent = parentNode;
            ProceduralRootNodes[i].valeur.proceduralComponentCount = ProceduralRootNodes[i].valeur.proceduralComponentAmount;
        }

        InstantiateHierarchy(proceduralsQueue);
    }

    private void InstantiateHierarchy(Stack<Noeud<ProceduralHierarchyNodeValue>> proceduralsQueue) 
    {
        Noeud<ProceduralHierarchyNodeValue> proceduralObj;
        float objSize;
        while (proceduralsQueue.Count > GameConstants.ACCEPTABLE_ZERO_VALUE)
        {
            proceduralObj = proceduralsQueue.Peek();
            while (proceduralObj.valeur.proceduralComponentCount > 0 && hierarchyVolume > 0)
            {
                --proceduralObj.valeur.proceduralComponentCount;

                if (proceduralObj.valeur.likelyhood > 0 && Random.Range(0, 100) < proceduralObj.valeur.likelyhood)//Condition qui est vrai si on doit instancier la pi�ce
                {
                    proceduralObj.valeur.InstantiatedObj = proceduralObj.valeur.proceduralComponent.InstanciateProcedural(proceduralObj.Parent.valeur.InstantiatedObj.transform);
                    if (proceduralObj.valeur.InstantiatedObj != null)
                    {
                        proceduralObj.valeur.InstantiatedObj.name = proceduralObj.valeur.InstantiatedObj.name + Random.Range(0, 100);
                        for (int i = 0; i < proceduralObj.noeudsEnfants.Count; ++i)
                        {
                            proceduralObj.noeudsEnfants[i].Parent = proceduralObj;
                            proceduralsQueue.Push(proceduralObj.noeudsEnfants[i]);
                            proceduralObj.noeudsEnfants[i].valeur.proceduralComponentCount = proceduralObj.noeudsEnfants[i].valeur.proceduralComponentAmount;
                        }
                        hierarchyVolume -= Algos.GetVector3Volume(proceduralObj.valeur.InstantiatedObj.GetComponent<BoundsManager>().objectBounds.size);
                        proceduralObj = proceduralsQueue.Peek();

                    }
                }
            }
            parentHierarchy.hierarchyVolume = hierarchyVolume;
            proceduralsQueue.Pop();
        }

    }
    //private void InstantiateHierarchy(Queue<Noeud<ProceduralHierarchyNodeValue>> proceduralsQueue)
    //{
    //    Noeud<ProceduralHierarchyNodeValue> proceduralObj;
    //    float objSize;
    //    while (proceduralsQueue.Count > GameConstants.ACCEPTABLE_ZERO_VALUE)
    //    {
    //        proceduralObj = proceduralsQueue.Peek();
    //        while (proceduralObj.valeur.proceduralComponentCount > 0 && hierarchyVolume > 0)
    //        {
    //            --proceduralObj.valeur.proceduralComponentCount;

    //            if (proceduralObj.valeur.likelyhood > 0 && Random.Range(0, 100) < proceduralObj.valeur.likelyhood)//Condition qui est vrai si on doit instancier la pi�ce
    //            {
    //                proceduralObj.valeur.InstantiatedObj = proceduralObj.valeur.proceduralComponent.InstanciateProcedural(proceduralObj.Parent.valeur.InstantiatedObj.transform);
    //                if (proceduralObj.valeur.InstantiatedObj != null)
    //                {
    //                    proceduralObj.valeur.InstantiatedObj.name = proceduralObj.valeur.InstantiatedObj.name + Random.Range(0, 100);
    //                    for (int i = 0; i < proceduralObj.noeudsEnfants.Count; ++i)
    //                    {
    //                        proceduralObj.noeudsEnfants[i].Parent = proceduralObj;
    //                        proceduralsQueue.Enqueue(proceduralObj.noeudsEnfants[i]);
    //                        proceduralObj.noeudsEnfants[i].valeur.proceduralComponentCount = proceduralObj.noeudsEnfants[i].valeur.proceduralComponentAmount;
    //                    }
    //                    hierarchyVolume -= Algos.GetVector3Volume(Algos.GetRendererBounds(proceduralObj.valeur.InstantiatedObj).size);
    //                }
    //            }
    //        }

    //        parentHierarchy.hierarchyVolume = hierarchyVolume;
    //        proceduralsQueue.Dequeue();
    //    }

    //}
}
