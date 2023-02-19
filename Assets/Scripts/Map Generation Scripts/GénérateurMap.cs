using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
   public class GénérateurMap : MonoBehaviour
   {
      [SerializeField]
      uint longueurMap, largeurMap, nbrPiècesMin, nbrPiècesMax;
      [SerializeField]
      float WallSizeMin, WallSizeMax;
      [SerializeField]
      ProceduralRoom roomObject;
      
      int nbrPièces;

      void Awake()
      {
         nbrPièces = (int)Random.Range(nbrPiècesMin, nbrPiècesMax + 1);
         RefreshMap();
      }

      public void RefreshMap()
      {
         for (int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);
         GénérateurPièces bspPièces = new GénérateurPièces(new RectangleInfo2d(new Vector2(longueurMap,largeurMap),transform.position), WallSizeMin, WallSizeMax);

         var noeuds = bspPièces.GénérerPièces();


         foreach (var noeud in noeuds)
         {
            roomObject.InstantiateRoom(noeud,transform);

            //InstancierPièce(noeud.Valeur);
         }
      }
      private void InstancierPièce(RectangleInfo2d dimensionsPièce) //-------------------------------------------TEMP--------------------------------------------
      {
       //  roomObject.InstanciateProceduralObject();
         
         GameObject Pièce = GameObject.CreatePrimitive(PrimitiveType.Cube);
         var Renderer = Pièce.GetComponent<MeshRenderer>();
         Pièce.transform.localPosition =new Vector3(dimensionsPièce.coordonnées.x , 0,dimensionsPièce.coordonnées.y );
         //Debug.Log(Renderer.bounds.size.x);
         //Debug.Log(dimensionsPièce.grandeur.x);
         //Debug.Log(dimensionsPièce.grandeur.x / Renderer.bounds.size.x);
         //Debug.Log(Pièce.transform.localScale);
         Pièce.transform.localScale = new Vector3(dimensionsPièce.grandeur.x / Renderer.bounds.size.x, 1, dimensionsPièce.grandeur.y/ Renderer.bounds.size.z );
         //Pièce.transform.localScale = new Vector3(dimensionsPièce.grandeur.x,1,dimensionsPièce.grandeur.y);
         Pièce.transform.parent = transform;
         Pièce.GetComponent<Renderer>().material.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
         
      }
   }
}
