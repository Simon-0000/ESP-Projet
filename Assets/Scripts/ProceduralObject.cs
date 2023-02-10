using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralObject : MonoBehaviour
{
    [SerializeField]
    GameObject[] objectVariations;
    
    [SerializeField]
    ProceduralObject[] childObjects;

    [Serializable]
    public class vectorRange
    {
        public Vector3 startAt;
        public Vector3 endAt;
        public bool IsRelativeTo;
    }
    
    
    [SerializeField]
    vectorRange[] positions;
    
    [SerializeField] 
    vectorRange[] orientations;
    
    
    // Start is called before the first frame update
    private void Awake()
    {
        for (int i = 0; i < objectVariations.Length; ++i)
            Debug.Assert(objectVariations[i].GetComponent<MeshRenderer>() != null);
        
        Debug.Assert(objectVariations.Length != 0 && 
                         positions.Length != 0 &&
                         orientations.Length != 0 && 
                         positions.Length == orientations.Length);
    }

    public void InstanciateObject(GameObject parent)
    {   

        int index = Random.Range(0,positions.Length);
        Vector3 localPosition = GetRandomVector(positions[index].startAt,positions[index].endAt);
        Vector3 localOrientation = GetRandomVector(orientations[index].startAt,orientations[index].endAt);
        
        if (parent != null)//Si l'objet est instancié selon un object parent
        {
            
            localPosition = new Vector3(parent.GetComponent<MeshRenderer>().bounds.size.x / localPosition.x,
                parent.GetComponent<MeshRenderer>().bounds.size.y / localPosition.y,
                parent.GetComponent<MeshRenderer>().bounds.size.z / localPosition.z);
            localOrientation = new Vector3(parent.transform.localRotation.x / localOrientation.x, 
                              parent.transform.localRotation.y / localOrientation.y, 
                              parent.transform.localRotation.z / localOrientation.z);
        }

        Debug.Log($"localPos: {localPosition}  localOri: {localOrientation}");
    }

    private static Vector3 GetRandomVector(Vector3 startAt, Vector3 endAt) => 
        new Vector3(Random.Range(startAt.x, endAt.x),Random.Range(startAt.y,endAt.y),Random.Range(startAt.z,endAt.z));

}
