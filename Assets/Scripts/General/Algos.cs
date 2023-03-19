//Auteurs: Simon Asmar
//Explication: Cette classe «static» a pour but de contenir divers algorithmes simples qui sont utilisés dans les
//             scripts.

using UnityEngine;
using System.Collections.Generic;


namespace Assets
{
    static class Algos
    {
        public const float OVERLAP_TOLERANCE = 0.002f;

        //Ces fonctions retournent le signe(-1 ou 1) de chaque composante du vecteur src
        public static Vector3 GetVectorSign(Vector3 src)
        {
            return new Vector3(Mathf.Sign(src.x), Mathf.Sign(src.y), Mathf.Sign(src.z));
        }
        public static Vector2 GetVectorSign(Vector2 src)
        {
            return new Vector2(Mathf.Sign(src.x), Mathf.Sign(src.y));
        }

        
        //Ces fonctions retournent un vecteur en valeur absolue
        public static Vector3 GetVectorAbs(Vector3 src)
        {
            return new Vector3(Mathf.Abs(src.x), Mathf.Abs(src.y), Mathf.Abs(src.z));
        }
        public static Vector2 GetVectorAbs(Vector2 src)
        {
            return new Vector2(Mathf.Abs(src.x), Mathf.Abs(src.y));
        }

        public static Vector3 Vector2dTo3dVector(Vector2 vector2)
        {
            return Vector2dTo3dVector(vector2, 0);
        }
        public static Vector3 Vector2dTo3dVector(Vector2 vector2d, float height)
        {
            return new Vector3(vector2d.x, height, vector2d.y); ;
        }


        //Cette fonction permet de prendre une certaine longueur (availableLength) et trouver une coupure aléatoire qui 
        //donnerait deux longueurs plus grandes ou égales à minimumCutLength 
        public static float FindRandomCut(float availableCutLength, float minimumCutLength)
        {
            if(availableCutLength <= 2 * minimumCutLength)
            {
                minimumCutLength = availableCutLength / 2;
            }
            if (Random.Range(0, 2) == 0)
                return availableCutLength / 2 + Random.Range(0, availableCutLength / 2 - minimumCutLength);
            return availableCutLength / 2 - Random.Range(0, availableCutLength / 2 - minimumCutLength);
        }
        
        
        //Filtrer à travers une liste de nœuds pour retourner ceux qui ne possèdent aucune connexion avec un nœud
        //enfant (nœud feuille)
        static public List<Noeud<T>> FilterNoeudsFeuilles<T>(List<Noeud<T>> bsp)
        {
            List<Noeud<T>> noeudsFeuilles = new();
            for (int i = 0; i < bsp.Count; ++i)
            {
                if (bsp[i].noeudsEnfants.Count == 0)
                {
                    noeudsFeuilles.Add(bsp[i]);
                }
            }
            return noeudsFeuilles;
        }

        static public Mesh CenterVertices(Mesh src) 
        {
            Vector3[] vertices = src.vertices;
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] -= src.bounds.center;
            }
            Mesh mesh = src;
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }

        public static Vector3 GetColliderOverlap(GameObject obj, Collider collider)
        {
            Vector3 distance = Algos.GetVectorAbs(obj.transform.position - collider.transform.position);
            Vector3 sizeObj = obj.GetComponent<MeshRenderer>().bounds.size;
            Vector3 sizeCollider = collider.bounds.size;
            return -new Vector3(distance.x - (sizeObj.x + sizeCollider.x) / 2, distance.y - (sizeObj.y + sizeCollider.y) / 2, distance.z - (sizeObj.z + sizeCollider.z) / 2);
        }

        public static bool IsColliderOverlaping(Vector3 overlap, float overlapMin)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (overlap[i] <= overlapMin)
                {
                    return false;
                }
            }

            return true;
        }
        public static Bounds GetRendererBounds(GameObject obj) //Le code de cette fonction a été pris de turnipski: https://www.reddit.com/r/Unity3D/comments/30y46p/getting_the_total_bounds_of_a_prefab_with/
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1, ni = renderers.Length; i < ni; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
                return bounds;
            }
            else
            {
                return new Bounds();
            }
        }
    }
}
