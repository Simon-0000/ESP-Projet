//Auteurs: Simon Asmar
//Explication: Cette classe «static» a pour but de faire un BSP et de retourner une liste contenant tous les nœuds du 
//             graphe. Il faut lui donner un nœud de départ et une fonction qui s'occupe de diviser un nœud en 2 

using System.Collections.Generic;
using System;
namespace Assets
{
   static class BinarySpacePartitioning
   {
      static public List<Noeud<T>> GénérerBSP<T>(Noeud<T> NoeudRacine, Func<Noeud<T>, (Noeud<T>, Noeud<T>)> TryDiviserNoeud) 
      {
         Queue<Noeud<T>> noeudsÀDiviser = new();
         List<Noeud<T>> bspList = new();
         Noeud<T> noeudParent;
         (Noeud<T> subNoeudA, Noeud<T> subNoeudB) subNoeuds = (null,null);//représente les nœuds résultants
         
         
         noeudsÀDiviser.Enqueue(NoeudRacine);
         while(noeudsÀDiviser.Count > 0)
         {
            noeudParent = noeudsÀDiviser.Dequeue();
            subNoeuds = TryDiviserNoeud(noeudParent);
            if (subNoeuds != (null,null))//Condition qui est true si on a pu diviser le nœud 
            {
               noeudParent.NoeudsEnfants.Add(subNoeuds.subNoeudA);
               noeudParent.NoeudsEnfants.Add(subNoeuds.subNoeudB);
               noeudsÀDiviser.Enqueue(subNoeuds.subNoeudA);
               noeudsÀDiviser.Enqueue(subNoeuds.subNoeudB);
               bspList.Add(subNoeuds.subNoeudA);
               bspList.Add(subNoeuds.subNoeudB);
            }
         }
         return bspList;
      }
   }
}
