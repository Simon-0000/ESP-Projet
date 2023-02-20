using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Assets;

namespace Assets
{
    public class ProceduralRoom : ProceduralObject
    {
        [SerializeField] ProceduralTiledObject wallObject;
        [SerializeField] ProceduralTiledObject roofObject;
        [SerializeField] ProceduralTiledObject floorObject;
        [SerializeField] GameObject openingGameObject;
        
        public void InstantiateRoom(Noeud<RectangleInfo2d> roomNode, Transform parentTransform)
        {

            GameObject roomObj = new GameObject("Room");
            roomObj.transform.parent = parentTransform;
            roomObj.transform.position = new Vector3(roomNode.Valeur.coordonnées.x,0, roomNode.Valeur.coordonnées.y);

            Vector3 RoomDimensions = new Vector3(roomNode.Valeur.grandeur.x, 3, roomNode.Valeur.grandeur.y);

            (vectorRange position, vectorRange orientation) placementVector;
            placementVector.position = new vectorRange(new Vector3(0,0.49f,0.49f), new Vector3(0, 0.49f, 0.49f),0);
            placementVector.orientation = new vectorRange();

            wallObject.InstantiateProceduralTiledObject(new Vector3(roomNode.Valeur.grandeur.x, 3, 0.1f), roomObj.transform, RoomDimensions, 0, 1, 0,placementVector);
            placementVector.position.startAt = new Vector3(0, 0.49f, -0.5f);
            placementVector.position.endAt = placementVector.position.startAt;
            wallObject.InstantiateProceduralTiledObject(new Vector3(roomNode.Valeur.grandeur.x, 3, 0.1f), roomObj.transform, RoomDimensions, 0, 1, 0, placementVector);

            placementVector.position.startAt = new Vector3(0.5f, 0.49f, 0);
            placementVector.position.endAt = placementVector.position.startAt;
            wallObject.InstantiateProceduralTiledObject(new Vector3(0.1f, 3, roomNode.Valeur.grandeur.y), roomObj.transform, RoomDimensions, 2, 1, 0, placementVector);
            placementVector.position.startAt = new Vector3(-0.5f, 0.49f, 0);
            placementVector.position.endAt = placementVector.position.startAt;
            wallObject.InstantiateProceduralTiledObject(new Vector3(0.1f, 3, roomNode.Valeur.grandeur.y), roomObj.transform, RoomDimensions, 2, 1, 0, placementVector);
            
            floorObject.InstantiateProceduralTiledObject(new Vector3(roomNode.Valeur.grandeur.x, 0.1f, roomNode.Valeur.grandeur.y), roomObj.transform, RoomDimensions, 0, 2);
        }

        private Vector3 GetRandomOpeningPosition(Vector2 cornerMin, Vector2 cornerMax,byte sideAxisA, byte sideAxisB,byte frontAxis)
        {
            Vector3 doorDimensions = openingGameObject.GetComponent<MeshRenderer>().bounds.size ;
            Vector3 VecteurPositionMin = new Vector3(cornerMin[sideAxisA] + doorDimensions[sideAxisA] / 2, cornerMin[sideAxisB] + doorDimensions[sideAxisB] / 2,cornerMin[frontAxis] + doorDimensions[frontAxis] / 2);
            Vector3 VecteurPositionMax = new Vector3(cornerMax[sideAxisA] + doorDimensions[sideAxisA] / 2, cornerMax[sideAxisB] + doorDimensions[sideAxisB] / 2,cornerMax[frontAxis] + doorDimensions[frontAxis] / 2);
            Debug.Log(Algos.GetRandomVector(VecteurPositionMin,VecteurPositionMax));
            return Algos.GetRandomVector(VecteurPositionMin,VecteurPositionMax);
        }
    }
}