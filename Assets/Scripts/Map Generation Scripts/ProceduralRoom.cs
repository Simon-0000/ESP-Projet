using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Assets;

namespace Assets
{
    public class ProceduralRoom : ProceduralObject
    {
        [SerializeField] ProceduralTiledObject[] wallVariations;
        [SerializeField] ProceduralTiledObject[] roofVariations;
        [SerializeField] ProceduralTiledObject[] floorVariations;
        [SerializeField] GameObject openingGameObject;//On assume que l'ouverture pointe vers le devant
        
        public void InstantiateRoom(Noeud<RectangleInfo2d> roomNode)
        {
            //Instanciate each connected rooms, then instanciate each walls
            //Find each doors position
            GameObject roomObj = Instantiate(new GameObject("room"));
            //wallVariations[0].InstantiateProceduralTiledObject(new Vector3(roomNode.Valeur.grandeur.x,roomNode.Valeur.grandeur.y,0),roomObj,0,1);
        }

        private Vector3 GetRandomOpeningPosition(Vector2 cornerMin, Vector2 cornerMax,byte sideAxisA, byte sideAxisB,byte frontAxis)
        {
            Vector3 doorDimensions = openingGameObject.GetComponent<MeshRenderer>().bounds.size ;
            Vector3 VecteurPositionMin = new Vector3(cornerMin[sideAxisA] + doorDimensions[sideAxisA] / 2, cornerMin[sideAxisB] + doorDimensions[sideAxisB] / 2,cornerMin[frontAxis] + doorDimensions[frontAxis] / 2);
            Vector3 VecteurPositionMax = new Vector3(cornerMax[sideAxisA] + doorDimensions[sideAxisA] / 2, cornerMax[sideAxisB] + doorDimensions[sideAxisB] / 2,cornerMax[frontAxis] + doorDimensions[frontAxis] / 2);
            Debug.Log(GetRandomVector(VecteurPositionMin,VecteurPositionMax));
            return GetRandomVector(VecteurPositionMin,VecteurPositionMax);
        }
    }
}