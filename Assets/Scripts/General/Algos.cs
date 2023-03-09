//Auteurs: Simon Asmar
//Explication: Cette classe «static» a pour but de contenir divers algorithmes simples qui sont utilisés dans les
//             scripts.

using UnityEngine;
using System.Collections.Generic;


namespace Assets
{
    static class Algos
    {
        
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
        
        //Cette fonction permet de prendre une certaine longueur (availableLength) et trouver une coupure aléatoire qui 
        //donnerait deux longueurs plus grandes ou égales à minimumCutLength 
        public static float FindRandomCut(float availableCutLength, float minimumCutLength)
        {
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
                if (bsp[i].NoeudsEnfants.Count == 0)
                {
                    noeudsFeuilles.Add(bsp[i]);
                }
            }
            return noeudsFeuilles;
        }
    }
}
