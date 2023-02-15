using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTiledObject : ProceduralObject
{

    public void InstantiateProceduralTiledObject(Vector3 grandeur, GameObject parent, byte axisA, byte axisB)
    {
        int variationIndex = Random.Range(0, objectVariations.Length);
        Debug.Log(variationIndex);
        GameObject tuileObject = Instantiate(objectVariations[variationIndex],parent.transform);
        Vector3 i = tuileObject.transform.localPosition;
        SetRandomRelativePositioning(tuileObject, grandeur,new Quaternion());

        Renderer tuileRenderer = tuileObject.GetComponent<MeshRenderer>();

        Vector3 grandeurGlobaleTuile = tuileRenderer.bounds.size;
        Vector3 nbrTuiles = new Vector3(grandeur.x / grandeurGlobaleTuile.x, grandeur.y / grandeurGlobaleTuile.y, grandeur.z / grandeurGlobaleTuile.z);
        tuileObject.transform.localScale = nbrTuiles;
        tuileObject.transform.parent = parent == null ? null : parent.transform;//Erreur possible de try to used null object....

        //Repeat Texture
        Material newMat = new Material(tuileRenderer.material);
        newMat.mainTexture.wrapMode = TextureWrapMode.Repeat;
        newMat.mainTextureScale = new Vector2(nbrTuiles[axisA], nbrTuiles[axisB]);
        newMat.mainTextureOffset = -new Vector2(tuileObject.transform.position[axisA] + grandeur[axisA] / 2, tuileObject.transform.position[axisB] + grandeur[axisB] / 2);
        tuileRenderer.material = newMat;
    }

}
