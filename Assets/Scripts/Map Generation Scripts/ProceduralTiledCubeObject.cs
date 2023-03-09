using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;
using System.Linq;

[RequireComponent(typeof(ProceduralObject))]
public class ProceduralTiledCubeObject : MonoBehaviour
{
    [SerializeField] 
    VectorRange[] Scales, ScaleOffsets;
    [SerializeField]
    bool wrapsAround = true;

    ProceduralObject proceduralObj;

    public void Awake() 
    {
        proceduralObj = GetComponent<ProceduralObject>();
        proceduralObj.Awake();
        Debug.Assert(proceduralObj != null);
        Debug.Assert(proceduralObj.positions.Length == Scales.Length && 
            proceduralObj.positions.Length == ScaleOffsets.Length);
        for(int i = 0; i < proceduralObj.objectVariations.Length; ++i)
            Debug.Assert(proceduralObj.objectVariations[i].GetComponent<MeshFilter>().sharedMesh.name == "Cube");
    }


    public GameObject InstantiateProceduralTiledObject(Transform parent, Vector3 parentDimensions,int variationIndex)
    {
        return InstantiateProceduralTiledObject(parent,parentDimensions,variationIndex,Random.Range(0,Scales.Length));
    }

    public GameObject InstantiateProceduralTiledObject(Transform parent, Vector3 parentDimensions, int variationIndex,int placementIndex)
    {
        GameObject tuileObject = Instantiate(proceduralObj.objectVariations[variationIndex], parent);
        Vector3 tileSize = Vector3.Scale(Scales[placementIndex].GetRandomVector(),parentDimensions) + ScaleOffsets[placementIndex].GetRandomVector();
        TileMaterial(tuileObject, tileSize);
        StretchVertices(tuileObject, tileSize);
        proceduralObj.SetRandomRelativePlacement(tuileObject, parentDimensions, placementIndex);
        return tuileObject;
    }


    public GameObject InstantiateProceduralTiledObject(Vector3 tileSize, Transform parent, Vector3 parentDimensions, int variationIndex,(VectorRange position, VectorRange orientation, VectorRange offset) placement)
    {
        GameObject tuileObject = Instantiate(proceduralObj.objectVariations[variationIndex], parent);

        TileMaterial(tuileObject, tileSize);
        StretchVertices(tuileObject, tileSize);
        proceduralObj.SetRandomRelativePlacement(tuileObject, parentDimensions, placement);

        return tuileObject;
    }


    private static int[] cubeAxisA = { 0, 0, 0 , 0, 2, 2 };
    private static int[] cubeAxisB = { 1, 2, 1, 2, 1, 1 };
    private static void TileMaterial(GameObject obj, Vector3 globalStretchSize)
    {
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        Vector3 grandeurGlobaleTuile = meshRenderer.bounds.size;
        Vector3 relativeStretchSize = new Vector3(globalStretchSize.x / grandeurGlobaleTuile.x, globalStretchSize.y / grandeurGlobaleTuile.y, globalStretchSize.z / grandeurGlobaleTuile.z);

        /*
        Material newMat = new Material(meshRenderer.material);
        newMat.mainTexture.wrapMode = TextureWrapMode.Repeat;
        newMat.mainTextureScale = new Vector2(relativeStretchSize[axisA], relativeStretchSize[axisB]);
        newMat.mainTextureOffset = -new Vector2(obj.transform.position[axisA] + relativeStretchSize[axisA] / 2, obj.transform.position[axisB] + relativeStretchSize[axisB] / 2);
        meshRenderer.material = newMat;
        */

        //NEED TO ADD WORLD OFFSET
        Vector2[] uvs = obj.GetComponent<MeshFilter>().mesh.uv;
        int[] uvIndex = obj.GetComponent<MeshFilter>().mesh.triangles.Distinct().ToArray();//Les vertices utilisé pour les uvs de chaques faces ne sont pas en ordre dans unity, donc on utilise ce array pour obtenir l'ordre

        for (int i = 0; i < 6; ++i)
        {
            for(int j = 0; j < 4; ++j)
            {
                uvs[uvIndex[i * 4 + j]].x *= relativeStretchSize[cubeAxisA[i]];
                uvs[uvIndex[i * 4 + j]].y *= relativeStretchSize[cubeAxisB[i]];
            }
        }
        obj.GetComponent<MeshFilter>().mesh.uv = uvs;
        
    }
    private static void StretchVertices(GameObject obj, Vector3 globalStretchSize)
    {

        Vector3 grandeurGlobaleTuile = obj.GetComponent<MeshRenderer>().bounds.size;
        Vector3 relativeStretchSize = new Vector3(globalStretchSize.x / grandeurGlobaleTuile.x, globalStretchSize.y / grandeurGlobaleTuile.y, globalStretchSize.z / grandeurGlobaleTuile.z);

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vector3.Scale(vertices[i], relativeStretchSize);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();


        MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider != null)
            meshCollider.sharedMesh = mesh;
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.size = globalStretchSize;
        }    
    }
}
