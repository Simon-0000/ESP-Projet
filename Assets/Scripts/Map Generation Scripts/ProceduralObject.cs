using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets;
public class ProceduralObject : MonoBehaviour
{
    public GameObject[] objectVariations;

    public ProceduralObject[] childProceduralObjects;

    public vectorRange[] positions;

    public vectorRange[] orientations;


   [Serializable]
    public class vectorRange
    {
        public Vector3 startAt;
        public Vector3 endAt;
        public int IsRelativeTo = 0;
        public vectorRange()
        {
            startAt = Vector3.zero;
            endAt = Vector3.zero;
            IsRelativeTo = 0;
        }

        public vectorRange(Vector3 startVector, Vector3 endVector, int relativePosition)
        {
            startAt = startVector;
            endAt = endVector;
            IsRelativeTo = relativePosition;
        }
    }
    
    

    private void Awake()
    {
        for (int i = 0; i < objectVariations.Length; ++i)
            Debug.Assert(objectVariations[i].GetComponent<MeshRenderer>() != null);

        Debug.Assert(objectVariations.Length != 0 &&
                     positions.Length != 0 &&
                     orientations.Length != 0 && 
                     positions.Length == orientations.Length);
    }
    

    public virtual void InstanciateProceduralObject(Transform parentTransform)
    {
        GameObject obj = Instantiate(objectVariations[Random.Range(0, objectVariations.Length)], parentTransform);
        SetRandomRelativePositioning(obj, parentTransform);
    }




    //protected void SetPositionning(GameObject obj)
    //{
    //    int index = Random.Range(0, positions.Length);
    //    SetRandomPositioning(obj, index);
    //}
    //protected void SetPositionning(GameObject obj, GameObject parent)
    //{
    //    int index = Random.Range(0, positions.Length);
    //    SetRandomRelativePositioning(obj,index, parent);
    //}




    //void SetRandomPositioning(GameObject obj, int index)
    //{
    //   obj.transform.position = GetRandomVector(positions[index].startAt, positions[index].endAt);
    //   obj.transform.Rotate(GetRandomVector(orientations[index].startAt, orientations[index].endAt),Space.World);
    //}


    protected void SetRandomRelativePositioning(GameObject obj, Transform parentTransform)
    {
        int index = Random.Range(0, positions.Length);
        SetRandomRelativePositioning(obj, parentTransform.gameObject.GetComponent<MeshRenderer>().bounds.size, index);
        return;
    }





    protected void SetRandomRelativePositioning(GameObject obj, Vector3 parentDimensions) =>
        SetRandomRelativePositioning(obj, parentDimensions, Random.Range(0, positions.Length));


    protected void SetRandomRelativePositioning(GameObject obj, Vector3 parentDimensions,int placementIndex)
    {

        SetRandomRelativePositioning(obj, parentDimensions,(positions[placementIndex],orientations[placementIndex]));
    }
    protected void SetRandomRelativePositioning(GameObject obj, Vector3 parentDimensions, (vectorRange position, vectorRange orientation) placement)
    {

        Vector3 relativePosition = Algos.GetRandomVector(placement.position.startAt, placement.position.endAt);
        Vector3 relativeOrientation = Algos.GetRandomVector(placement.orientation.startAt, placement.orientation.endAt);

        obj.transform.localPosition = new Vector3(parentDimensions.x * relativePosition.x,
            parentDimensions.y * relativePosition.y,
            parentDimensions.z * relativePosition.z);
         obj.transform.localRotation = Quaternion.Euler(relativeOrientation.x, relativeOrientation.y, relativeOrientation.z);
    }



    static Transform GetParentFromIndex(Transform childTransform, int index) 
    {
        Transform subParent = childTransform.parent;
        if(subParent.parent != null)
        {
            for (int i = 0; i < index; ++i)
            {
                subParent = subParent.parent;
                if (subParent == null)
                    break;
            }
        }
        return subParent;
    }
}
