using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ShowVerticesComponent : MonoBehaviour
{
   private Vector3[] vertices;
    private Vector2[] uvs;
   private int[,] positions;
   private int[,] vert2unique;
   private Color[] colours;
   private bool started = false;
   int nUniqueVertices = 1;
   public bool up = false;

   private void Update()
   {
      if (up)
         vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        if (up)
            uvs = GetComponent<MeshFilter>().sharedMesh.uv;
    }
   private void Start()
   {
      vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        uvs = GetComponent<MeshFilter>().sharedMesh.uv;
        int nVertices = vertices.Length;
      vert2unique = new int[nVertices, 2];
      positions = new int[nVertices, 2];
      positions[0, 0] = 0;
      positions[0, 1] = 1;
      vert2unique[0, 0] = 0;
      vert2unique[0, 1] = 1;
      for (int i = 1; i < nVertices; ++i)
      {
         vert2unique[i, 0] = i;
         vert2unique[i, 1] = 1;
         bool isUnique = true;
         Vector3 currentVertex = vertices[i];
         for (int j = 0; j < nUniqueVertices && isUnique; ++j)
         {
            if (Vector3.Distance(vertices[positions[j, 0]], currentVertex) < 0.01f)
            {
               vert2unique[i, 0] = j;
               positions[j, 1]++;
               vert2unique[i, 1] = positions[j, 1];
               isUnique = false;
            }
         }
         if (isUnique)
         {
            positions[nUniqueVertices, 0] = i;
            positions[nUniqueVertices, 1] = 1;
            nUniqueVertices++;
         }
      }
      GenerateColours(nUniqueVertices);
      started = true;
   }

   private void GenerateColours(int nUniqueVertices)
   {
      colours = new Color[nUniqueVertices];
      float delta = 1.0f / (nUniqueVertices - 1);
      float h = 0;
      for (int i = 0; i < nUniqueVertices; ++i)
      {
         colours[i] = Color.HSVToRGB(h, 1, 1);
         h += delta;
      }
   }

   private void OnDrawGizmos()
   {
      if (!Application.isPlaying || !started)
         return;

      Vector3[] worldPositions = new Vector3[nUniqueVertices];
      for (int i = 0; i < nUniqueVertices; ++i)
      {
         Gizmos.color = colours[i];
         var worldPos = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[positions[i, 0]]);
         worldPositions[i] = worldPos;
         Gizmos.DrawSphere(worldPos, .1f);
      }

      for (int i = 0; i < vertices.Length; ++i)
      {
         var q = vert2unique[i, 1] - 1;
         var worldPos = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]) + Vector3.down / 7f * q + Vector3.down / 9f;

         Handles.Label(worldPos, i.ToString() + uvs[i].ToString());
      }
   }
}
