using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public bool IsRelativeTo;
        
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




    //public virtual void InstanciateProceduralObject()
    //{
    //    GameObject obj = Instantiate(objectVariations[Random.Range(0, objectVariations.Length)]);
    //    SetPositionning(obj);
    //}

    public virtual void InstanciateProceduralObject(GameObject parent)
    {
        GameObject obj = Instantiate(objectVariations[Random.Range(0, objectVariations.Length)], parent.transform);
        SetRandomRelativePositioning(obj, parent);
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
    protected void SetRandomRelativePositioning(GameObject obj, GameObject parent)
    {
        int index = Random.Range(0, positions.Length);

        obj.transform.position = GetRandomVector(positions[index].startAt, positions[index].endAt);
        obj.transform.Rotate(GetRandomVector(orientations[index].startAt, orientations[index].endAt),Space.Self);

        obj.transform.localPosition = new Vector3(parent.GetComponent<MeshRenderer>().bounds.size.x / obj.transform.position.x,
                     parent.GetComponent<MeshRenderer>().bounds.size.y / obj.transform.position.y,
                     parent.GetComponent<MeshRenderer>().bounds.size.z / obj.transform.position.z);
       obj.transform.localPosition = new Vector3(parent.transform.localRotation.x / obj.transform.rotation.x,
                        parent.transform.localRotation.y / obj.transform.rotation.y,
                        parent.transform.localRotation.z / obj.transform.rotation.z);
    }




    static Vector3 GetRandomVector(Vector3 startAt, Vector3 endAt) => 
        new Vector3(Random.Range(startAt.x, endAt.x),Random.Range(startAt.y,endAt.y),Random.Range(startAt.z,endAt.z));

}
