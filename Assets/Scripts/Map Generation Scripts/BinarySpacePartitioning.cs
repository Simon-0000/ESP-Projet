
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Assets
{
   static class BinarySpacePartitioning
   {
      static public List<Noeud<T>> GénérerBSP<T>(Noeud<T> NoeudRacine, Func<Noeud<T>, (Noeud<T>, Noeud<T>)> TryDiviserNoeud) 
      {
         Queue<Noeud<T>> noeudsÀDiviser = new();
         List<Noeud<T>> bspList = new();
         Noeud<T> noeudParent;
         (Noeud<T> subNoeudA, Noeud<T> subNoeudB) subNoeuds = (null,null);
         noeudsÀDiviser.Enqueue(NoeudRacine);

         while(noeudsÀDiviser.Count > 0)
         {
            noeudParent = noeudsÀDiviser.Dequeue();
            subNoeuds = TryDiviserNoeud(noeudParent);
            if (subNoeuds != (null,null))
            {
               noeudParent.AjouterNoeud(subNoeuds.subNoeudA);
               noeudParent.AjouterNoeud(subNoeuds.subNoeudB);
               noeudsÀDiviser.Enqueue(subNoeuds.subNoeudA);
               noeudsÀDiviser.Enqueue(subNoeuds.subNoeudB);
               bspList.Add(subNoeuds.subNoeudA);
               bspList.Add(subNoeuds.subNoeudB);
            }
         }
         return bspList;
      }
      static public List<Noeud<T>> FilterNoeudsFeuille<T>(List<Noeud<T>> bsp)
      {
         List<Noeud<T>> noeudsFeuille = new();
         for (int i = 0; i < bsp.Count; ++i)
         {
            if (bsp[i].ConnexionCount == 0)
            {
               noeudsFeuille.Add(bsp[i]);
            }
         }
         return noeudsFeuille;
      }
   }
}
