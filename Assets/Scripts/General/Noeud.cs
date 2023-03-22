//Auteurs: Simon Asmar
//Explication: Cette classe est générique est a pour but d'être utilisé dans quelques algorithmes nécessitant
//des nœuds (ex d'utilisation: BSP, DFS)

using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets
{
    [Serializable]
   public class Noeud <T>
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
   }
}
