using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace Assets
{
   public class Noeud <T>
   {
      public List<Noeud<T>> NoeudsConnectés;
      Noeud<T> parent;
      T valeur;
      public Noeud<T> Parent { get => parent; }
      public T Valeur { 
         get => valeur;
         set => valeur = value;
      }
      public int ConnexionCount { get => NoeudsConnectés.Count; }
      public Noeud(Noeud<T> parent, T valeur) 
      {
         this.parent = parent;
         this.valeur = valeur;
         NoeudsConnectés = new();
      }
      public void AjouterNoeud(Noeud<T> noeud) 
      {
         NoeudsConnectés.Add(noeud);
      }
      public void RetirerNoeud(int index)
      {
         NoeudsConnectés.RemoveAt(index);
      }
   }
}
