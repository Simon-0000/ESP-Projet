using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

using Assets;

namespace Assets
{
    public class ProceduralRoom : ProceduralObject
    {
        [SerializeField] ProceduralTiledCubeObject wallObject;
        [SerializeField] ProceduralTiledCubeObject roofObject;
        [SerializeField] ProceduralTiledCubeObject floorObject;
        [SerializeField] ProceduralObject openingGameObject;
        static VectorRange[] BasicWallPositions = { 
            new VectorRange(new Vector3(0, 0.5f, 0.5f), new Vector3(0, 0.5f, 0.5f)),
            new VectorRange(new Vector3(0, 0.5f, -0.5f), new Vector3(0, 0.5f, -0.5f)),
            new VectorRange(new Vector3(0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 0)),
            new VectorRange(new Vector3(-0.5f, 0.5f, 0), new Vector3(-0.5f, 0.5f, 0))
        };
        public void InstantiateRoom(Noeud<RectangleInfo2d> roomNode, Transform parentTransform)
        {
            wallObject.Awake();
            floorObject.Awake();
            GameObject roomObj = new GameObject("Room");
            roomObj.transform.parent = parentTransform;
            roomObj.transform.position = new Vector3(roomNode.Valeur.coordinates.x,0, roomNode.Valeur.coordinates.y);

            Vector3 RoomDimensions = new Vector3(roomNode.Valeur.Grandeur.x, 3, roomNode.Valeur.Grandeur.y);
            //VectorRange orientation = new();
            //VectorRange offset = new();
            int variationIndex = Random.Range(0,wallObject.GetComponent<ProceduralObject>().objectVariations.Length);
            for(int i = 0; i < wallObject.GetComponent<ProceduralObject>().positions.Length; ++i)
                wallObject.InstantiateProceduralTiledObject(roomObj.transform, RoomDimensions, variationIndex, i);
            
            //wallObject.InstantiateProceduralTiledObject(new Vector3(roomNode.Valeur.Grandeur.x, 3, 0.1f), roomObj.transform, RoomDimensions, 0, 1, 0, (BasicWallPositions[0], orientation, offset));

            // wallObject.InstantiateProceduralTiledObject(new Vector3(roomNode.Valeur.Grandeur.x, 3, 0.1f), roomObj.transform, RoomDimensions, 0, 1, 0, (BasicWallPositions[0], orientation, offset));

            //GameObject wall = wallObject.InstantiateProceduralTiledObject(new Vector3(roomNode.Valeur.Grandeur.x, 3, 0.1f), roomObj.transform, RoomDimensions, 0, 1, 0, (BasicWallPositions[1], orientation, offset));
            //openingGameObject.InstanciateProceduralObject(wall.transform);

            //wallObject.InstantiateProceduralTiledObject(new Vector3(0.1f, 3, roomNode.Valeur.Grandeur.y), roomObj.transform, RoomDimensions, 2, 1, 0, (BasicWallPositions[2], orientation, offset));
            //wallObject.InstantiateProceduralTiledObject(new Vector3(0.1f, 3, roomNode.Valeur.Grandeur.y), roomObj.transform, RoomDimensions, 2, 1, 0, (BasicWallPositions[3], orientation, offset));

            floorObject.InstantiateProceduralTiledObject(roomObj.transform, RoomDimensions,0);


            /*
            for (int i = 0; i < roomNode.ConnexionCount; ++i)
            {
                GameObject door = Instantiate(openingGameObject, roomObj.transform);
                
                door.transform.localPosition = GetRandomWallOpeningPosition(openingGameObject.GetComponent<MeshRenderer>().bounds.size,roomNode.Valeur, roomNode.NoeudsConnectés[i].Valeur);
                Debug.Log(door.transform.localPosition);
            }*/
        }


        private int GetRoomConnectionAxis(RectangleInfo2d src, RectangleInfo2d connectedRoom)
        {
            
            Vector2 distance = src.coordinates - connectedRoom.coordinates;
            distance = distance.Abs();
            Vector2 chevauchementDistance = new(distance.x - (src.Grandeur.x + connectedRoom.Grandeur.x)/2,distance.y - (src.Grandeur.y + connectedRoom.Grandeur.y)/2);
            return  chevauchementDistance.x < 0 ? 0: 2;
        }

        /*
        private Vector3 GetRandomWallOpeningPosition(Vector3 wallOpeningDimensions,RectangleInfo2d src, RectangleInfo2d connectedRoom)
        {
            int axisA = GetRoomConnectionAxis(src, connectedRoom);
            Vector3 surfaceDimensions = new Vector3();
            surfaceDimensions[axisA] = src.Grandeur[axisA == 2 ? 1 : 0];
            surfaceDimensions.y = 3;
            surfaceDimensions[3 - 1 - axisA] = src.Grandeur[(3 - 1 - axisA)== 2? 1 : 0] / 2;
            return  GetRandomRoomOpeningPosition(surfaceDimensions,wallOpeningDimensions,axisA,1);
        }
        
        private Vector3 GetRandomRoomOpeningPosition(Vector3 surfaceDimensions, Vector3 OpeningDimensions,int sideAxisA, int sideAxisB)
        {
            Vector3 VecteurPositionMin = new Vector3();
            VecteurPositionMin[sideAxisA] = OpeningDimensions[sideAxisA] / 2;
            VecteurPositionMin[sideAxisB] = OpeningDimensions[sideAxisB] / 2;
            Vector3 VecteurPositionMax = new Vector3();
            VecteurPositionMax[sideAxisA] = surfaceDimensions[sideAxisA] - OpeningDimensions[sideAxisA] / 2;
            VecteurPositionMax[sideAxisB] = surfaceDimensions[sideAxisB] - OpeningDimensions[sideAxisB] / 2;
            return Algos.GetRandomVector(VecteurPositionMin,VecteurPositionMax);
        }*/
    }
}