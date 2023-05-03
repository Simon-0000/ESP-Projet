//Auteurs: Simon Asmar, Michael Bruneau, Sergio Abreo Alvarez, Olivier Castonguay
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
        public RoomsGenerator(Vector2 mapSize, float roomSizeMin, float roomSizeMax, GameObject door)
        {
            this.roomSizeMin = roomSizeMin;
            this.roomSizeMax = roomSizeMax;
            mapDimensions = new RectangleInfo2d(mapSize, Vector2.zero);
            doorObject = door;
            doorObject.TryAddComponent<BoundsManager>().Awake();
            doorBounds = doorObject.GetComponent<BoundsManager>().objectBoundsLocal;
        }
        public List<Noeud<RectangleInfo2d>> GenerateRooms()
        {
            //Créer les pièces et les connexions
            List<Noeud<RectangleInfo2d>> leafNodes = Algos.FilterNoeudsFeuilles(BinarySpacePartitioning.GénérerBSP(new Noeud<RectangleInfo2d>(null, mapDimensions), TrySplitRoom), 0);
            LinkRoomsByPhysicalConnections(leafNodes, doorBounds.size.x);

            //Filtrer les connexions
            List<Noeud<RectangleInfo2d>>[] paths = new List<Noeud<RectangleInfo2d>>[2];
            paths[0] = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes, leafNodes[Random.Range(0, leafNodes.Count)], null, true);
            paths[1] = DepthFirstSearch.GetPath<RectangleInfo2d>(leafNodes, leafNodes[Random.Range(0, leafNodes.Count)], null, true);
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
        static void LinkRoomsByPhysicalConnections(List<Noeud<RectangleInfo2d>> unlinkedRooms, float minOverlapOnASide)
        {
            for (int i = 0; i < unlinkedRooms.Count - 1; ++i)
            {
                for (int j = i + 1; j < unlinkedRooms.Count; ++j)
                {
                    if (AreRoomsConnected(unlinkedRooms[i].valeur, unlinkedRooms[j].valeur, minOverlapOnASide))
                    {
                        Noeud<RectangleInfo2d>.TryFormerLienRéciproque(unlinkedRooms[i], unlinkedRooms[j]);
                    }
                }
            }
        }

        //GetEmptyWindowRooms permet de connecter des pièces à une pièce fantômes (si il s'agit d'un pièce extérieur) puisque cette connexion représente
        //l'endroit où une fenêtre pourrait être placée
        List<Noeud<RectangleInfo2d>> GetEmptyWindowRooms(List<Noeud<RectangleInfo2d>> roomConnections)
        {
            List<Noeud<RectangleInfo2d>> fullRoomConnections = Noeud<RectangleInfo2d>.GetUnconnectedNodesCopy(roomConnections);
            List<Noeud<RectangleInfo2d>> windowEmptyRooms = new();
            LinkRoomsByPhysicalConnections(fullRoomConnections, GameConstants.OVERLAP_TOLERANCE);

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

            List<Noeud<RectangleInfo2d>> windowRooms = GetEmptyWindowRooms(roomsNodes);
            Algos.RandomlyRemoveListElements(windowRooms, windowRooms.Count / 3);

            List<GameObject> aStarConnections = InstantiateObjectBetweenRooms(windowRooms, windowObject, windowObject.TryAddComponent<BoundsManager>().RefreshBounds(), windowParent.transform);

            aStarConnections.AddRange(InstantiateObjectBetweenRooms(roomsNodes, doorObject,
                doorObject.TryAddComponent<BoundsManager>().RefreshBounds(), doorParent.transform));

            Dictionary<Noeud<RectangleInfo2d>, ProceduralRoom> nodeToRoomTypeDictionary = new(roomsNodes.Count);


            for (int i = 0; i < roomsNodes.Count; ++i)
            {
                //La logique ci dessous (avant GameObject room =...) permet d'instancier une pièce différente des pièces collée à celle-ci
                List<ProceduralRoom> availableRooms = new(possibleRooms);
                for (int j = 0; j < roomsNodes[i].noeudsEnfants.Count; ++j)
                {
                    ProceduralRoom childProceduralRoomType = nodeToRoomTypeDictionary.GetValueOrDefault(roomsNodes[i].noeudsEnfants[j], null);
                    if (childProceduralRoomType != null)
                        availableRooms.Remove(childProceduralRoomType);
                }

                ProceduralRoom proceduralRoomType;
                if (availableRooms.Count > 0)
                    proceduralRoomType = availableRooms[Random.Range(0, availableRooms.Count)];
                else
                    proceduralRoomType = possibleRooms[Random.Range(0, possibleRooms.Length)];

                nodeToRoomTypeDictionary.Add(roomsNodes[i], proceduralRoomType);

                GameObject room = proceduralRoomType.InstanciateProceduralRoom(roomsNodes[i], roomParent.transform);
                UseAStarOnRoom(room, aStarConnections);

            }
            roomParent.transform.localRotation = Quaternion.identity;
            roomParent.transform.localPosition = Vector3.zero;

            doorParent.transform.localRotation = Quaternion.identity;
            doorParent.transform.localPosition = Vector3.zero;

            windowParent.transform.localRotation = Quaternion.identity;
            windowParent.transform.localPosition = Vector3.zero;
        }

        const float ASTAR_NODE_SIZE = 1.5f;
        const float ASTAR_NODE_HEIGHT = 2;
        const float ASTAR_OBJECT_COST_PER_CUBE = 100;
        const int ASTAR_INERMEDIATE_NODES = 4;//représente le nombre de noeuds intermédiaire qui doivent être utilisé
                                              //entre chaque noeud principal. Permet de rendre l'algorithme
                                              //plus précis pour éviter de détruire des objets qui se
                                              //trouveraient légèrement dans un noeud

        public void UseAStarOnRoom(GameObject room, List<GameObject> objectsToConnect)
        {
            Physics.SyncTransforms();

            Dictionary<Noeud<AStarAlgorithm.AStarNodeValue>, GameObject[]> NodeToObjDictionary = new();//Dictionnaire qui permet d'associé un noeud à un tableau
                                                                                                       //de GameObject (qui influencent le coût du noeud)

            List<Noeud<AStarAlgorithm.AStarNodeValue>> startEndNodes = new();//Les noeuds de départ et d'arrivée utilisés par le A*

            List<GameObject> startEndObjects = new();//Une liste des objets qui font partie de la liste objectsToConnect et de la pièce

            List<Noeud<AStarAlgorithm.AStarNodeValue>> nodesToDelete = new();//La liste des noeuds trouvés par le A* dont les objets devront être détruits

            List<Noeud<AStarAlgorithm.AStarNodeValue>> nodesToDeleteOnObjects = new();//La liste des noeuds qui touchent un objectsToConnect. Ils devront être
                                                                                      //regarder afin de déterminer quelle GameObject devra être détruit pour
                                                                                      //libérer de l'espace en avant du objectsToConnect

            //On trouve les valeurs de grandeur et quantité qui fonctionnent avec la grandeur de la pièce
            Vector3 roomDimensions3d = room.GetComponent<BoundsManager>().objectBoundsLocal.size;
            Vector2 roomDimensions = new Vector2(roomDimensions3d.x, roomDimensions3d.z);

            Vector2Int mainNodeAmount = new Vector2Int((int)(roomDimensions.x / ASTAR_NODE_SIZE), (int)(roomDimensions.y / ASTAR_NODE_SIZE));
            Vector2 nodeSize = new Vector2(roomDimensions.x / mainNodeAmount.x, roomDimensions.y / mainNodeAmount.y);
            Vector2 roomCorner = -roomDimensions / 2 + nodeSize / 2;
            Vector2Int nodeAmount = Vector2Int.Scale(mainNodeAmount, Vector2Int.one * ASTAR_INERMEDIATE_NODES);
            nodeAmount.x -= ASTAR_INERMEDIATE_NODES - 1;
            nodeAmount.y -= ASTAR_INERMEDIATE_NODES - 1;

            //On évalue les GameObject qui se trouvent à l'intérieur de chaque noeud
            for (int i = 0; i < nodeAmount.x; ++i)
            {

                for (int j = 0; j < nodeAmount.y; ++j)
                {
                    Vector2 position = roomCorner + Vector2.Scale(nodeSize, new Vector2(i * 1 / (float)ASTAR_INERMEDIATE_NODES, j * 1 / (float)ASTAR_INERMEDIATE_NODES));
                    Collider[] colliders = Physics.OverlapBox(room.transform.TransformPoint(new Vector3(position.x, -GameConstants.ROOM_HEIGHT / 2 + ASTAR_NODE_HEIGHT / 2, position.y)), new Vector3(nodeSize.x, ASTAR_NODE_HEIGHT, nodeSize.y) / 2 - Vector3.one * GameConstants.OVERLAP_TOLERANCE, room.transform.rotation);
                    
                    //Trouver toutes les instances d'objets qui sont dans le noeud
                    GameObject[] nodeObjects = colliders.Select(collider => collider.GetComponentInParent<BoundsManager>()).Where(boundManager => boundManager != null).Select(boundManager => boundManager.gameObject).Distinct().ToArray();

                    //Trouver toutes les instances d'obstacles parmi les objets du noeud
                    GameObject[] nodeObstacles = nodeObjects.Where(obj => !objectsToConnect.Exists(o => o == obj)).ToArray();

                    //Trouver toutes les instances des débuts/fins du A* parmi les objets du noeud
                    bool isStartEndNode = false;
                    int oldconnectionCount = startEndObjects.Count;

                    startEndObjects.AddRange(nodeObjects.Where(obj => objectsToConnect.Exists(o => o == obj)).ToArray());
                    if (startEndObjects.Count != oldconnectionCount)
                        isStartEndNode = true;
                    startEndObjects = startEndObjects.Distinct().ToList();

                    //Créer le noeud qui représente l'information récoltée
                    float cost = 0;
                    for (int k = 0; k < nodeObstacles.Length; ++k)
                    {
                        BoundsManager[] boundsManagers = nodeObstacles[k].GetComponentsInChildren<BoundsManager>(); 
                        for(int l = 0; l < boundsManagers.Length; ++l)
                            cost += Algos.GetVector3Volume(boundsManagers[l].objectBoundsLocal.size) * ASTAR_OBJECT_COST_PER_CUBE;
                    }
                    Noeud<AStarAlgorithm.AStarNodeValue> gridNode = new(null, new(position, cost));

                    //Connecter le noeud avec les noeuds qui sont physiquement collés à celui-ci
                    List<int> dictionaryNodesToConnect = new();
                    if (i != 0)
                        dictionaryNodesToConnect.Add((i - 1) * (nodeAmount.y) + j);//index du noeud en haut
                    if (j != 0)
                        dictionaryNodesToConnect.Add(i * (nodeAmount.y) + (j - 1));//index du noeud à gauche

                    for (int n = 0; n < dictionaryNodesToConnect.Count; ++n)
                        Noeud<AStarAlgorithm.AStarNodeValue>.TryFormerLienRéciproque(gridNode, NodeToObjDictionary.ElementAt(dictionaryNodesToConnect[n]).Key);

                    //Associer le noeuds à ses obstacles
                    NodeToObjDictionary.Add(gridNode, nodeObstacles);

                    //S'il y a lieu, ajouté le noeud qui se trouve près d'un objet de objectsToConnect pour être évalué plus tard dans le code
                    if (isStartEndNode)
                        nodesToDeleteOnObjects.Add(gridNode);
                }

            }

            //Choisir les noeuds de départ/arrivée (startEndNodes) qui devrait se trouver approximativement au centre de l'objet de départ/arrivée
            nodesToDeleteOnObjects = nodesToDeleteOnObjects.Distinct().ToList();
            startEndObjects = startEndObjects.Distinct().ToList();
            (float, int) closestNode;
            for (int i = 0; i < startEndObjects.Count; ++i)
            {
                closestNode = (10000,10000);
                for (int j = 0; j < nodesToDeleteOnObjects.Count; ++j)
                {
                    Vector3 nodePosition = room.transform.TransformPoint( new Vector3(nodesToDeleteOnObjects[j].valeur.position.x, 0, nodesToDeleteOnObjects[j].valeur.position.y));
                    float nodeDistance = (startEndObjects[i].transform.position - nodePosition).magnitude;
                    if (nodeDistance < closestNode.Item1)
                        closestNode = (nodeDistance, j);
                }
                if (closestNode != (10000, 10000))
                    startEndNodes.Add(nodesToDeleteOnObjects[closestNode.Item2]);
            }

            //Trouver les chemins A*
            for (int i = 0; i < startEndNodes.Count - 1; ++i)
            {
                var nextObjectTodelete = AStarAlgorithm.GetPath(startEndNodes[i], startEndNodes[i + 1]);

                //remettre les valeurs des noeuds à leurs valeurs par défaut
                Noeud<AStarAlgorithm.AStarNodeValue>.ForEachHierarchieChildren(startEndNodes[i], null, n => n.Parent = null);
                Noeud<AStarAlgorithm.AStarNodeValue>.ForEachHierarchieChildren(startEndNodes[i], null, n => n.valeur.visited = false);

                //Enlever le cout pour les objets qui seront détruits, afin de facilement repasser par ce noeud pour les prochains chemins A* qui seront générés 
                nextObjectTodelete.ForEach(node => node.valeur.costOffset = 0);
                nodesToDelete.AddRange(nextObjectTodelete);
            }

            //Détruire les objets associés aux noeuds du chemin trouvé par A*
            for (int i = 0; i < nodesToDelete.Count; ++i)
            {
                GameObject[] objectToDelete;
                NodeToObjDictionary.TryGetValue(nodesToDelete[i], out objectToDelete);

                if (objectToDelete != null)
                    for (int j = 0; j < objectToDelete.Length; ++j)
                        if (objectToDelete[j] != null)
                            GameObject.Destroy(objectToDelete[j]);
            }

            //Détruire les objets qui se trouvent à ASTAR_NODE_SIZE en avant de chaque objectsToConnect
            for (int i = 0; i < nodesToDeleteOnObjects.Count; ++i)
            {
                GameObject[] possibleObjectsToDelete = NodeToObjDictionary.GetValueOrDefault(nodesToDeleteOnObjects[i], null);
                if (possibleObjectsToDelete != null)
                    for (int j = 0; j < startEndObjects.Count; ++j)
                    {
                        Bounds startEndObjectBounds = startEndObjects[j].GetComponent<BoundsManager>().objectBoundsWorld;
                        startEndObjectBounds.size += startEndObjects[j].transform.forward.normalized * ASTAR_NODE_SIZE;
                        for (int k = 0; k < possibleObjectsToDelete.Length; ++k)
                        {
                            Bounds possibleObjToDeleteBounds = possibleObjectsToDelete[k].GetComponent<BoundsManager>().objectBoundsWorld;
                            if (startEndObjectBounds.Intersects(possibleObjToDeleteBounds))
                                GameObject.Destroy(possibleObjectsToDelete[k]);
                        }
                    }
            }
            Physics.SyncTransforms();

        }


        //InstantiateObjectBetweenRooms s'occupe de placer un templateObject s'il y a une connexion entre deux pièces
        static private List<GameObject> InstantiateObjectBetweenRooms(List<Noeud<RectangleInfo2d>> roomsNodes, GameObject templateObject, Bounds templateBounds, Transform parent)
        {
            int j;
            List<GameObject> objects = new();
            List<(Noeud<RectangleInfo2d>, Noeud<RectangleInfo2d>)> destroyedLinks = new();
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
                        if (biggerRoom.valeur.coordinates.y > smallerRoom.valeur.coordinates.y)
                            doorRotation = Quaternion.Euler(0, 180, 0);
                        else
                            doorRotation = Quaternion.identity;

                    }
                    else if (roomOverlap.y > templateBounds.size.x)
                    {
                        //Si la connexion se trouve sur l'axe des y (z en 3d), on tourne la porte
                        planeAxis = 1;
                        if (biggerRoom.valeur.coordinates.x > smallerRoom.valeur.coordinates.x)
                            doorRotation = Quaternion.Euler(0, 270, 0);
                        else
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
                        Algos.SwapValues<Noeud<RectangleInfo2d>>(ref smallerRoom, ref biggerRoom);
                    }

                    //Trouver le positionnement en 2d 
                    Vector2 distanceOffset = smallerRoom.valeur.size / 2 - new Vector2(Algos.FindRandomCut(roomOverlap.x, templateBounds.size.x), Algos.FindRandomCut(roomOverlap.y, templateBounds.size.x));
                    Vector2 connectionDirectionSign = -Algos.GetVectorSign(smallerRoom.valeur.coordinates - biggerRoom.valeur.coordinates);
                    distanceOffset = Vector2.Scale(distanceOffset, connectionDirectionSign);

                    //Traduire le positionnement 2d en 3d
                    Vector3 centerOffset = Algos.Vector2dTo3dVector(distanceOffset, -(GameConstants.ROOM_HEIGHT - templateBounds.size.y) / 2);


                    //Instancier l'objet
                    objects.Add(GameObject.Instantiate(templateObject, Algos.Vector2dTo3dVector(smallerRoom.valeur.coordinates, 0) + centerOffset, doorRotation, parent));
                    objects[objects.Count - 1].TryAddComponent<BoundsManager>().RefreshBounds();

                    //On enlève le lien entre les pièces pour ne pas instancier l'objet une seconde fois
                    Noeud<RectangleInfo2d>.EnleverLienRéciproque(smallerRoom, biggerRoom);
                    destroyedLinks.Add((smallerRoom, biggerRoom));
                }
            }
            for (int i = 0; i < destroyedLinks.Count; ++i)
            {
                Noeud<RectangleInfo2d>.FormerLienRéciproque(destroyedLinks[i].Item1, destroyedLinks[i].Item2);
            }
            return objects;
        }

        //GetRoomOverlap permet d'obtenir le chevauchement entre deux pièces
        static Vector2 GetRoomOverlap(RectangleInfo2d roomA, RectangleInfo2d roomB)
        {
            Vector2 distance = Algos.GetVectorAbs(roomA.coordinates - roomB.coordinates);
            return -new Vector2(distance.x - (roomA.size.x + roomB.size.x) / 2, distance.y - (roomA.size.y + roomB.size.y) / 2);
        }
    }
}
