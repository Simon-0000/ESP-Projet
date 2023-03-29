//Auteurs: Simon Asmar
//Explication: Ce script a pour but de définir les positions, les orientations et les décalages d'un objet procédural.
//Le positionnement est relatif à un parent, donc on peut facilement créer des pièces qui ont de l'allure. On peut aussi
//limité le X,Y,Z pour que l'objet ne dépasse pas les limites du parent.
//ex d'implémentations: portrait relatif à un mur, un lit relatif au sol,etc.

using System;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets;
using UnityEditor;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ProceduralObject : Procedural
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
    [SerializeField]
    bool repositionAtCollision = true;
    //X,Y,Z «IsConstrained» permet de limiter la position d'un objet pour qu'il ne dépasse pas les limites de son parent
    //ex: on ne veut pas qu'une porte sorte physiquement du mur(«Constrained» en x,y), mais on voudrait qu'une vase puisse
    //être plus haute que la table (pas «Constrained»)
    public bool XIsConstrained = true;
    public bool YIsConstrained = true;
    public bool ZIsConstrained = true;

    public void Awake()
    {
        for (int i = 0; i < objectVariations.Length; ++i)//On s'assure que chaque GameObject possède une grandeur
            Debug.Assert(Algos.GetRendererBounds(objectVariations[i]) != default);

        //Il doit y avoir au moins 1 objet et 1 positionnement pour l'objet
        Debug.Assert(objectVariations.Length != 0 &&
                     positions.Length != 0 &&
                     orientations.Length != 0 &&
                     offsets.Length != 0);
        
        //Il doit y avoir autant de position que d'orientation et de décalage
        Debug.Assert(positions.Length == orientations.Length&& positions.Length == offsets.Length);
    }

    public override GameObject InstanciateProcedural(Transform parentTransform)//Instancier l'objet relatif à un parent
    {
        GameObject obj =  Instantiate(objectVariations[Random.Range(0, objectVariations.Length)], parentTransform);

        //Si on a accès à l'information du "bounds" donnée par le parent, on l'utilise, sinon on obtient cette
        //information manuellement
        BoundsManager parentBoundsManager = parentTransform.gameObject.GetComponent<BoundsManager>();
        if (parentBoundsManager != null)
             SetRandomRelativePlacement(ref obj, parentBoundsManager.objectBounds.size);
        else
            SetRandomRelativePlacement(ref obj, Algos.GetRendererBounds(parentTransform.gameObject).size);
        return obj;
    }

    public void SetRandomRelativePlacement(ref GameObject obj, Vector3 parentDimensions) =>
        SetRandomRelativePlacement(ref obj, parentDimensions, Random.Range(0, positions.Length));

    public void SetRandomRelativePlacement(ref GameObject obj, Vector3 parentDimensions,int placementIndex)=>
        SetRandomRelativePlacement(ref obj, parentDimensions,(positions[placementIndex],orientations[placementIndex], offsets[placementIndex]));

    public void SetRandomRelativePlacement(ref GameObject obj, Vector3 parentDimensions, (VectorRange position, VectorRange orientation, VectorRange offset) placement)
    {
        bool reposition;
        int iterationAttemps = 0;
        obj.TryAddComponent<BoundsManager>().Awake();
        Vector3 objDimensions;

        //les lignes de codes avec Physics.autoSimulation et Physics.Simulate ont été prises par Pablo Lanza et derHugo
        //https://stackoverflow.com/questions/69055600/bad-usage-of-physics-overlapbox
        Physics.autoSimulation = false;
        Physics.Simulate(Time.deltaTime);
        do
        {
            Vector3 relativePosition = placement.position.GetRandomVector();
            Vector3 relativeOrientation = placement.orientation.GetRandomVector();
            Vector3 offset = placement.offset.GetRandomVector();

            //On doit prendre les mesures de l'objet après la rotation (pour le bougé adéquatement),
            //mais on ne peut pas le tourner pour le moment, sinon un mouvement par rapport aux axes x,y,z (local) ne sera pas valide
            obj.transform.localRotation = Quaternion.Euler(relativeOrientation.x, relativeOrientation.y, relativeOrientation.z);//-------
            obj.GetComponent<BoundsManager>().Awake();//----------
            objDimensions = obj.GetComponent<BoundsManager>().objectBounds.size;
            obj.transform.localRotation = Quaternion.identity;//-------

            //Placer l'objet relativement à son parent
            obj.transform.localPosition = TryConstrainRelativePosition(objDimensions, parentDimensions, new Vector3(parentDimensions.x * relativePosition.x,
                parentDimensions.y * relativePosition.y,
                parentDimensions.z * relativePosition.z));
            if (obj.transform.localPosition.x > parentDimensions.x || obj.transform.localPosition.y > parentDimensions.y || obj.transform.localPosition.z > parentDimensions.z)
                Debug.Log("Must have been the wind: "+ obj.name + obj.transform.localPosition + ", " + parentDimensions);
                
            
            //Décaler l'objet
                offset.Scale(Algos.GetVectorSign(obj.transform.localPosition));
            obj.transform.localPosition += offset;

            //Prendre les dimensions sans la rotation pour que les enfants qui dépendent de cet objet soient bien positionnés 
            obj.GetComponent<BoundsManager>().Awake();

            //Tourner l'objet
            obj.transform.localRotation = Quaternion.Euler(relativeOrientation.x, relativeOrientation.y, relativeOrientation.z);


            //Repositionner l'objet s'il rentre en collision avec d'autres objets
            reposition = false;
            if (repositionAtCollision)
            {
                Collider[] colliders = Physics.OverlapBox(obj.transform.position, (objDimensions*0.5f ),obj.transform.rotation);
                for (int i = 0; i < colliders.Length; ++i)
                {
                    if (Algos.IsColliderOverlaping(Algos.GetColliderOverlap(obj,colliders[i])))
                    {
                        Transform colliderProceduralParent = Algos.FindFirstParentInstance(colliders[i].gameObject, p => p.gameObject.GetComponent<BoundsManager>() != null);
                        if (colliderProceduralParent != null && colliderProceduralParent.gameObject != obj && colliderProceduralParent.gameObject != obj.transform.parent.gameObject)
                        {
                            Debug.Log("Collider: " + obj.name + " With: " + colliders[i].gameObject.name + "OBJ Size: " + objDimensions * 0.5f + "Overlap = " + Algos.GetColliderOverlap(obj, colliders[i]));
                            reposition = true;
                            break;
                        }    
                    }
                }

            }
            ++iterationAttemps;
        } while (reposition == true && iterationAttemps <= GameConstants.MAX_ITERATIONS);
        if (reposition == true)// condition qui est vraie si on n'a pas pu repositionner l'objet après 100 essais
        {
            Destroy(obj);
            obj = null;
        }
        Physics.autoSimulation = true;

    }

    //TryConstrainRelativePosition retourne une position relative qui ne dépasse pas les limites XYZ de son parent si
    // xyz «IsConstrained» = true 
    private Vector3 TryConstrainRelativePosition(Vector3 objDimensions, Vector3 parentDimensions, Vector3 relativePosition) =>
            TryConstrainRelativePosition(objDimensions, parentDimensions, relativePosition, XIsConstrained, YIsConstrained, ZIsConstrained);

    private static Vector3 TryConstrainRelativePosition(Vector3 objDimensions, Vector3 parentDimensions, Vector3 relativePosition, bool xIsConstrained, bool yIsConstrained, bool zIsConstrained)
    {
        Vector3 cornerPosition = objDimensions / 2 + Algos.GetVectorAbs(relativePosition);
        Vector3 relativePositionSign = Algos.GetVectorSign(relativePosition);

        if (cornerPosition.x > parentDimensions.x/2 && xIsConstrained)
            relativePosition.x = relativePositionSign.x * ((parentDimensions.x - objDimensions.x)/2) ;

        if (cornerPosition.y > parentDimensions.y/2 && yIsConstrained)
            relativePosition.y = relativePositionSign.y * (parentDimensions.y - objDimensions.y) / 2;

        if (cornerPosition.z > parentDimensions.z/2 && zIsConstrained)
            relativePosition.z = relativePositionSign.z * (parentDimensions.z - objDimensions.z) / 2;
        
        return relativePosition;
    }
}
