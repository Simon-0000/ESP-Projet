//Auteurs: Simon Asmar
//Explication: Ce script est utilisé en combinaison avec «ProceduralObject» pour étirer un cube procédural selon une
//certaine grandeur. On ne l'étire pas avec un «scale» et le «Material», mais plutôt avec les «vertices» et les «uvs»,
//pour ne pas qu'un autre objet procédural qui dérive de celle-ci soit étiré (ex d'implémentation: faire un mur, des
//ouvertures de grandeur différentes, etc...)
//On a aussi la possibilité de contournée les objets qui se trouvent dans le chemin

using UnityEngine;
using Assets;
using System.Linq;
using System.Collections.Generic;
using Parabox.CSG;


public class ProceduralTiledCubeObject : ProceduralObject
{
    [SerializeField] 
    VectorRange[] scales;// Les valeurs de «scales» sont utilisées comme des uvs où 0 représente une grandeur de 0
                         // et 1 représente la grandeur du parent

    [SerializeField]
    VectorRange[] scaleOffsets; //Les décalages permettent de donner une grandeur qui vaudrait par exemple,
                                //toujours entre 1 et 2 unités. Une valeur négative va diminuer la grandeur,
                                //une valeur positive va l'agrandir.
    
    [SerializeField]
    bool wrapsAround = true;//Permet de contourner les objets qui se trouvent dans le chemin
                            //(ex: un mur qui contourne une porte)
   
                            
    public void Awake() 
    {
        
        base.Awake();
        
        Debug.Assert(positions.Length == scales.Length && 
                     positions.Length == scaleOffsets.Length);//Il doit y avoir autant de position
                                                                            //que de «scales» et de «scaleOffsets»
                                                                            
        //Chaque variation de l'objet à instancier doit être un cube
        for(int i = 0; i < objectVariations.Length; ++i) 
            Debug.Assert(objectVariations[i].GetComponent<MeshFilter>().sharedMesh.name == "Cube");
        CSG.epsilon = GameConstants.OVERLAP_TOLERANCE;
    }
    public override GameObject InstanciateProcedural(Transform parent)
    {
        BoundsManager parentBoundsManager = parent.gameObject.GetComponent<BoundsManager>();
        if (parentBoundsManager != null)
            return InstantiateProceduralTiledObject(parent, parent.gameObject.GetComponent<BoundsManager>().objectBoundsWorld.size, Random.Range(0, objectVariations.Length), Random.Range(0, scales.Length));
        else
            return InstantiateProceduralTiledObject(parent, Algos.GetRendererBounds(parent.gameObject).size, Random.Range(0, objectVariations.Length), Random.Range(0, scales.Length));
    }
    public GameObject InstantiateProceduralTiledObject(Transform parent, Vector3 parentDimensions,int variationIndex)
    {
        return InstantiateProceduralTiledObject(parent,parentDimensions,variationIndex,Random.Range(0,scales.Length));
    }
    public GameObject InstantiateProceduralTiledObject(Transform parent, Vector3 parentDimensions, int variationIndex,int placementIndex)
    {
        GameObject tuileObject = Instantiate(objectVariations[variationIndex], parent);
        Vector3 tileSize = Vector3.Scale(scales[placementIndex].GetRandomVector(),parentDimensions) + scaleOffsets[placementIndex].GetRandomVector();
        TileUvs(tuileObject, tileSize);
        StretchVertices(tuileObject, tileSize);
        TrySetRandomRelativePlacement(ref tuileObject, parentDimensions, new int[] { placementIndex });
        if (wrapsAround == true)
            WrapMesh(tuileObject, tileSize);

        return tuileObject;
        
    }


    //WrapMesh permet de trouver tous les objets valides qui rentre en collision avec obj afin de modifier
    //le mesh de obj aux objets trouvés
    private void WrapMesh(GameObject obj, Vector3 size) 
    {
        Collider[] colliders = Physics.OverlapBox(obj.transform.position, size / 2,obj.transform.rotation);
        for (int i = 0; i < colliders.Length; ++i)
        {
            if (obj != colliders[i].gameObject)
            {
                
                Vector3 roomOverlap = Algos.GetColliderOverlap((obj.transform.position,obj.GetComponent<BoundsManager>().objectBoundsLocal.size), colliders[i]);
                if (!Algos.IsColliderOverlaping(roomOverlap)) 
                    continue;
                HollowOutMesh(obj, colliders[i].gameObject);
            }
        }
    }
    private static void HollowOutMesh(GameObject obj, GameObject substractedObj)
    {
        if (obj == null || substractedObj == null || substractedObj.GetComponent<MeshRenderer>() == null)
            return;
        //Le try catch est là pour attraper les erreurs qui pourraient venir du package pb_CSG
        try
        {
            Model result = CSG.Subtract(obj, substractedObj);
            obj.GetComponent<MeshFilter>().sharedMesh = Algos.CenterVertices(result.mesh);
            obj.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
            obj.TryAddComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
            Destroy(obj.GetComponent<BoxCollider>());
        }
        catch
        {
            Debug.Log("ERROR: COULDNT USE pb_CSG PACKAGE TO HOLLOW OUT TILE");
        }
    }



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
