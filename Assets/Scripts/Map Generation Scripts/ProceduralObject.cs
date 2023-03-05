using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets;
public class ProceduralObject : MonoBehaviour
{
    public GameObject[] objectVariations;

    public ProceduralObject[] childProceduralObjects;

    public VectorRange[] positions;

    public VectorRange[] orientations;

    public VectorRange[] offsets;

    public bool XIsConstrained = true;
    public bool YIsConstrained = true;
    public bool ZIsConstrained = true;




    public void Awake()
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

        SetRandomRelativePlacement(obj, parentTransform);
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


    public void SetRandomRelativePlacement(GameObject obj, Transform parentTransform)
    {
        int index = Random.Range(0, positions.Length);
        SetRandomRelativePlacement(obj, parentTransform.gameObject.GetComponent<MeshRenderer>().bounds.size, index);
        return;
    }





    public void SetRandomRelativePlacement(GameObject obj, Vector3 parentDimensions) =>
        SetRandomRelativePlacement(obj, parentDimensions, Random.Range(0, positions.Length));


    public void SetRandomRelativePlacement(GameObject obj, Vector3 parentDimensions,int placementIndex)
    {

        SetRandomRelativePlacement(obj, parentDimensions,(positions[placementIndex],orientations[placementIndex], offsets[placementIndex]));
    }
    public void SetRandomRelativePlacement(GameObject obj, Vector3 parentDimensions, (VectorRange position, VectorRange orientation, VectorRange offset) placement)
    {
        Vector3 relativePosition = placement.position.GetRandomVector();

        Vector3 relativeOrientation = placement.orientation.GetRandomVector();

        TryPositionObject(obj,parentDimensions, new Vector3(parentDimensions.x * relativePosition.x,
            parentDimensions.y * relativePosition.y,
            parentDimensions.z * relativePosition.z));
        //Check if relativePosition is out of bounds depending on isConstrained--------------------------------------------------------------------------------
        Vector3 offset =  placement.offset.GetRandomVector();
        offset.Scale(Algos.GetVectorSign(obj.transform.localPosition));
        obj.transform.localPosition += offset;
        //Check if relativePosition is out of bounds depending on isConstrained--------------------------------------------------------------------------------

        obj.transform.localRotation = Quaternion.Euler(relativeOrientation.x, relativeOrientation.y, relativeOrientation.z);
    }
    private void TryPositionObject(GameObject obj, Vector3 parentDimensions, Vector3 relativePosition) =>
            TryPositionObject(obj, parentDimensions, relativePosition, XIsConstrained, YIsConstrained, ZIsConstrained);


    private static void TryPositionObject(GameObject obj, Vector3 parentDimensions, Vector3 relativePosition, bool xIsConstrained, bool yIsConstrained, bool zIsConstrained)
    {
        Vector3 objDimensions = obj.gameObject.GetComponent<MeshRenderer>().bounds.size;
        Vector3 cornerPosition = objDimensions / 2 + Algos.GetVectorAbs(relativePosition);
        Vector3 relativePositionSign = Algos.GetVectorSign(relativePosition);

        if (cornerPosition.x > parentDimensions.x/2 && xIsConstrained)
            relativePosition.x = relativePositionSign.x * (parentDimensions.x - objDimensions.x)/2;

        if (cornerPosition.y > parentDimensions.y/2 && yIsConstrained)
            relativePosition.y = relativePositionSign.y * (parentDimensions.y - objDimensions.y) / 2;

        if (cornerPosition.z > parentDimensions.z/2 && zIsConstrained)
            relativePosition.z = relativePositionSign.z * (parentDimensions.z - objDimensions.z) / 2;

        obj.transform.localPosition = relativePosition;
    }
}
