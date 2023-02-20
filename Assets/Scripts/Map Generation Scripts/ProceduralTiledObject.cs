using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTiledObject : ProceduralObject
{


    public GameObject InstantiateProceduralTiledObject(Vector3 tileSize, Transform parent, Vector3 parentDimensions, byte axisA, byte axisB)
    {
        int variationIndex = Random.Range(0, objectVariations.Length);
        return InstantiateProceduralTiledObject(tileSize, parent, parentDimensions, axisA, axisB, variationIndex);
    }//USELESS
    public GameObject InstantiateProceduralTiledObject(Vector3 tileSize, Transform parent, Vector3 parentDimensions, byte axisA, byte axisB,int variationIndex)
    {
        GameObject tuileObject = Instantiate(objectVariations[variationIndex], parent);

        SetRandomRelativePositioning(tuileObject, parentDimensions);
        Renderer tuileRenderer = tuileObject.GetComponent<MeshRenderer>();

        Vector3 grandeurGlobaleTuile = tuileRenderer.bounds.size;
        Vector3 nbrTuiles = new Vector3(tileSize.x / grandeurGlobaleTuile.x, tileSize.y / grandeurGlobaleTuile.y, tileSize.z / grandeurGlobaleTuile.z);
        tuileObject.transform.localScale = Vector3.Scale(tuileObject.transform.localScale, nbrTuiles);

        //Repeat Texture
        Material newMat = new Material(tuileRenderer.material);
        newMat.mainTexture.wrapMode = TextureWrapMode.Repeat;
        newMat.mainTextureScale = new Vector2(nbrTuiles[axisA], nbrTuiles[axisB]);
        newMat.mainTextureOffset = -new Vector2(tuileObject.transform.position[axisA] + tileSize[axisA] / 2, tuileObject.transform.position[axisB] + tileSize[axisB] / 2);
        tuileRenderer.material = newMat;
        return tuileObject;
    }//USELESS
    public GameObject InstantiateProceduralTiledObject(Vector3 tileSize, Transform parent, Vector3 parentDimensions, byte axisA, byte axisB, int variationIndex,(vectorRange position, vectorRange orientation) placement)
    {
        GameObject tuileObject = Instantiate(objectVariations[variationIndex], parent);

        SetRandomRelativePositioning(tuileObject, parentDimensions, placement);
        Renderer tuileRenderer = tuileObject.GetComponent<MeshRenderer>();

        Vector3 grandeurGlobaleTuile = tuileRenderer.bounds.size;
        Vector3 nbrTuiles = new Vector3(tileSize.x / grandeurGlobaleTuile.x, tileSize.y / grandeurGlobaleTuile.y, tileSize.z / grandeurGlobaleTuile.z);
        tuileObject.transform.localScale = Vector3.Scale(tuileObject.transform.localScale, nbrTuiles);

        //Repeat Texture
        Material newMat = new Material(tuileRenderer.material);
        newMat.mainTexture.wrapMode = TextureWrapMode.Repeat;
        newMat.mainTextureScale = new Vector2(nbrTuiles[axisA], nbrTuiles[axisB]);
        newMat.mainTextureOffset = -new Vector2(tuileObject.transform.position[axisA] + tileSize[axisA] / 2, tuileObject.transform.position[axisB] + tileSize[axisB] / 2);
        tuileRenderer.material = newMat;
        return tuileObject;
    }
}
