//Auteurs: Simon Asmar
//Explication: Cette classe a pour but de générer les pièces selon BSP et DFS. Ça permet aussi de trouver les
//connexions physiques entre les pièces afin d'utiliser DFS pour choisir quelques portes qui permettront de
//connecter toute la carte.

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using Unity.VisualScripting;

namespace Assets
{
   class RoomsGenerator
   {
      float roomSizeMin,roomSizeMax, grandeur;
      RectangleInfo2d mapDimensions;
      private const float ACCEPTABLE_ZERO_VALUE = 0.00001f;//Valeur approximatif de zero qui doit être utilisé à cause d'une comparaison entre un float et la valeur 0
      private GameObject doorObject;
      public RoomsGenerator(RectangleInfo2d mapDimensions, float roomSizeMin,float roomSizeMax,GameObject door)
      {
         this.roomSizeMin = roomSizeMin;
         this.roomSizeMax = roomSizeMax;
         this.mapDimensions = mapDimensions;
         doorObject = door;
      }
      public List<Noeud<RectangleInfo2d>> GenerateRooms()
      {
         List<Noeud<RectangleInfo2d>> leafNodes = Algos.FilterNoeudsFeuilles(BinarySpacePartitioning.GénérerBSP(new Noeud<RectangleInfo2d>(null, mapDimensions), TrySplitRoom));
         LinkRoomsByPhysicalConnections(leafNodes);
         leafNodes = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes,leafNodes[0], null);

         DepthFirstSearch.ConnectNodesAccordingToPath(leafNodes);
         leafNodes = leafNodes.Distinct().ToList();

         RandomlyRemoveRooms(leafNodes, 0);
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
         for (int i = 0; i < unlinkedRooms.Count - 1; ++i)
         {
            for (int j = i + 1; j < unlinkedRooms.Count; ++j)
            {
                    if (AreRoomsConnected(unlinkedRooms[i].Valeur, unlinkedRooms[j].Valeur, doorObject.GetComponent<MeshRenderer>().bounds.size.x))
               {
                  unlinkedRooms[i].NoeudsEnfants.Add(unlinkedRooms[j]);
                  unlinkedRooms[j].NoeudsEnfants.Add(unlinkedRooms[i]);
               }
            }
         }
      }

      static bool AreRoomsConnected(RectangleInfo2d roomA, RectangleInfo2d roomB,float minOverlapOnASide)
      {
            Vector2 roomOverlap = GetRoomOverlap(roomA, roomB);
            bool isConnected = roomOverlap.x >= -ACCEPTABLE_ZERO_VALUE && roomOverlap.y >= -ACCEPTABLE_ZERO_VALUE;
            bool overlapIsSufficient = roomOverlap.x >= minOverlapOnASide || roomOverlap.y >= minOverlapOnASide;
            return isConnected && overlapIsSufficient;
      }
      
      static Vector2 GetRoomOverlap(RectangleInfo2d roomA, RectangleInfo2d roomB)
      {
         Vector2 distance = Algos.GetVectorAbs(roomA.coordinates - roomB.coordinates);
         return -new Vector2(distance.x - (roomA.size.x + roomB.size.x)/2,distance.y - (roomA.size.y + roomB.size.y)/2);
      }

      const int MAX_ITERATION_ATTEMPS = 100;
      void RandomlyRemoveRooms(List<Noeud<RectangleInfo2d>> rooms, float removalPercentage)
      {
         int iterationAttemps = 0;
         while (removalPercentage > ACCEPTABLE_ZERO_VALUE && iterationAttemps <= MAX_ITERATION_ATTEMPS)
         {
            int roomIndex = Random.Range(0, rooms.Count);
            float roomPercentage = 100 * (rooms[roomIndex].Valeur.size.x * rooms[roomIndex].Valeur.size.y) / (mapDimensions.size.x * mapDimensions.size.y);
            if (rooms[roomIndex].NoeudsEnfants.Count == 1 && removalPercentage - roomPercentage > ACCEPTABLE_ZERO_VALUE)
            {
               Noeud<RectangleInfo2d>.EnleverLiensRéciproque(rooms[roomIndex], rooms[roomIndex].NoeudsEnfants[0]);
               rooms.RemoveAt(roomIndex);
               removalPercentage -= roomPercentage;
               Debug.Log(removalPercentage);
            }
            ++iterationAttemps;
         }
      }
        public void InstantiateRooms(ProceduralRoom[] roomObjects, List<Noeud<RectangleInfo2d>> roomsNodes,Transform parent) 
        {
            for (int i = 0; i < roomsNodes.Count; ++i)
            {
                int roomTypeIndex = Random.Range(0, roomObjects.Length);
                roomObjects[roomTypeIndex].InstantiateRoom(roomsNodes[i], parent);
            }

        }
   }
}
