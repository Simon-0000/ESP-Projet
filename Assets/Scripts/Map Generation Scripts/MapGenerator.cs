//Auteurs: Simon Asmar
//Explication: Ce script à pour but de regrouper tous les algorithmes utilisés pour générer les pièces, le navmesh,
//             les spawn points,etc...
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
namespace Assets
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        uint longueurMap, largeurMap;
        [SerializeField]
        float wallSizeMin, wallSizeMax;
        [SerializeField]
        ProceduralRoom[] roomObjects;
        
        [SerializeField]
        GameObject doorObject;

        [SerializeField]
        GameObject windowObject;

        [SerializeField]
        private UnityEvent mapIsLoaded;

        bool isLoaded = true;
        void Awake()
        {
            Debug.Assert(roomObjects.Length != 0);
            for (int i = 0; i < roomObjects.Length; ++i)
                Debug.Assert(roomObjects[i] != null);
            Debug.Assert(doorObject != null);
            Debug.Assert(wallSizeMax + GameConstants.ACCEPTABLE_ZERO_VALUE >= wallSizeMin * 2);
            RefreshMap();
        }
        public void RefreshMap()
        {
            if (!isLoaded)
                return;
            isLoaded = false;
            //On détruit la carte précédente
            for (int i = 0; i < transform.childCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
            StartCoroutine(CreateMap());

        }
        IEnumerator CreateMap()
        {
            yield return 0;

            //On instancie le RoomsGenerator
            RoomsGenerator bspPièces = new RoomsGenerator(new Vector2(longueurMap, largeurMap), wallSizeMin, wallSizeMax, doorObject);

            //On génère les pièces
            List<Noeud<RectangleInfo2d>> noeudsPièces = bspPièces.GenerateRooms();


            //On instancie les pièces
            bspPièces.InstantiateRooms(noeudsPièces, transform, roomObjects, doorObject, windowObject);


            yield return 0;//Attendre 1 frame avant de finaliser la map

            mapIsLoaded.Invoke();
            isLoaded = true;
            Physics.SyncTransforms();//ligne de code nécessaire pour instancier le jeu

        }
    }
}
