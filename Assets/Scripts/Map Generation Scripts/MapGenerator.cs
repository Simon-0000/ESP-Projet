//Auteurs: Simon Asmar
//Explication: Ce script à pour but de regrouper tous les algorithmes utilisés pour générer les pièces, le navmesh,
//             les spawn points,etc.
using UnityEngine;
using System.Collections.Generic;
using TreeEditor;


namespace Assets
{
   public class MapGenerator : MonoBehaviour
   {
      [SerializeField]
      uint longueurMap, largeurMap;
      [SerializeField]
      float wallSizeMin, wallSizeMax;
      [SerializeField]
      ProceduralRoom roomObject;
      
      void Awake()
      {
         RefreshMap();
      }

      public void RefreshMap()
      {
         //On détruit la carte précédente
         for (int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);
         
         //On instancie le RoomsGenerator
         RoomsGenerator bspPièces = new RoomsGenerator(new RectangleInfo2d(new Vector2(longueurMap,largeurMap),transform.position), wallSizeMin, wallSizeMax);
         
         //On génère les pièces
         List<Noeud<RectangleInfo2d>> noeudsPièces = bspPièces.GenerateRooms();
         
         //On instancie les pièces
         for (int i = 0; i < noeudsPièces.Count; ++ i)
         {
            roomObject.InstantiateRoom(noeudsPièces[i],transform);
         }
         
         //On roule l'algorithme A* (pas implémenté)
         
         //On crée le NavMesh (pas implémenté)
         
      }
   }
}