//Auteurs: Simon Asmar
//Explication: Ce script à pour but de regrouper tous les algorithmes utilisés pour générer les pièces, le navmesh,
//             les spawn points,etc.
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


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

        bool mapHasRefreshed = true;
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
            if (mapHasRefreshed == false)
                return;

            //On détruit la carte précédente
            for (int i = 0; i < transform.childCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
            StartCoroutine(CreateMap());

        }
        IEnumerator CreateMap()
        {
            mapHasRefreshed = false;

            yield return new WaitForSeconds(0.25f);
            //On instancie le RoomsGenerator
            RoomsGenerator bspPièces = new RoomsGenerator(new RectangleInfo2d(new Vector2(longueurMap, largeurMap), transform.position), wallSizeMin, wallSizeMax, doorObject);

            //On génère les pièces
            List<Noeud<RectangleInfo2d>> noeudsPièces = bspPièces.GenerateRooms();


            //On instancie les pièces
            bspPièces.InstantiateRooms(noeudsPièces, transform, roomObjects, doorObject, windowObject);

            //On roule l'algorithme A* (pas implémenté)

            //On crée le NavMesh (pas implémenté)


            mapHasRefreshed = true;

        }
    }
}
