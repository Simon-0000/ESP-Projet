//Auteurs: Simon Asmar
//Explication: Ce script est utilisé en combinaison avec «ProceduralObject» pour étirer un cube procédural selon une
//certaine grandeur. On ne l'étire pas avec un «scale» et le «Material», mais plutôt avec les «vertices» et les «uvs»,
//pour ne pas qu'un autre objet procédural qui dérive de celle-ci soit étiré (ex d'implémentation: faire un mur, des
//ouvertures de grandeur différentes, etc...)

using UnityEngine;
using Assets;
using System.Linq;
using System.Collections.Generic;
using Parabox.CSG;


[RequireComponent(typeof(ProceduralObject))]
public class ProceduralTiledCubeObject : Procedural
{
    [SerializeField] 
    VectorRange[] scales;// Les valeurs de «scales» sont utilisées comme des uvs où 0 représente une grandeur de 0
                         // et 1 représente la grandeur du parent

    [SerializeField]
    VectorRange[] scaleOffsets; //Les décalages permettent de donner une grandeur qui vaudrait par exemple,
                                //toujours entre 1 et 2 unités. Une valeur négative va diminuer la grandeur,
                                //une valeur positive va l'agrandir.
    
    [SerializeField]
    bool wrapsAround = true;//Permet de contourner les objets qui se trouve dans le chemin (pas implémenté)
                            //(ex: un mur qui contourne une porte)
   
                            
    ProceduralObject proceduralObj;
    public void Awake() 
    {
        
        proceduralObj = GetComponent<ProceduralObject>();
        proceduralObj.Awake();
        
        Debug.Assert(proceduralObj.positions.Length == scales.Length && 
                     proceduralObj.positions.Length == scaleOffsets.Length);//Il doit y avoir autant de position
                                                                            //que de «scales» et de «scaleOffsets»
                                                                            
        //Chaque variation de l'objet à instancier doit être un cube
        for(int i = 0; i < proceduralObj.objectVariations.Length; ++i) 
            Debug.Assert(proceduralObj.objectVariations[i].GetComponent<MeshFilter>().sharedMesh.name == "Cube");
        CSG.epsilon = Mathf.Abs(GameConstants.OVERLAP_TOLERANCE);
    }
    public override GameObject InstanciateProcedural(Transform parent)
    {
        return InstantiateProceduralTiledObject(parent, Algos.GetRendererBounds(parent.gameObject).size, Random.Range(0, proceduralObj.objectVariations.Length), Random.Range(0, scales.Length));
    }
    public GameObject InstantiateProceduralTiledObject(Transform parent, Vector3 parentDimensions,int variationIndex)
    {
        return InstantiateProceduralTiledObject(parent,parentDimensions,variationIndex,Random.Range(0,scales.Length));
    }
    public GameObject InstantiateProceduralTiledObject(Transform parent, Vector3 parentDimensions, int variationIndex,int placementIndex)
    {
        GameObject tuileObject = Instantiate(proceduralObj.objectVariations[variationIndex], parent);
        Vector3 tileSize = Vector3.Scale(scales[placementIndex].GetRandomVector(),parentDimensions) + scaleOffsets[placementIndex].GetRandomVector();
        TileUvs(tuileObject, tileSize);
        StretchVertices(tuileObject, tileSize);
        proceduralObj.SetRandomRelativePlacement(ref tuileObject, parentDimensions, placementIndex);
        WrapMesh(tuileObject, tileSize);
        return tuileObject;
        
    }
    
    
    
    private void WrapMesh(GameObject obj, Vector3 size) 
    {
        if(wrapsAround == true)
        {
            Collider[] colliders = Physics.OverlapBox(obj.transform.position, size / 2);
           foreach (Collider collider in colliders)
                if (obj != collider.gameObject)
                    HollowOutMesh(obj, collider);
        }
    }
    private static void HollowOutMesh(GameObject obj, Collider collider)
    {
        if (obj == null || collider.gameObject == null || collider.gameObject.GetComponent<MeshRenderer>() == null)
            return;
        Vector3 roomOverlap = Algos.GetColliderOverlap(obj, collider);//new(distance.x - (sizeObj.x + sizeCollider.x) / 2, distance.y - (sizeObj.y + sizeCollider.y) / 2, distance.z - (sizeObj.z + sizeCollider.z) / 2);
        List<(float, int)> cuts = new();

        if (!Algos.IsColliderOverlaping(roomOverlap)) 
            return;
        try
        {
            Model result = CSG.Subtract(obj, collider.gameObject);
            obj.GetComponent<MeshFilter>().sharedMesh = Algos.CenterVertices(result.mesh);
            obj.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
            obj.TryAddComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
            Destroy(obj.GetComponent<BoxCollider>());
            //DOIT CHANGER LA LOGIQUE pusque si on tranche le bas d'un mur, le mur sera complètement décaler à cause de ça
        }
        catch
        {
            Debug.Log("ERROR: COULDNT USE CSG LIBRARY TO HOLLOW OUT TILE");
        }
    }

