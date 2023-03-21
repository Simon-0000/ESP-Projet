//Auteurs: Simon Asmar
//Explication: Cette classe a pour but de générer les pièces selon BSP et DFS. Ça permet aussi de trouver les
//connexions physiques entre les pièces afin d'utiliser DFS pour choisir quelques portes qui permettront de
//connecter toute la carte.

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;


namespace Assets
{
   class RoomsGenerator
   {
      float roomSizeMin,roomSizeMax, grandeur;
      RectangleInfo2d mapDimensions;
      private GameObject doorObject;
      Bounds doorBounds;
      public RoomsGenerator(RectangleInfo2d mapDimensions, float roomSizeMin,float roomSizeMax,GameObject door)
      {
         this.roomSizeMin = roomSizeMin;
         this.roomSizeMax = roomSizeMax;
         this.mapDimensions = mapDimensions;
         doorObject = door;
         doorBounds = Algos.GetRendererBounds(doorObject);
      }
      public List<Noeud<RectangleInfo2d>> GenerateRooms()
      {
         List<Noeud<RectangleInfo2d>> leafNodes = Algos.FilterNoeudsFeuilles(BinarySpacePartitioning.GénérerBSP(new Noeud<RectangleInfo2d>(null, mapDimensions), TrySplitRoom));
         LinkRoomsByPhysicalConnections(leafNodes);
         List<Noeud<RectangleInfo2d>>[] connections = new List<Noeud<RectangleInfo2d>>[2];
         connections[0] = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes,leafNodes[Random.Range(0,leafNodes.Count)], null,true);
         connections[1] = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes,leafNodes[Random.Range(0,leafNodes.Count)], leafNodes[Random.Range(0,leafNodes.Count)],true);
         
         Noeud<RectangleInfo2d>.ClearConnecions(leafNodes);
         DepthFirstSearch.ConnectNodesAccordingToPath(connections[0]);
         DepthFirstSearch.ConnectNodesAccordingToPath(connections[1]);
         
