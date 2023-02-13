using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets
{
   public class Noeud <T>
   {
      List<Noeud<T>> noeudsConnectés;
      Noeud<T> parent;
      T valeur;
      public Noeud<T> Parent { get => parent; }
      public T Valeur { get => valeur; }
      public int connexionCount { get => noeudsConnectés.Count; }
      public Noeud(Noeud<T> parent, T valeur) 
      {
         this.parent = parent;
         this.valeur = valeur;
         noeudsConnectés = new();
      }
      public void AjouterNoeud(Noeud<T> noeud) 
      {
         noeudsConnectés.Add(noeud);
      }
      public void RetirerNoeud(Noeud<T> noeud)
      {
         noeudsConnectés.Remove(noeud);
      }
   }
}