    //private static void ModifyMesh(GameObject obj, Model operationResult)
    //{


        //    //Vector3[] vertices = operationResult.mesh.vertices;
        //    //Vector3 MinVertices = vertices[0],MaxVertices = MinVertices, Offset;
        //    //for(int i = 0; i < vertices.Length; ++i)
        //    //{
        //    //    for(int j = 0; j < 3; ++j)
        //    //    {
        //    //        if (vertices[i][j] < MinVertices[j])
        //    //            MinVertices[j] = vertices[i][j];
        //    //        else if (vertices[i][j] > MaxVertices[j])
        //    //            MaxVertices[j] = vertices[i][j];
        //    //    }
        //    //}
        //    //Offset.x = -MinVertices.x - Mathf.Abs(MinVertices.x - MaxVertices.x) / 2;
        //    //Offset.y = -MinVertices.y - Mathf.Abs(MinVertices.y - MaxVertices.y) / 2;
        //    //Offset.z = -MinVertices.z - Mathf.Abs(MinVertices.z - MaxVertices.z) / 2;
        //    //for (int i = 0; i < vertices.Length; ++i)
        //    //{
        //    //    vertices[i] += Offset;
        //    //}
        //    //obj.GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
        //    /*
        //    GameObject intersection = new GameObject();
        //    intersection.transform.parent = obj.transform.parent;

        //    intersection.AddComponent<MeshFilter>().sharedMesh = operationResult.mesh;
        //    intersection.AddComponent<MeshRenderer>().sharedMaterials = operationResult.materials.ToArray();
        //    */
        //}




    private static int[] cubeAxisA = { 0, 0, 0 , 0, 2, 2 };//Représente les axes de chaque face/plan du cube Unity
    private static int[] cubeAxisB = { 1, 2, 1, 2, 1, 1 };
    private static void TileUvs(GameObject obj, Vector3 globalStretchSize)
    {
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        Vector3 cubeSize = meshRenderer.bounds.size;
        Vector3 relativeStretchSize = new Vector3(globalStretchSize.x / cubeSize.x, globalStretchSize.y / cubeSize.y, globalStretchSize.z / cubeSize.z);
        Vector2[] uvs = obj.GetComponent<MeshFilter>().mesh.uv;
        
        //Les «vertices» utilisées pour les «uvs» de chaque face ne sont pas en ordre croissant dans le cube de Unity,
        //donc on utilise ce array pour obtenir l'ordre (les 4 premiers «vertices» du array nous donne la première face,
        //4 prochains = 2e face,etc.)
        int[] uvIndex = obj.GetComponent<MeshFilter>().mesh.triangles.Distinct().ToArray();

        for (int i = 0; i < 6; ++i)
        {
            for(int j = 0; j < 4; ++j)
            {
                //Pas implémenté: «uvs» influencés par la position du monde pour ne pas avoir
                //                des coupures de texture entre deux pièces connectées par exemple
                uvs[uvIndex[i * 4 + j]].x *= relativeStretchSize[cubeAxisA[i]];
                uvs[uvIndex[i * 4 + j]].y *= relativeStretchSize[cubeAxisB[i]];
            }
        }
        obj.GetComponent<MeshFilter>().mesh.uv = uvs;
        
    }
    private static void StretchVertices(GameObject obj, Vector3 globalStretchSize)
    {

        Vector3 cubeSize = obj.GetComponent<MeshRenderer>().bounds.size;
        Vector3 relativeStretchSize = new Vector3(globalStretchSize.x / cubeSize.x, globalStretchSize.y / cubeSize.y, globalStretchSize.z / cubeSize.z);
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        
        //On étire les «vertices» du cube
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vector3.Scale(vertices[i], relativeStretchSize);
        }

        //On donne les nouveaux «vertices» au «mesh» et on recalcule le «mesh»
        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        //On recalcule le box collider du cube
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.size = globalStretchSize;
        }    
    }
}
