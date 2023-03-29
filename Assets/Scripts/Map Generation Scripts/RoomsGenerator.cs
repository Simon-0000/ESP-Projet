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
      const float LEAF_NODES_REMOVE_PERCENTAGE = 25;
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
         doorObject.TryAddComponent<BoundsManager>().Awake();
         doorBounds = doorObject.GetComponent<BoundsManager>().objectBounds;
      }
      public List<Noeud<RectangleInfo2d>> GenerateRooms()
      {
         //Créer les pièces et les connexions
         List<Noeud<RectangleInfo2d>> leafNodes = Algos.FilterNoeudsFeuilles(BinarySpacePartitioning.GénérerBSP(new Noeud<RectangleInfo2d>(null, mapDimensions), TrySplitRoom),0);
         LinkRoomsByPhysicalConnections(leafNodes);

         //Filtrer les connexions
         List<Noeud<RectangleInfo2d>>[] paths = new List<Noeud<RectangleInfo2d>>[2];
         paths[0] = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes,leafNodes[Random.Range(0, leafNodes.Count)], null,true);
         paths[1] = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes,leafNodes[Random.Range(0,leafNodes.Count)], leafNodes[Random.Range(0,leafNodes.Count)],true);
         Noeud<RectangleInfo2d>.ClearConnecions(leafNodes);
         DepthFirstSearch.ConnectNodesAccordingToPath(paths[0]);
         DepthFirstSearch.ConnectNodesAccordingToPath(paths[1]);
         
         RandomlyRemoveRooms(leafNodes, LEAF_NODES_REMOVE_PERCENTAGE);
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
         bool canCutVertically = dimensions.size.x + GameConstants.ACCEPTABLE_ZERO_VALUE >= grandeur  * 2;
         bool canCutHorizontally = dimensions.size.y + GameConstants.ACCEPTABLE_ZERO_VALUE >= grandeur * 2;
         
         Direction direction = Direction.None;
         
         if (canCutVertically && canCutHorizontally)
            direction = Random.Range(0, 2) == 0 ? Direction.Vertical : Direction.Horizontal;
         else if (canCutVertically)
            direction =  Direction.Vertical;
         else if (canCutHorizontally)
            direction = Direction.Horizontal;
         
         return direction;
      }



      //LinkRoomsByPhysicalConnections permet de créer des liens entre les pièces selon leurs connexions physiques
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


      //RandomlyRemoveRooms retire des noeuds feuille jusqu'à ce qu'un certain pourcentage de la carte a été retiré
      void RandomlyRemoveRooms(List<Noeud<RectangleInfo2d>> rooms, float removalPercentage)
      {
         int iter = 0;
         List<Noeud<RectangleInfo2d>> availableNodesToDelete = Algos.FilterNoeudsFeuilles(rooms,1);

         while (availableNodesToDelete.Count != 0 && removalPercentage > 0)
         {
            ++iter;
            int roomIndex = Random.Range(0, availableNodesToDelete.Count);

            float roomPercentage = 100 * (availableNodesToDelete[roomIndex].valeur.size.x * availableNodesToDelete[roomIndex].valeur.size.y) / (mapDimensions.size.x * mapDimensions.size.y);
            if (removalPercentage - roomPercentage >  GameConstants.ACCEPTABLE_ZERO_VALUE)
            {
               //Si la pièce connectée à celle qui va se faire enlever deviendra elle aussi un noeud feuille, on l'ajoute à availableNodesToDelete
               if (availableNodesToDelete[roomIndex].noeudsEnfants[0].noeudsEnfants.Count == 2)
                  availableNodesToDelete.Add(availableNodesToDelete[roomIndex].noeudsEnfants[0]);

               //On détruit le lien entre les deux pièces, puis on enlève le noeud feuille de la liste
               Noeud<RectangleInfo2d>.EnleverLienRéciproque(availableNodesToDelete[roomIndex], availableNodesToDelete[roomIndex].noeudsEnfants[0]);
               availableNodesToDelete.RemoveAt(roomIndex);
               removalPercentage -= roomPercentage;
            }
            else
            {
                //Si on ne peut plus diminuer la grandeur de la carte, on termine la fonction
                break;
            }
         }

         //On enlève tous les pièces qui ne sont pas connectées à une autre pièce
         Algos.FilterNoeudsFeuilles(rooms, 0, true);
        }

        //InstantiateRooms s'occupe d'instancier les portes et les pièces de la carte
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

                roomObjects[roomTypeIndex].InstanciateProceduralRoom(roomsNodes[i],roomParent.transform);
            }
        }

        //InstantiateDoors s'occupe de placer des portes s'il y a une connexion entre deux pièces
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
                        doorRotation = Quaternion.identity;

                    } else if (roomOverlap.y > doorBounds.size.x)
                    {
                        //Si la connexion se trouve sur l'axe des y (z en 3d), on tourne la porte
                        planeAxis = 1;
                        doorRotation = Quaternion.Euler(0,90,0);
                    }
                    else //Si une connexion entre deux pièces est invalide, on ignore la connexion
                    {
                        Noeud<RectangleInfo2d>.EnleverLienRéciproque(roomNodesCopy[i], roomNodesCopy[i].noeudsEnfants[j]);
                        Debug.Log("UNEXPECTED CONNECTION");
                        continue ;
                    }

                    if (roomNodesCopy[i].valeur.size[planeAxis] > roomNodesCopy[i].noeudsEnfants[j].valeur.size[planeAxis] )
                    {
                        //On ignore la connexion si la pièce est plus grande que la pièce qui est connectée à celle-ci
                        //On placera la porte quand on sera revenu à la plus petite pièce dans le loop
                        ++j;
                        continue;
                    }

                    //Trouver le positionnement en 2d 
                    Vector2 distanceOffset = roomNodesCopy[i].valeur.size / 2 - new Vector2(Algos.FindRandomCut(roomOverlap.x, doorBounds.size.x), Algos.FindRandomCut(roomOverlap.y, doorBounds.size.x));
                    Vector2 connectionDirectionSign = -Algos.GetVectorSign(roomNodesCopy[i].valeur.coordinates - roomNodesCopy[i].noeudsEnfants[j].valeur.coordinates);
                    distanceOffset = Vector2.Scale(distanceOffset, connectionDirectionSign);

                    //Traduire le positionnement 2d en 3d
                    Vector3 centerOffset = Algos.Vector2dTo3dVector(distanceOffset,-(GameConstants.ROOM_HEIGHT - doorBounds.size.y)/2);
                    
                    //Instancier les portes
                    GameObject.Instantiate(doorObject, Algos.Vector2dTo3dVector(roomNodesCopy[i].valeur.coordinates, 0) + centerOffset, doorRotation, parent);
                    
                    //On enlève le lien entre les pièces pour ne pas instancier la porte une seconde fois
                    Noeud<RectangleInfo2d>.EnleverLienRéciproque(roomNodesCopy[i],roomNodesCopy[i].noeudsEnfants[j]);

                }
            }
        }

        //GetRoomOverlap permet d'obtenir le chevauchement entre deux pièces
        static Vector2 GetRoomOverlap(RectangleInfo2d roomA, RectangleInfo2d roomB)
        {
            Vector2 distance = Algos.GetVectorAbs(roomA.coordinates - roomB.coordinates);
            return -new Vector2(distance.x - (roomA.size.x + roomB.size.x) / 2, distance.y - (roomA.size.y + roomB.size.y) / 2);
        }
    }
}
