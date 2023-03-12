//Auteurs: Simon Asmar
//Explication: Cette classe a pour but de générer les pièces selon BSP et DFS. Ça permet aussi de trouver les
//connexions physiques entre les pièces afin d'utiliser DFS pour choisir quelques portes qui permettront de
//connecter toute la carte.

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Assets
{
   class RoomsGenerator
   {
      float roomSizeMin,roomSizeMax, grandeur;
      RectangleInfo2d mapDimensions;

      public RoomsGenerator(RectangleInfo2d mapDimensions, float roomSizeMin,float roomSizeMax)
      {
         this.roomSizeMin = roomSizeMin;
         this.roomSizeMax = roomSizeMax;
         this.mapDimensions = mapDimensions;
      }
      public List<Noeud<RectangleInfo2d>> GenerateRooms()
      {
         List<Noeud<RectangleInfo2d>> leafNodes = Algos.FilterNoeudsFeuilles(BinarySpacePartitioning.GénérerBSP(new Noeud<RectangleInfo2d>(null, mapDimensions), TrySplitRoom));
         LinkRoomsByPhysicalConnections(leafNodes);
            //DFS ici(la classe existe, mais elle n'est pas utilisée pour le moment)
         //leafNodes = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes[0], null);
         //foreach (Noeud<RectangleInfo2d> leaf in leafNodes)
             //Debug.Log(leaf.NoeudsEnfants.Count);
         return leafNodes;
      }
      
      
      
      //La méthode TryDiviserPièce est utilisée par le BSP pour diviser les pièces
      (Noeud<RectangleInfo2d>, Noeud<RectangleInfo2d>) TrySplitRoom(Noeud<RectangleInfo2d> noeudParent)
      {
         Vector2 grandeurEnfantA, grandeurEnfantB;

         switch (FindValidCutDirection(noeudParent.Valeur))
         {
            case Direction.Horizontal:
               grandeurEnfantA = new(noeudParent.Valeur.size.x, Algos.FindRandomCut(noeudParent.Valeur.size.y, grandeur));
               grandeurEnfantB = new(grandeurEnfantA.x, noeudParent.Valeur.size.y - grandeurEnfantA.y);

               break;
            case Direction.Vertical:
               grandeurEnfantA = new(Algos.FindRandomCut(noeudParent.Valeur.size.x, grandeur), noeudParent.Valeur.size.y);
               grandeurEnfantB = new(noeudParent.Valeur.size.x - grandeurEnfantA.x, grandeurEnfantA.y);
               break;
            default:
               return (null, null);
         }

         return (new Noeud<RectangleInfo2d>(noeudParent, new RectangleInfo2d(grandeurEnfantA, noeudParent.Valeur.BottomLeftCoordinates + grandeurEnfantA/2)),
               new Noeud<RectangleInfo2d>(noeudParent, new RectangleInfo2d(grandeurEnfantB,  noeudParent.Valeur.TopRightCoordinates - grandeurEnfantB/2)));
      }
      Direction FindValidCutDirection(RectangleInfo2d dimensions)
      {
         grandeur = Random.Range(roomSizeMin, roomSizeMax);
         bool canCutVertically = dimensions.size.x >= grandeur  * 2;
         bool canCutHorizontally = dimensions.size.y >= grandeur * 2;
         
         Direction direction = Direction.None;
         
         if (canCutVertically && canCutHorizontally)
            direction = Random.Range(0, 2) == 0 ? Direction.Vertical : Direction.Horizontal;
         else if (canCutVertically)
            direction =  Direction.Vertical;
         else if (canCutHorizontally)
            direction = Direction.Horizontal;
         
         return direction;
      }
      
      
      
      //Cette méthode permet de créer des liens entre les pièces selon leurs connexions physiques
      //(deux pièces qui se touchent = une connexion)
      void LinkRoomsByPhysicalConnections(List<Noeud<RectangleInfo2d>> unlinkedRooms)
      {
         for (int i = 0; i < unlinkedRooms.Count; ++i)
         {
            for (int j = i + 1; j < unlinkedRooms.Count; ++j)
            {
               if (AreRoomsConnected(unlinkedRooms[i].Valeur,unlinkedRooms[j].Valeur))
               {
                  unlinkedRooms[i].NoeudsEnfants.Add(unlinkedRooms[j]);
                  unlinkedRooms[j].NoeudsEnfants.Add(unlinkedRooms[i]);
               }
            }
         }
      }
      static bool AreRoomsConnected(RectangleInfo2d roomA, RectangleInfo2d roomB)
      {
         Vector2 distance = roomA.coordinates - roomB.coordinates;
         distance = Algos.GetVectorAbs(distance);
         Vector2 roomOverlap = new(distance.x - (roomA.size.x + roomB.size.x)/2,distance.y - (roomA.size.y + roomB.size.y)/2);
         return roomOverlap.x <= 0 && roomOverlap.y <= 0;
      }
   }
}
