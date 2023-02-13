using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuild : MonoBehaviour
{
   [SerializeField]
   GameObject floor, wall;
   private void Awake()
   {
      floor.transform.localScale = new Vector3(1,1,1);
      wall.transform.localScale = new Vector3(1, 1, 1);
   }
   public void GénérerPièce(Transform parent, Vector3 coordonnées, Vector3 grandeur) 
   {
      
      GameObject room = new GameObject("Room");
      room.transform.position = coordonnées - grandeur/2;
      room.transform.parent = parent;



      GénérerComposante(floor,room.transform, new Vector3(grandeur.x / 2, 0, grandeur.z / 2), new Vector3(grandeur.x,0.1f, grandeur.z),0,2);
      GénérerComposante(wall, room.transform, new Vector3(grandeur.x / 2, grandeur.y/2, 0), new Vector3(grandeur.x, grandeur.y, .1f), 0, 1);
      GénérerComposante(wall, room.transform, new Vector3( 0, grandeur.y/2, grandeur.z / 2), new Vector3(.1f, grandeur.y, grandeur.z), 2, 1);
      //G�n�rerComposante(wall, room.transform, new Vector3(grandeur.x / 2, 1, 0), new Vector3(grandeur.x, 2, .1f), 0, 1);
      //G�n�rerComposante(wall, room.transform, new Vector3(grandeur.x / 2, 1, 0), new Vector3(grandeur.x, 2, .1f), 0, 1);

   }

   private GameObject GénérerComposante(GameObject templateComposante, Transform parent, Vector3 centerOffset,Vector3 grandeur, byte tileAxisA, byte tileAxisB)
   {

      GameObject tuileObject = Instantiate(templateComposante);
      Renderer tuileRenderer = tuileObject.GetComponent<MeshRenderer>();
      Vector3 grandeurGlobaleTuile = tuileRenderer.bounds.size;
      Vector3 nbrTuiles = new Vector3(grandeur.x / grandeurGlobaleTuile.x, grandeur.y / grandeurGlobaleTuile.y, grandeur.z / grandeurGlobaleTuile.z);
      tuileObject.transform.localScale = nbrTuiles;
      tuileObject.transform.parent = parent;
      tuileObject.transform.localPosition = centerOffset;

      //Repeat Texture
      Material newMat = new Material(tuileRenderer.material);
      newMat.mainTexture.wrapMode = TextureWrapMode.Repeat;
      newMat.mainTextureScale = new Vector2(nbrTuiles[tileAxisA],nbrTuiles[tileAxisB]);
      newMat.mainTextureOffset = -new Vector2(tuileObject.transform.position[tileAxisA] + grandeur[tileAxisA]/2, tuileObject.transform.position[tileAxisB] + grandeur[tileAxisB]/2);
      tuileRenderer.material = newMat;

      return tuileObject;
   }
   /*
   GameObject CarrelerTuile(GameObject tuile, Vector3 grandeurTuileFinale) 
   {

      //tuileParent.GetComponent<MeshFilter>().mesh.uv = nouvelleTuileUvs;
      //tuileParent.GetComponent<MeshFilter>().mesh.uv = new Vector2[] {new Vector2(0,0), new Vector2(nouvelleTuileScale.x,0), new Vector2(0, nouvelleTuileScale.y),new Vector2(nouvelleTuileScale.x, nouvelleTuileScale.y)};
      return tuileParent;
   }
   */
}

