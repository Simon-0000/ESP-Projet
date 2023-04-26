//Auteurs: Simon Asmar et Michaël Bruneau
//Explication: Cette classe est générique est a pour but d'être utilisé dans quelques algorithmes nécessitant
//des nœuds (ex d'utilisation: BSP, DFS)

using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class Noeud<T>
    {

        public List<Noeud<T>> noeudsEnfants = new();
        public T valeur;//la valeur du noeud

        public Noeud<T> Parent { set; get; }

        public Noeud(Noeud<T> parent, T valeur)
        {
            Parent = parent;
            this.valeur = valeur;
        }
        static public bool TryFormerLienRéciproque(Noeud<T> noeudA, Noeud<T> noeudB)
        {
            bool alreadyAdded = noeudA.noeudsEnfants.Exists(n => n == noeudB);
            if (!alreadyAdded)
                FormerLienRéciproque(noeudA, noeudB);
            return !alreadyAdded;
        }
        static public void FormerLienRéciproque(Noeud<T> noeudA, Noeud<T> noeudB)
        {
            noeudA.noeudsEnfants.Add(noeudB);
            noeudB.noeudsEnfants.Add(noeudA);
        }
        static public void EnleverLienRéciproque(Noeud<T> noeudA, Noeud<T> noeudB)
        {
            noeudA.noeudsEnfants.Remove(noeudB);
            noeudB.noeudsEnfants.Remove(noeudA);
        }
        static public void ClearConnecions(List<Noeud<T>> nodes)
        {
            for (int i = 0; i < nodes.Count; ++i)
                nodes[i].noeudsEnfants.Clear();
        }
        static public List<Noeud<T>> GetUnconnectedNodesCopy(List<Noeud<T>> connectedNodes)
        {
            List<Noeud<T>> unconnectedNodes = new(connectedNodes.Count);
            for (int i = 0; i < connectedNodes.Count; ++i)
                unconnectedNodes.Add(new Noeud<T>(connectedNodes[i].Parent, connectedNodes[i].valeur));
            return unconnectedNodes;
        }

        static public void ForEachHierarchieChildren(Noeud<T>node, List<Noeud<T>> visitedNodes,Action<Noeud<T>> nodeFunction)
        {
            if(visitedNodes == null)
              visitedNodes = new();
            visitedNodes.Add(node);
            nodeFunction(node);

            for (int i = 0; i < node.noeudsEnfants.Count; ++i)
            {
                if(!visitedNodes.Exists(n=>n==node.noeudsEnfants[i]))
                {
                    ForEachHierarchieChildren(node.noeudsEnfants[i],visitedNodes,nodeFunction);
                }
            }
        }
        //CODE CI-DESSOUS FAIT PAR Michaël Bruneau
        static public List<Noeud<T>> GetParents(Noeud<T> node) =>
            GetParents(node, GameConstants.MAX_ITERATIONS);
        static public List<Noeud<T>> GetParents(Noeud<T> node, int iterationAttempts)
        {
            List<Noeud<T>> parents = new();
            parents.Add(node);
            Noeud<T> currentParent = node.Parent;
            while(currentParent != null && iterationAttempts > 0)
            {
                parents.Add(currentParent);
                currentParent = currentParent.Parent;
                --iterationAttempts;
            }
            return parents;
        }
    }
}
