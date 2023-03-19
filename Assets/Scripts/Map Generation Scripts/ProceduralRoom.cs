//Auteurs: Simon Asmar
//Explication: Ce script permet d'instancier une pièce (murs,sol,plafond) et les objets associés à cette
//pièce (selon des probabilités (pas implémenté))

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Parabox.CSG;
using Assets;

namespace Assets
{
    public class ProceduralRoom : ProceduralObject
    {
        [SerializeField] ProceduralTiledCubeObject wallObject;
        [SerializeField] ProceduralTiledCubeObject roofObject;
        [SerializeField] ProceduralTiledCubeObject floorObject;
        [SerializeField] private ProceduralObject openingObject;
        [SerializeField] ProceduralObject[] childObjects;
        const int ROOM_HEIGHT = 3;//Pour le moment, la grandeur d'une pièce reste constante
        const String ROOM_NAME = "Room";
        

        public void InstantiateRoom(Noeud<RectangleInfo2d> roomNode, Transform parentTransform)
        {
            //On fait les «Awake» ici, puisqu'un «ProceduralObject» est utilisé comme un modèle qui nous fourni
            //l'information nécessaire pour instancier un gameObject, donc leur «Awake» ne sera jamais appelé si on
            //ne le fait pas ici
            
            wallObject.Awake();
            floorObject.Awake();
            
            
            
            //Innstancier la pièce
            GameObject roomObj = new GameObject(ROOM_NAME);
            roomObj.transform.parent = parentTransform;
            roomObj.transform.position = Algos.Vector2dTo3dVector(roomNode.valeur.coordinates);
            
            //On transforme la grandeur 2d du «RectangleInfo2d» de «roomNode» en une grandeur 3d
            Vector3 RoomDimensions = Algos.Vector2dTo3dVector(roomNode.valeur.size, ROOM_HEIGHT);

            //Instancier les murs:
            int wallVariation = Random.Range(0,wallObject.GetComponent<ProceduralObject>().objectVariations.Length);
            for(int i = 0; i < wallObject.GetComponent<ProceduralObject>().positions.Length; ++i)
                wallObject.InstantiateProceduralTiledObject(roomObj.transform, RoomDimensions, wallVariation, i);

            //Instancier le sol:
            int floorVariation = Random.Range(0, floorObject.GetComponent<ProceduralObject>().objectVariations.Length);
            floorObject.InstantiateProceduralTiledObject(roomObj.transform, RoomDimensions, floorVariation);

            //Instancier le plafond (pas implémenté pour les tests)

            //Instancier les objets de la pièce (pas implémenté)


        }
    }
}