         RandomlyRemoveRooms(leafNodes, 25);
         return leafNodes;
      }
      
      
      
      //La méthode TryDiviserPièce est utilisée par le BSP pour diviser les pièces
      (Noeud<RectangleInfo2d>, Noeud<RectangleInfo2d>) TrySplitRoom(Noeud<RectangleInfo2d> noeudParent)
      {
         Vector2 grandeurEnfantA, grandeurEnfantB;

         switch (FindValidCutDirection(noeudParent.valeur))
         {
            case Direction.Horizontal:
               grandeurEnfantA = new(noeudParent.valeur.size.x, Algos.FindRandomCut(noeudParent.valeur.size.y, grandeur));
               grandeurEnfantB = new(grandeurEnfantA.x, noeudParent.valeur.size.y - grandeurEnfantA.y);

               break;
            case Direction.Vertical:
               grandeurEnfantA = new(Algos.FindRandomCut(noeudParent.valeur.size.x, grandeur), noeudParent.valeur.size.y);
               grandeurEnfantB = new(noeudParent.valeur.size.x - grandeurEnfantA.x, grandeurEnfantA.y);
               break;
            default:
               return (null, null);
         }

         return (new Noeud<RectangleInfo2d>(noeudParent, new RectangleInfo2d(grandeurEnfantA, noeudParent.valeur.BottomLeftCoordinates + grandeurEnfantA/2)),
               new Noeud<RectangleInfo2d>(noeudParent, new RectangleInfo2d(grandeurEnfantB,  noeudParent.valeur.TopRightCoordinates - grandeurEnfantB/2)));
      }
      Direction FindValidCutDirection(RectangleInfo2d dimensions)
      {
         if(Mathf.Max(dimensions.size.x, dimensions.size.y) > roomSizeMax)
            grandeur = Random.Range(roomSizeMin, Mathf.Max(dimensions.size.x, dimensions.size.y)/2);
         else 
            return Direction.None; 

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
               if (AreRoomsConnected(unlinkedRooms[i].valeur, unlinkedRooms[j].valeur, doorBounds.size.x))
               {
                   Noeud<RectangleInfo2d>.TryFormerLienRéciproque(unlinkedRooms[i], unlinkedRooms[j]);
               }
            }
         }
        }

        static bool AreRoomsConnected(RectangleInfo2d roomA, RectangleInfo2d roomB,float minOverlapOnASide)
      {
            Vector2 roomOverlap = GetRoomOverlap(roomA, roomB);
            bool isConnected = roomOverlap.x >= - GameConstants.ACCEPTABLE_ZERO_VALUE && roomOverlap.y >= - GameConstants.ACCEPTABLE_ZERO_VALUE;
            bool overlapIsSufficient = roomOverlap.x >= minOverlapOnASide || roomOverlap.y >= minOverlapOnASide;
            return isConnected && overlapIsSufficient;
      }

      const int MAX_ITERATION_ATTEMPS = 100;
      void RandomlyRemoveRooms(List<Noeud<RectangleInfo2d>> rooms, float removalPercentage)
      {
         int iterationAttemps = 0;
         while (removalPercentage > GameConstants.ACCEPTABLE_ZERO_VALUE && iterationAttemps <= MAX_ITERATION_ATTEMPS)
         {
            int roomIndex = Random.Range(0, rooms.Count);
            float roomPercentage = 100 * (rooms[roomIndex].valeur.size.x * rooms[roomIndex].valeur.size.y) / (mapDimensions.size.x * mapDimensions.size.y);
            if (rooms[roomIndex].noeudsEnfants.Count == 1 && removalPercentage - roomPercentage >  GameConstants.ACCEPTABLE_ZERO_VALUE)
            {
               Noeud<RectangleInfo2d>.EnleverLienRéciproque(rooms[roomIndex], rooms[roomIndex].noeudsEnfants[0]);
               rooms.RemoveAt(roomIndex);
               removalPercentage -= roomPercentage;
            }
            ++iterationAttemps;
         }
      }

        public void InstantiateRooms(ProceduralRoom[] roomObjects, List<Noeud<RectangleInfo2d>> roomsNodes,Transform parent)
        {
           GameObject doorParent = new GameObject("Doors");
           doorParent.transform.parent = parent;
           GameObject roomParent = new GameObject("Rooms");
           roomParent.transform.parent = parent;
           
            InstantiateDoors(roomsNodes, doorParent.transform);

            for (int i = 0; i < roomsNodes.Count; ++i)
            {
                int roomTypeIndex = Random.Range(0, roomObjects.Length);
                roomObjects[roomTypeIndex].InstantiateRoom(roomsNodes[i], roomParent.transform);
            }
        }

        private void InstantiateDoors(List<Noeud<RectangleInfo2d>> roomsNodes, Transform parent)
        {
            int j;
            Noeud<RectangleInfo2d>[] roomNodesCopy =  roomsNodes.ToArray();
            for (int i = 0; i < roomsNodes.Count; ++i)
            {
                j = 0;
                while (j < roomNodesCopy[i].noeudsEnfants.Count)
                {
                    Vector2 roomOverlap = GetRoomOverlap(roomNodesCopy[i].valeur, roomNodesCopy[i].noeudsEnfants[j].valeur);
                    Quaternion doorRotation;
                    int planeAxis = 0;
                    if (roomOverlap.x > doorBounds.size.x)
                    {
                        doorRotation = Quaternion.identity;//Si la connection se trouve sur l'axe des x, on ne tourne pas la porte

                    } else if (roomOverlap.y > doorBounds.size.x)
                    {
                        planeAxis = 1;
                        doorRotation = Quaternion.Euler(0,90,0);//Si la connection se trouve sur l'axe des y(y en 2d ou z en 3d), on tourne la porte
                    }
                    else {
                        Noeud<RectangleInfo2d>.EnleverLienRéciproque(roomNodesCopy[i], roomNodesCopy[i].noeudsEnfants[j]);
                        Debug.Log("UNEXPECTED CONNECTION");
                        continue ;//Si une connection entre deux pièce est invalide on ignore la connection
                    }
                    if (roomNodesCopy[i].valeur.size[planeAxis] > roomNodesCopy[i].noeudsEnfants[j].valeur.size[planeAxis] )
                    {
                        ++j;
                        continue;
                    }
                    Vector2 distanceOffset = roomNodesCopy[i].valeur.size / 2 - new Vector2(Algos.FindRandomCut(roomOverlap.x, doorBounds.size.x), Algos.FindRandomCut(roomOverlap.y, doorBounds.size.x));
                    Vector2 connectionDirectionSign = -Algos.GetVectorSign(roomNodesCopy[i].valeur.coordinates - roomNodesCopy[i].noeudsEnfants[j].valeur.coordinates);
                    distanceOffset = Vector2.Scale(distanceOffset, connectionDirectionSign);
                    Vector3 centerOffset = Algos.Vector2dTo3dVector(distanceOffset, doorBounds.size.y / 2);
                    GameObject.Instantiate(doorObject, Algos.Vector2dTo3dVector(roomNodesCopy[i].valeur.coordinates, 0) + centerOffset, doorRotation, parent);
                    Noeud<RectangleInfo2d>.EnleverLienRéciproque(roomNodesCopy[i],roomNodesCopy[i].noeudsEnfants[j]);

                }
            }
        }

        static Vector2 GetRoomOverlap(RectangleInfo2d roomA, RectangleInfo2d roomB)
        {
            Vector2 distance = Algos.GetVectorAbs(roomA.coordinates - roomB.coordinates);
            return -new Vector2(distance.x - (roomA.size.x + roomB.size.x) / 2, distance.y - (roomA.size.y + roomB.size.y) / 2);
        }
    }
}
