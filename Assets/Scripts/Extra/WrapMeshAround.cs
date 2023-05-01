using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;
using Parabox.CSG;

public class WrapMeshAround : MonoBehaviour
{
    //WrapMesh permet de trouver tous les objets valides qui rentre en collision avec obj afin de modifier
    //le mesh de obj aux objets trouvés
    List<GameObject> meshToWrap;
    public void ReloadScript()
    {
        meshToWrap = new();
        CSG.epsilon = GameConstants.OVERLAP_TOLERANCE;
    }
    public void AddObjToWrapMeshList(GameObject obj) =>
        meshToWrap.Add(obj);
    public void UpdateWrapMeshList()
    {
        Debug.Log(meshToWrap.Count);
        meshToWrap.ForEach(obj => WrapMesh(obj));
    }

    private void WrapMesh(GameObject obj)
    {
        if (obj == null)
            Debug.Log("Problem here");
        Vector3 size = obj.TryAddComponent<BoundsManager>().objectBoundsLocal.size;
        Debug.Log("WRAPPING" + size);

        Collider[] colliders = Physics.OverlapBox(obj.transform.position, size / 2, obj.transform.rotation);
        for (int i = 0; i < colliders.Length; ++i)
        {
            Debug.Log("FRFR");

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

}
