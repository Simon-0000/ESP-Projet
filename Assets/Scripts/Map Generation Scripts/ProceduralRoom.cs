//Auteurs: Simon Asmar
//Explication: Ce script permet d'instancier une pièce (murs,sol,plafond) et les objets associés à cette pièce

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets;
namespace Assets
{
    public class ProceduralRoom : MonoBehaviour
    {
        [SerializeField] ProceduralTiledCubeObject wallObject;
        [SerializeField] ProceduralTiledCubeObject ceilingObject;
        [SerializeField] ProceduralTiledCubeObject floorObject;
        [SerializeField] ProceduralHierarchy[] roomHierarchyRoots;

        [Range(0,1)] [SerializeField]
        float roomFillPercentage = 0.5f;

        const String ROOM_NAME = "Room";

        public GameObject InstanciateProceduralRoom(Noeud<RectangleInfo2d> roomNode,Transform parentTransform)
        {
            //On fait les «Awake» ici, puisqu'un «ProceduralObject» est utilisé comme un modèle qui nous fourni
            //l'information nécessaire pour instancier un gameObject, donc leur «Awake» ne sera jamais appelé si on
            //ne le fait pas ici

            wallObject.Awake();
            floorObject.Awake();



            //Instancier la pièce
            GameObject roomObj = new GameObject(ROOM_NAME + Random.Range(0,10000));
            roomObj.transform.parent = parentTransform;
            roomObj.transform.localPosition = Algos.Vector2dTo3dVector(roomNode.valeur.coordinates);
            roomObj.transform.localRotation = Quaternion.identity;

            BoundsManager defaultBounds = GetComponent<BoundsManager>();
            if (defaultBounds != null)
                Algos.CopyComponent(defaultBounds, roomObj);
            else
                roomObj.TryAddComponent<BoundsManager>();

            //On transforme la grandeur 2d du «RectangleInfo2d» de «roomNode» en une grandeur 3d
            Vector3 roomDimensions = Algos.Vector2dTo3dVector(roomNode.valeur.size, GameConstants.ROOM_HEIGHT);


            //Instancier les murs:
            int wallVariation = Random.Range(0, wallObject.objectVariations.Length);
            for (int i = 0; i < wallObject.GetComponent<ProceduralObject>().positions.Length; ++i)
                wallObject.InstantiateProceduralTiledObject(roomObj.transform, roomDimensions, wallVariation, i);

            //Instancier le sol:
            int floorVariation = Random.Range(0, floorObject.objectVariations.Length);
            floorObject.InstantiateProceduralTiledObject(roomObj.transform, roomDimensions, floorVariation,0);

            //Instancier le plafond (pas implémenté pour les tests)
            int roofVariation = Random.Range(0, ceilingObject.objectVariations.Length);
            ceilingObject.InstantiateProceduralTiledObject(roomObj.transform, roomDimensions, roofVariation, 0);

            //Garder la grandeur de la pièce en mémoire
            roomObj.GetComponent<BoundsManager>().RefreshBounds();

            //Instancier les objets de la pièce
            InstantiateHierarchies(roomObj.transform,Algos.GetVector3Volume(roomDimensions) * roomFillPercentage);

            return roomObj;
        }
        private void InstantiateHierarchies(Transform parentTransform, float roomVolume)
        {
            //On donne la composante ProceduralHierarchy au parent, afin que les hiérarchies instanciées puissent
            //se fier à son volume lors de la génération
            
            parentTransform.gameObject.TryAddComponent<ProceduralHierarchy>().hierarchyVolume = roomVolume;
            
            for (int i = 0; i < roomHierarchyRoots.Length; ++i)
            {
                roomHierarchyRoots[i].InstanciateProcedural(parentTransform);
            }
            Destroy(parentTransform.gameObject.GetComponent<ProceduralHierarchy>());
        }
    }
}