//Auteurs: Simon Asmar
//Explication: Une hiérarchie procédurale permet de générer tout ce qui est de type "Procedural". Chaque "Procedural" est
//compris dans le type ProceduralHierarchyNodeValue et celui-ci agit comme la valeur des noeuds pouvant chacun posséder des
//enfants (de valeur ProceduralHierarchyNodeValue).
//Pour instancier un "Procedural", on utilise un système de probabilité (0 à 100).
//On peut choisir le nombre de fois qu'on peut pigé à travers le même "Procedural" pour le générer autant de fois qu'on le désire
//Après l'instanciation d'un objet, on diminue le volume de cette hiérarchie, afin de s'assurer de ne pas trop remplir la pièce
//À noter qu'on utilise un stack pour parcourir les noeuds, donc une hiérarchie devra atteindre un enfant qui ne possède plus
//d'enfants avant de passé à une autre branche
//
//Exemple d'implémentation: une table(ProceduralObject) est à la racine de la hiérarchie et possède une probabilité
//de 100% d'être instancier. Lors de l'instanciation de cette table, il est possible d'instancier 3 assiettes
//(ProceduralObject, 50% chacun) qui seront toutes relatives à la table. On réussit à instancier 2 assiettes sur 3 et
//le volume disponible diminue proportionnellement à la table et aux assiettes,etc...
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
    public ProceduralHierarchy parentHierarchy;// le Hierarchy parent qui nous permet d'obtenir le volume restant à instancier
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
        //Trouver le parent de la hiérarchie. S'il n'y a pas de parent,
        //il n'y a pas de "hierarchyVolume" donc on ne peut pas générer la hiérarchie

        if (!TryConnectToNewParentHierarchy(rootParent))
            return null;
        GameObject hierarchyObj = new GameObject(gameObject.name);
        hierarchyObj.transform.parent = rootParent;
        hierarchyObj.transform.localPosition = Vector3.zero;
        hierarchyObj.transform.localRotation = Quaternion.identity;
        if (rootParent.GetComponent<BoundsManager>() != null)
        {
            hierarchyObj.TryAddComponent<BoundsManager>();
            hierarchyObj.GetComponent<BoundsManager>().centerMesh = true;
            hierarchyObj.GetComponent<BoundsManager>().objectBoundsWorld = rootParent.GetComponent<BoundsManager>().objectBoundsWorld;
            hierarchyObj.GetComponent<BoundsManager>().objectBoundsLocal = rootParent.GetComponent<BoundsManager>().objectBoundsLocal;

        }
        //Le volume max de cette hiérarchie correspond au volume du parent, lorsque la génération de la hiérarchie sera terminée,
        //on met à jour la valeur de volume du parent
        hierarchyVolume = parentHierarchy.hierarchyVolume;
        InstantiateHierarchy(hierarchyObj.transform);
        parentHierarchy.hierarchyVolume = hierarchyVolume;
        hierarchyObj.GetComponent<BoundsManager>().Awake();
        return hierarchyObj.gameObject;
    }
    private void InstantiateHierarchy(Transform rootParent)
    {
        //Créer un Stack de noeuds utilisés dans la surcharge/overload pour générer la hiérarchie 
        Stack<Noeud<ProceduralHierarchyNodeValue>> proceduralsQueue = new(ProceduralRootNodes);

        //Préparer les noeuds enfants à être utilisés et créer un noeud parent qui va englober notre hiérarchie.
        Noeud<ProceduralHierarchyNodeValue> parentNode = new(null, new ProceduralHierarchyNodeValue());
        parentNode.valeur.InstantiatedObj = rootParent.gameObject;
        for (int i = 0; i < ProceduralRootNodes.Length; ++i)
        {
            ProceduralRootNodes[i].Parent = parentNode;
            ProceduralRootNodes[i].valeur.proceduralComponentCount = ProceduralRootNodes[i].valeur.proceduralComponentAmount;
        }

        InstantiateHierarchy(proceduralsQueue);
    }

    private void InstantiateHierarchy(Stack<Noeud<ProceduralHierarchyNodeValue>> proceduralsStack) 
    {
        Noeud<ProceduralHierarchyNodeValue> proceduralObj;
        float objSize;
         
        while (proceduralsStack.Count > 0)// Pour chaque noeuds qui peut être instancié
        {
            proceduralObj = proceduralsStack.Peek();

            while (proceduralObj.valeur.proceduralComponentCount > 0 && hierarchyVolume > 0)//Pour chaque instance du même "Procedural" d'un noeud
            {
                --proceduralObj.valeur.proceduralComponentCount;

                if (proceduralObj.valeur.likelyhood > 0 && Random.Range(0, 100) < proceduralObj.valeur.likelyhood)//Condition qui est vraie si on pige le "Procedural" d'un noeud 
                {

                    proceduralObj.valeur.InstantiatedObj = proceduralObj.valeur.proceduralComponent.InstanciateProcedural(proceduralObj.Parent.valeur.InstantiatedObj.transform);
                    if (proceduralObj.valeur.InstantiatedObj != null)//condition qui est vraie si on a pu instancier le Procedural
                    {
                        for (int i = 0; i < proceduralObj.noeudsEnfants.Count; ++i)//Pour chaque noeud enfant, on le prépare et on les met en haut du Stack
                        {
                            proceduralObj.noeudsEnfants[i].Parent = proceduralObj;
                            proceduralsStack.Push(proceduralObj.noeudsEnfants[i]);
                            proceduralObj.noeudsEnfants[i].valeur.proceduralComponentCount = proceduralObj.noeudsEnfants[i].valeur.proceduralComponentAmount;
                        }
                        //On diminue le volume et on Peek au cas où il existe un noeud enfant qui se trouve en haut du stack
                        if(proceduralObj.valeur.InstantiatedObj.GetComponent<BoundsManager>() != null)
                            hierarchyVolume -= Algos.GetVector3Volume(proceduralObj.valeur.InstantiatedObj.GetComponent<BoundsManager>().objectBoundsWorld.size);
                        proceduralObj = proceduralsStack.Peek();
                    }
                }
            }
            parentHierarchy.hierarchyVolume = hierarchyVolume;

            proceduralsStack.Pop();
        }
    }

}
