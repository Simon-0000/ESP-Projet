//Auteurs: Simon Asmar
//Explication: Cette classe est générique est a pour but d'être utilisé dans quelques algorithmes nécessitant
//des nœuds (ex d'utilisation: BSP, DFS)

using System.Collections.Generic;



namespace Assets
{
   public class Noeud <T>
   {
      public List<Noeud<T>> NoeudsEnfants;
      Noeud<T> parent;
      T valeur;//la valeur du noeud
      
      public Noeud<T> Parent { get => parent; }
      public T Valeur { 
         get => valeur;
         set => valeur = value;
      }
      public Noeud(Noeud<T> parent, T valeur) 
      {
         this.parent = parent;
         this.valeur = valeur;
         NoeudsEnfants = new();
      }
   }
}
