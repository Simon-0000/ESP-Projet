//Auteurs: Simon Asmar
//Explication: Ce script hérite de «ProceduralObject» qui est de type «Procedural». ProceduralTiledCubeObject permet d'étirer un cube 
//selon une grandeur relative. On ne l'étire pas avec un «scale» et le «Material», mais plutôt avec les «vertices» et les «uvs»,
//pour ne pas qu'un autre objet procédural qui dépend de cet objet soit étiré. On a aussi la possibilité de contournée les objets qui se
//trouvent dans le chemin de la tuile afin de créer des ouvertures dans le maillage ex: mur qui contourne une porte pour créer une ouverture

using Assets;
using Parabox.CSG;//Le «package» utilisé pour soustraire le maillage d'un objet à celle de la tuile
using System.Linq;
using UnityEngine;


public class ProceduralTiledCubeObject : ProceduralObject
{
    [SerializeField] 
    VectorRange[] scales;// Les valeurs de «scales» sont utilisées comme des uvs où 0 représente une grandeur de 0
                         // et 1 représente la grandeur du parent

    [SerializeField]
    VectorRange[] scaleOffsets; //Les décalages permettent de donner une grandeur qui vaudrait, par exemple,
                                //toujours entre 1 et 2 unités. Une valeur négative va diminuer la grandeur,
                                //une valeur positive va l'agrandir.
    
    [SerializeField]
    bool wrapsAround = true;//Permet de contourner les objets qui se trouvent dans le chemin
   
                            
    public void Awake() 
    {
        //Verifier les conditions de «ProceduralObject»
        base.Awake();

        //Il doit y avoir autant de position que de «scales» et de «scaleOffsets»
        Debug.Assert(positions.Length == scales.Length && 
                     positions.Length == scaleOffsets.Length);
                                                                            
        //Chaque variation de l'objet à instancier doit être un cube
        for(int i = 0; i < objectVariations.Length; ++i) 
            Debug.Assert(objectVariations[i].GetComponent<MeshFilter>().sharedMesh.name == "Cube");

        //Constante qui permet de définir la précision du CSG
        CSG.epsilon = GameConstants.OVERLAP_TOLERANCE;
    }

    public override GameObject InstanciateProcedural(Transform parent)//Implémentation de la classe abstraite «Procedural»
    {
        GameObject obj = Instantiate(objectVariations[Random.Range(0, objectVariations.Length)], parent);

        BoundsManager parentBoundsManager = parent.gameObject.GetComponent<BoundsManager>();
        if (parentBoundsManager != null)
            InstantiateProceduralTiledObject(ref obj, parentBoundsManager.objectBoundsLocal.size);
        else
            InstantiateProceduralTiledObject(ref obj, Algos.GetRendererBounds(parent.gameObject).size);
        return obj;
    }
    public GameObject InstantiateProceduralTiledObject(ref GameObject obj, Vector3 parentDimensions)
    {
        return InstantiateProceduralTiledObject(ref obj, parentDimensions, Enumerable.Range(0, positions.Length).ToArray());
    }
    public GameObject InstantiateProceduralTiledObject(Transform parent, Vector3 parentDimensions, int variationIndex, int placementIndex)//Seulement utilisé par ProceduralRoom, pour faire les murs, sols, etc.
    {
        GameObject tuileObject = Instantiate(objectVariations[variationIndex], parent);
        Vector3 tileSize = Vector3.Scale(scales[placementIndex].GetRandomVector(), parentDimensions) + scaleOffsets[placementIndex].GetRandomVector();
        TileUvs(tuileObject, tileSize);
        StretchVertices(tuileObject, tileSize);
        TrySetRandomRelativePlacement(ref tuileObject, parentDimensions, new int[] { placementIndex });
        if (wrapsAround == true && tuileObject != null)
            WrapMesh(tuileObject, tileSize);

        return tuileObject;
    }
    public GameObject InstantiateProceduralTiledObject(ref GameObject obj, Vector3 parentDimensions, int[] placementIndexes)
    {
        int placementIndex = placementIndexes[Random.Range(0, placementIndexes.Length)];
        Vector3 tileSize = Vector3.Scale(scales[placementIndex].GetRandomVector(), parentDimensions) + scaleOffsets[placementIndex].GetRandomVector();
        TileUvs(obj, tileSize);
        StretchVertices(obj, tileSize);
        TrySetRandomRelativePlacement(ref obj, parentDimensions, new int[1]{ placementIndex});
        if (wrapsAround == true && obj != null)
            WrapMesh(obj, tileSize);

        return obj;
    }

    //WrapMesh permet de trouver tous les objets valides qui rentrent en collision avec «obj» afin de soustraire
    //le maillage des objets trouvés au maillage de «obj»
    private void WrapMesh(GameObject obj, Vector3 size) 
    {
        //On cherche tous les objets qui pourraient être soustraits du maillage de la tuile
        Collider[] colliders = Physics.OverlapBox(obj.transform.position, size / 2,obj.transform.rotation);
        for (int i = 0; i < colliders.Length; ++i)
        {
            //On s'assure de ne pas essayer de soustraire le maillage de la tuile par son propre maillage
            if (obj != colliders[i].gameObject)
            {
                Vector3 roomOverlap = Algos.GetColliderOverlap(obj, colliders[i]);
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
            obj.GetComponent<MeshFilter>().sharedMesh = Algos.CenterVertices(result.mesh);//Le maillage retourner par le «package» n'est pas centré
            obj.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();

            //Puisque le maillage a été modifié, on enlève le box collider et on ajoute/modifie le meshCollider
            Destroy(obj.GetComponent<BoxCollider>());
            obj.TryAddComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
        }
        catch
        {
            Debug.Log("ERROR: COULDNT USE pb_CSG PACKAGE TO HOLLOW OUT TILE");
        }
    }



    private static int[] cubeAxisA = { 0, 0, 0 , 0, 2, 2 };//Représente les axes de chaque face/plan d'un cube Unity
    private static int[] cubeAxisB = { 1, 2, 1, 2, 1, 1 };
    private static void TileUvs(GameObject obj, Vector3 globalStretchSize)
    {
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        Vector3 cubeSize = meshRenderer.bounds.size;
        Vector3 relativeStretchSize = new Vector3(globalStretchSize.x / cubeSize.x, globalStretchSize.y / cubeSize.y, globalStretchSize.z / cubeSize.z);
        Vector2[] uvs = obj.GetComponent<MeshFilter>().mesh.uv;

        //Les «vertices» utilisées pour les «uvs» de chaque face ne sont pas en ordre croissant dans un cube de Unity,
        //donc on utilise le array cubeAxisA et cubeAxisB pour obtenir l'ordre des faces
        int[] uvIndex = obj.GetComponent<MeshFilter>().mesh.triangles.Distinct().ToArray();

        for (int i = 0; i < 6; ++i)//pour chaque face
        {
            for(int j = 0; j < 4; ++j)//pour chaque «vertice»
            {
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

        //On donne les nouveaux «vertices» au «mesh» et on recalcule les «bounds»
        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        //On recalcule le «BoxCollider» du cube
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.size = globalStretchSize;
        }    
    }
}
