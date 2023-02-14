using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

namespace Assets
{
    public class ProceduralRoom : ProceduralObject
    {
        [SerializeField] ProceduralTiledObject[] wallVariations;
        [SerializeField] ProceduralTiledObject[] roofVariations;
        [SerializeField] ProceduralTiledObject[] floorVariations;

        public void InstantiateRoom(Noeud<RectangleInfo2d> roomNode)
        {
            //Instanciate each connected rooms, then instanciate each walls
        }
    }
}