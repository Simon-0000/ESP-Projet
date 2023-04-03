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
        float roomSizeMin, roomSizeMax, grandeur;
        RectangleInfo2d mapDimensions;
        private GameObject doorObject;
        Bounds doorBounds;
        public RoomsGenerator(RectangleInfo2d mapDimensions, float roomSizeMin, float roomSizeMax, GameObject door)
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
            List<Noeud<RectangleInfo2d>> leafNodes = Algos.FilterNoeudsFeuilles(BinarySpacePartitioning.GénérerBSP(new Noeud<RectangleInfo2d>(null, mapDimensions), TrySplitRoom), 0);
            LinkRoomsByPhysicalConnections(leafNodes);

            //Filtrer les connexions
            List<Noeud<RectangleInfo2d>>[] paths = new List<Noeud<RectangleInfo2d>>[2];
            paths[0] = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes, leafNodes[Random.Range(0, leafNodes.Count)], null, true);
            paths[1] = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes, leafNodes[Random.Range(0, leafNodes.Count)], leafNodes[Random.Range(0, leafNodes.Count)], true);
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

            return (new Noeud<RectangleInfo2d>(noeudParent, new RectangleInfo2d(grandeurEnfantA, noeudParent.valeur.BottomLeftCoordinates + grandeurEnfantA / 2)),
                  new Noeud<RectangleInfo2d>(noeudParent, new RectangleInfo2d(grandeurEnfantB, noeudParent.valeur.TopRightCoordinates - grandeurEnfantB / 2)));
        }
        Direction FindValidCutDirection(RectangleInfo2d dimensions)
        {
            if (Mathf.Max(dimensions.size.x, dimensions.size.y) > roomSizeMax)
                grandeur = Random.Range(roomSizeMin, Mathf.Max(dimensions.size.x, dimensions.size.y) / 2);
            else
                return Direction.None;
            bool canCutVertically = dimensions.size.x + GameConstants.ACCEPTABLE_ZERO_VALUE >= grandeur * 2;
            bool canCutHorizontally = dimensions.size.y + GameConstants.ACCEPTABLE_ZERO_VALUE >= grandeur * 2;

            Direction direction = Direction.None;

            if (canCutVertically && canCutHorizontally)
                direction = Random.Range(0, 2) == 0 ? Direction.Vertical : Direction.Horizontal;
            else if (canCutVertically)
                direction = Direction.Vertical;
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
        List<Noeud<RectangleInfo2d>> GetEmptyWindowRooms(List<Noeud<RectangleInfo2d>> roomConnections)
        {
            List<Noeud<RectangleInfo2d>> fullRoomConnections = Noeud<RectangleInfo2d>.GetUnconnectedNodesCopy(roomConnections);
            List<Noeud<RectangleInfo2d>> windowEmptyRooms = new();
            LinkRoomsByPhysicalConnections(fullRoomConnections);

            Vector2[] emptyRoomOffset = { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };

            for (int i = 0; i < fullRoomConnections.Count; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    Vector2 emptyWindowRoomSize = new Vector2((fullRoomConnections[i].valeur.size.x - GameConstants.OVERLAP_TOLERANCE) * Mathf.Abs(emptyRoomOffset[j].y), (fullRoomConnections[i].valeur.size.y - GameConstants.OVERLAP_TOLERANCE) * Mathf.Abs(emptyRoomOffset[j].x));
                    Vector2 emptyWindowRoomCoords = new Vector2(fullRoomConnections[i].valeur.coordinates.x + fullRoomConnections[i].valeur.size.x * emptyRoomOffset[j].x / 2, fullRoomConnections[i].valeur.coordinates.y + fullRoomConnections[i].valeur.size.y * emptyRoomOffset[j].y / 2);
                    Noeud<RectangleInfo2d> emptyWindowRoom = new Noeud<RectangleInfo2d>(null, new RectangleInfo2d(emptyWindowRoomSize, emptyWindowRoomCoords));

                    bool emptyWindowRoomIsValid = true;
                    for (int k = 0; k < fullRoomConnections[i].noeudsEnfants.Count; ++k)
                    {
                        if (AreRoomsConnected(emptyWindowRoom.valeur, fullRoomConnections[i].noeudsEnfants[k].valeur, 1))
                        {
                            emptyWindowRoomIsValid = false;
                            break;
                        }
                    }
                    if (emptyWindowRoomIsValid)
                    {
                        Noeud<RectangleInfo2d>.TryFormerLienRéciproque(emptyWindowRoom, fullRoomConnections[i]);
                        windowEmptyRooms.Add(emptyWindowRoom);
                    }
                }
            }
            return windowEmptyRooms;
        }

        static bool AreRoomsConnected(RectangleInfo2d roomA, RectangleInfo2d roomB, float minOverlapOnASide)
        {
            Vector2 roomOverlap = GetRoomOverlap(roomA, roomB);
            bool isConnected = roomOverlap.x >= -GameConstants.ACCEPTABLE_ZERO_VALUE && roomOverlap.y >= -GameConstants.ACCEPTABLE_ZERO_VALUE;
            bool overlapIsSufficient = roomOverlap.x >= minOverlapOnASide || roomOverlap.y >= minOverlapOnASide;
            return isConnected && overlapIsSufficient;
        }



        //RandomlyRemoveRooms retire des noeuds feuille jusqu'à ce qu'un certain pourcentage de la carte a été retiré
        void RandomlyRemoveRooms(List<Noeud<RectangleInfo2d>> rooms, float removalPercentage)
        {
            int iter = 0;
            List<Noeud<RectangleInfo2d>> availableNodesToDelete = Algos.FilterNoeudsFeuilles(rooms, 1);

            //On enlèves tous les pièces feuilles qui se trouve au centre pour ne pas avoir de trou dans la carte 
            for (int i = availableNodesToDelete.Count - 1; i >= 0; --i)
                if (IsRoomSurrounded(availableNodesToDelete[i], rooms))
                    availableNodesToDelete.RemoveAt(i);

            //On enlève les pièces feuilles aléatoirement
            while (availableNodesToDelete.Count != 0 && removalPercentage > 0)
            {
                ++iter;
                int roomIndex = Random.Range(0, availableNodesToDelete.Count);

                float roomPercentage = 100 * (availableNodesToDelete[roomIndex].valeur.size.x * availableNodesToDelete[roomIndex].valeur.size.y) / (mapDimensions.size.x * mapDimensions.size.y);
                if (removalPercentage - roomPercentage > GameConstants.ACCEPTABLE_ZERO_VALUE)
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
        static bool IsRoomSurrounded(Noeud<RectangleInfo2d> room, List<Noeud<RectangleInfo2d>> rooms)
        {
            bool[] allSidesAreTaken = new bool[4];

            for (int i = 0; i < rooms.Count; ++i)
            {
                if (allSidesAreTaken[0] == false && rooms[i].valeur.BottomLeftCoordinates.x + GameConstants.ACCEPTABLE_ZERO_VALUE < room.valeur.BottomLeftCoordinates.x)
                    allSidesAreTaken[0] = true;
                if (allSidesAreTaken[1] == false && rooms[i].valeur.BottomRightCoordinates.x - GameConstants.ACCEPTABLE_ZERO_VALUE > room.valeur.BottomRightCoordinates.x)
                    allSidesAreTaken[1] = true;
                if (allSidesAreTaken[2] == false && rooms[i].valeur.BottomLeftCoordinates.y + GameConstants.ACCEPTABLE_ZERO_VALUE < room.valeur.BottomLeftCoordinates.y)
                    allSidesAreTaken[2] = true;
                if (allSidesAreTaken[3] == false && rooms[i].valeur.TopLeftCoordinates.y - GameConstants.ACCEPTABLE_ZERO_VALUE > room.valeur.TopLeftCoordinates.y)
                    allSidesAreTaken[3] = true;
            }
            for (int i = 0; i < allSidesAreTaken.Length; ++i)
                if (allSidesAreTaken[i] == false)
                    return false;

            return true;

        }

        //InstantiateRooms s'occupe d'instancier les portes et les pièces de la carte
        public void InstantiateRooms(List<Noeud<RectangleInfo2d>> roomsNodes, Transform parent, ProceduralRoom[] possibleRooms, GameObject doorObject, GameObject windowObject)
        {
            GameObject roomParent = new GameObject("Rooms");
            roomParent.transform.parent = parent;
            GameObject doorParent = new GameObject("Doors");
            doorParent.transform.parent = parent;
            GameObject windowParent = new GameObject("Windows");
            windowParent.transform.parent = parent;

            List<Noeud<RectangleInfo2d>> windowList = GetEmptyWindowRooms(roomsNodes);
            Algos.RandomlyRemoveListElements(windowList, windowList.Count / 3);

            InstantiateObjectBetweenRooms(windowList, windowObject, windowObject.TryAddComponent<BoundsManager>().RefreshBounds(), windowParent.transform);
            InstantiateObjectBetweenRooms(roomsNodes, doorObject, doorObject.TryAddComponent<BoundsManager>().RefreshBounds(), doorParent.transform);

            for (int i = 0; i < roomsNodes.Count; ++i)
            {
                int roomTypeIndex = Random.Range(0, possibleRooms.Length);

                possibleRooms[roomTypeIndex].InstanciateProceduralRoom(roomsNodes[i], roomParent.transform);
            }
            UseAStarOnRoom(null, null);
        }
        public void UseAStarOnRoom(GameObject room, List<GameObject> objectsToConnect)
        {
            //   Bounds roomBounds = room.GetComponent<BoundsManager>().objectBounds;
            Noeud<AStarAlgorithm.AStarNodeValue> nodeA = new(null,new AStarAlgorithm.AStarNodeValue(new Vector2(0,0),0));
            Noeud<AStarAlgorithm.AStarNodeValue> nodeB = new(null,new AStarAlgorithm.AStarNodeValue(new Vector2(1, 0), 1000));
            Noeud<AStarAlgorithm.AStarNodeValue> nodeC = new(null,new AStarAlgorithm.AStarNodeValue(new Vector2(1, 1), 0));
            Noeud<AStarAlgorithm.AStarNodeValue> nodeD = new(null,new AStarAlgorithm.AStarNodeValue(new Vector2(1, 2), 0));
            Noeud<AStarAlgorithm.AStarNodeValue> nodeE = new(null, new AStarAlgorithm.AStarNodeValue(new Vector2(2, 2), 0));

            nodeA.noeudsEnfants.Add(nodeB);
            nodeB.noeudsEnfants.Add(nodeE);

            nodeA.noeudsEnfants.Add(nodeC);
            nodeC.noeudsEnfants.Add(nodeD);
            nodeD.noeudsEnfants.Add(nodeE);
            List<Noeud<AStarAlgorithm.AStarNodeValue>> path = AStarAlgorithm.GetPath(nodeA, nodeE);
            Debug.Log("AStarSIZE: " + path.Count + " , " + (path.Last() == nodeE) + " , " + AStarAlgorithm.GetPathSize(path));
     
        }


        //InstantiateDoors s'occupe de placer des portes s'il y a une connexion entre deux pièces
        static private void InstantiateObjectBetweenRooms(List<Noeud<RectangleInfo2d>> roomsNodes, GameObject templateObject, Bounds templateBounds, Transform parent)
        {
            int j;

            for (int i = 0; i < roomsNodes.Count; ++i)
            {
                j = 0;
                while (j < roomsNodes[i].noeudsEnfants.Count)
                {
                    Noeud<RectangleInfo2d> smallerRoom = roomsNodes[i], biggerRoom = roomsNodes[i].noeudsEnfants[j];
                    Vector2 roomOverlap = GetRoomOverlap(roomsNodes[i].valeur, roomsNodes[i].noeudsEnfants[j].valeur);
                    Quaternion doorRotation;
                    int planeAxis = 0;


                    if (roomOverlap.x > templateBounds.size.x)
                    {
                        doorRotation = Quaternion.identity;

                    }
                    else if (roomOverlap.y > templateBounds.size.x)
                    {
                        //Si la connexion se trouve sur l'axe des y (z en 3d), on tourne la porte
                        planeAxis = 1;
                        doorRotation = Quaternion.Euler(0, 90, 0);
                    }
                    else //Si une connexion entre deux pièces est invalide, on ignore la connexion
                    {
                        Noeud<RectangleInfo2d>.EnleverLienRéciproque(smallerRoom, biggerRoom);
                        Debug.Log("UNEXPECTED CONNECTION");
                        continue;
                    }

                    if (smallerRoom.valeur.size[planeAxis] > biggerRoom.valeur.size[planeAxis] + GameConstants.OVERLAP_TOLERANCE)
                    {
                        //++j;
                        //continue;
                        Algos.SwapValues<Noeud<RectangleInfo2d>>(ref smallerRoom, ref biggerRoom);
                    }

                    //Trouver le positionnement en 2d 
                    Vector2 distanceOffset = smallerRoom.valeur.size / 2 - new Vector2(Algos.FindRandomCut(roomOverlap.x, templateBounds.size.x), Algos.FindRandomCut(roomOverlap.y, templateBounds.size.x));
                    Vector2 connectionDirectionSign = -Algos.GetVectorSign(smallerRoom.valeur.coordinates - biggerRoom.valeur.coordinates);
                    distanceOffset = Vector2.Scale(distanceOffset, connectionDirectionSign);

                    //Traduire le positionnement 2d en 3d
                    Vector3 centerOffset = Algos.Vector2dTo3dVector(distanceOffset, -(GameConstants.ROOM_HEIGHT - templateBounds.size.y) / 2);

                    //Instancier l'objet
                    GameObject.Instantiate(templateObject, Algos.Vector2dTo3dVector(smallerRoom.valeur.coordinates, 0) + centerOffset, doorRotation, parent);

                    //On enlève le lien entre les pièces pour ne pas instancier l'objet une seconde fois
                    Noeud<RectangleInfo2d>.EnleverLienRéciproque(smallerRoom, biggerRoom);
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
