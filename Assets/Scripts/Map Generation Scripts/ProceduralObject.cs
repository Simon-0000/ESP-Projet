//Auteurs: Simon Asmar
//Explication: Ce script a pour but de définir les positions, les orientations et les décalages d'un objet procédural.
//Le positionnement est relatif à un parent, donc on peut facilement créer des pièces qui ont de l'allure. On peut aussi
//limité le X,Y,Z pour que l'objet ne dépasse pas les limites du parent.
//ex d'implémentations: portrait relatif à un mur, un lit relatif au sol,etc.

using UnityEngine;
using Random = UnityEngine.Random;
using Assets;
public class ProceduralObject : MonoBehaviour
{
    
    //Pour «positions», «orientations» et «offsets», il doit y avoir autant d'éléments dans chaque array,
    //puisque la position 1 correspond à l'orientation 1 et le décalage 1 (ces trois composantes forme le
    //positionnement de l'objet)
    
    public GameObject[] objectVariations;//Les différentes variations de l'objet
    
    public VectorRange[] positions;// Les valeurs de positions sont utilisées comme des uvs où 0 se
                                   //trouve au centre de son parent, 0.5 à une extrémité et -0.5 à l'autre extrémité 

    public VectorRange[] orientations;//L'orientation représente des angles (en degrés) relatifs au parent

    public VectorRange[] offsets;//Les décalages permettent de positionner des objets qui sont par exemple,
                                 //toujours décalés du centre de 1 unité. Une valeur négative va le décaler
                                 //vers le centre de son parent, une valeur positive fera l'inverse
                                 
    //X,Y,Z «IsConstrained» permet de limiter la position d'un objet pour qu'il ne dépasse pas les limites de son parent
    //ex: on ne veut pas qu'une porte sorte physiquement du mur(«Constrained» en x,y), mais on voudrait qu'une vase puisse
    //être plus haute que la table (pas «Constrained»)
    public bool XIsConstrained = true;
    public bool YIsConstrained = true;
    public bool ZIsConstrained = true;

    public void Awake()
    {
        for (int i = 0; i < objectVariations.Length; ++i)//On s'assure que chaque GameObject possède un «MeshRenderer»
            Debug.Assert(objectVariations[i].GetComponent<MeshRenderer>() != null);

        //Il doit y avoir au moins 1 objet et 1 positionnement pour l'objet
        Debug.Assert(objectVariations.Length != 0 &&
                     positions.Length != 0 &&
                     orientations.Length != 0 &&
                     offsets.Length != 0);
        
        //Il doit y avoir autant de position que d'orientation et de décalage
        Debug.Assert(positions.Length == orientations.Length&& positions.Length == offsets.Length);
    }

    public virtual void InstanciateProceduralObject(Transform parentTransform)//Instancier l'objet relatif à un parent
    {
        GameObject obj = Instantiate(objectVariations[Random.Range(0, objectVariations.Length)], parentTransform);
        SetRandomRelativePlacement(obj, parentTransform.gameObject.GetComponent<MeshRenderer>().bounds.size);
    }

    public void SetRandomRelativePlacement(GameObject obj, Vector3 parentDimensions) =>
        SetRandomRelativePlacement(obj, parentDimensions, Random.Range(0, positions.Length));
    
    public void SetRandomRelativePlacement(GameObject obj, Vector3 parentDimensions,int placementIndex)=>
        SetRandomRelativePlacement(obj, parentDimensions,(positions[placementIndex],orientations[placementIndex], offsets[placementIndex]));
    
    public void SetRandomRelativePlacement(GameObject obj, Vector3 parentDimensions, (VectorRange position, VectorRange orientation, VectorRange offset) placement)
    {
        Vector3 relativePosition = placement.position.GetRandomVector();
        Vector3 relativeOrientation = placement.orientation.GetRandomVector();
        Vector3 offset =  placement.offset.GetRandomVector();
        
        //Placer l'objet relativement à son parent
        PositionObject(obj,parentDimensions, new Vector3(parentDimensions.x * relativePosition.x,
            parentDimensions.y * relativePosition.y,
            parentDimensions.z * relativePosition.z));
        
        //Décaler l'objet
        offset.Scale(Algos.GetVectorSign(obj.transform.localPosition));
        obj.transform.localPosition += offset;

        //Tourner l'objet
        obj.transform.localRotation = Quaternion.Euler(relativeOrientation.x, relativeOrientation.y, relativeOrientation.z);
        
        //Repositionner l'objet s'il rentre en collision avec d'autres objets (pas implémenté)

    }
    
    
    //Cet fonction positionne l'objet relatif au parent
    private void PositionObject(GameObject obj, Vector3 parentDimensions, Vector3 relativePosition) =>
            PositionObject(obj, parentDimensions, relativePosition, XIsConstrained, YIsConstrained, ZIsConstrained);
    private static void PositionObject(GameObject obj, Vector3 parentDimensions, Vector3 relativePosition, bool xIsConstrained, bool yIsConstrained, bool zIsConstrained)
    {
        //On essaye de limiter la position de l'objet
        relativePosition = TryConstrainRelativePosition(obj, parentDimensions, relativePosition, xIsConstrained, yIsConstrained, zIsConstrained);
        
        //On positionne l'objet
        obj.transform.localPosition = relativePosition;
    }

    
    //Cette fonction retourne une position relative qui ne dépasse pas les limites XYZ de son parent si
    // XYZ «IsConstrained» = true 
    private static Vector3 TryConstrainRelativePosition(GameObject obj, Vector3 parentDimensions, Vector3 relativePosition, bool xIsConstrained, bool yIsConstrained, bool zIsConstrained)
    {
        Vector3 objDimensions = obj.GetComponent<MeshRenderer>().bounds.size;
        Vector3 cornerPosition = objDimensions / 2 + Algos.GetVectorAbs(relativePosition);
        Vector3 relativePositionSign = Algos.GetVectorSign(relativePosition);

        if (cornerPosition.x > parentDimensions.x/2 && xIsConstrained)
            relativePosition.x = relativePositionSign.x * (parentDimensions.x - objDimensions.x)/2;

        if (cornerPosition.y > parentDimensions.y/2 && yIsConstrained)
            relativePosition.y = relativePositionSign.y * (parentDimensions.y - objDimensions.y) / 2;

        if (cornerPosition.z > parentDimensions.z/2 && zIsConstrained)
            relativePosition.z = relativePositionSign.z * (parentDimensions.z - objDimensions.z) / 2;
        
        return relativePosition;
    }
}
