using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Assets
{
   class GénérateurPièces
   {
      float grandeurMin,grandeurMax, grandeur;
      RectangleInfo2d dimensionsMap;

      public GénérateurPièces(RectangleInfo2d dimensionMap, float grandeurMin,float grandeurMax)
      {
         this.grandeurMin = grandeurMin;
         this.grandeurMax = grandeurMax;
         this.dimensionsMap = dimensionMap;
      }
      public List<Noeud<RectangleInfo2d>> GénérerPièces() 
      {
         List<Noeud<RectangleInfo2d>> noeudsFeuille = BinarySpacePartitioning.FilterNoeudsFeuille(BinarySpacePartitioning.GénérerBSP(new Noeud<RectangleInfo2d>(null, dimensionsMap), TryDiviserPièce));
         CréerLiaisonPhysiquePièces(noeudsFeuille);
         
         Dictionary<Noeud<RectangleInfo2d>, bool> dictionaryVisitedNodes = noeudsFeuille.ToDictionary(x=>x, val=>false);
         //DFS ICI, utilise le dictionaryVisitedNodes qui contient tous les noeuds(représente des pièces avec leur connections) 
         //Le dictionnaire retourne faux par défaut(non visité) et tu peut le changer pour qu'il retoure true (visité) pour faire DFS
         
         return noeudsFeuille;
      }

      (Noeud<RectangleInfo2d>, Noeud<RectangleInfo2d>) TryDiviserPièce(Noeud<RectangleInfo2d> noeudParent)//Divise une pièce parente en deux pièce pour être mis dans le Queue des pièce à diviser
      {
         Vector2 grandeurEnfantA, grandeurEnfantB;

         switch (TrouverDirectionCoupure(noeudParent.Valeur))
         {
            case Direction.Horizontale:
               grandeurEnfantA = new(noeudParent.Valeur.grandeur.x, TrouverCoupureAléatoire(noeudParent.Valeur.grandeur.y, grandeur));
               grandeurEnfantB = new(grandeurEnfantA.x, noeudParent.Valeur.grandeur.y - grandeurEnfantA.y);

               break;
            case Direction.Verticale:
               grandeurEnfantA = new(TrouverCoupureAléatoire(noeudParent.Valeur.grandeur.x, grandeur), noeudParent.Valeur.grandeur.y);
               grandeurEnfantB = new(noeudParent.Valeur.grandeur.x - grandeurEnfantA.x, grandeurEnfantA.y);
               break;
            default:
               return (null, null);
         }

         return (new Noeud<RectangleInfo2d>(noeudParent, new RectangleInfo2d(grandeurEnfantA, noeudParent.Valeur.CoordonnéesBasGauche + grandeurEnfantA/2)),
               new Noeud<RectangleInfo2d>(noeudParent, new RectangleInfo2d(grandeurEnfantB,  noeudParent.Valeur.CoordonnéesHautDroit - grandeurEnfantB/2)));
      }

      void CréerLiaisonPhysiquePièces(List<Noeud<RectangleInfo2d>> piècesNonLiées)
      {
         for (int i = 0; i < piècesNonLiées.Count; ++i)
         {
            for (int j = i + 1; j < piècesNonLiées.Count; ++j)
            {
               if (AreRoomsConnected(piècesNonLiées[i].Valeur,piècesNonLiées[j].Valeur))
               {
                  piècesNonLiées[i].AjouterNoeud(piècesNonLiées[j]);
                  piècesNonLiées[j].AjouterNoeud(piècesNonLiées[i]);
               }
            }
         }
      }


      static float TrouverCoupureAléatoire(float longueurParent, float longueurMinEnfant)
      {
         if (Random.Range(0, 2) == 0)
            return longueurParent / 2 + Random.Range(0, longueurParent / 2 - longueurMinEnfant);
         return longueurParent / 2 - Random.Range(0, longueurParent / 2 - longueurMinEnfant);
      }
      Direction TrouverDirectionCoupure(RectangleInfo2d dimensions)
      {

         grandeur = Random.Range(grandeurMin, grandeurMax);
         bool coupureVerticale = dimensions.grandeur.x >= grandeur  * 2;
         bool coupureHorizontale = dimensions.grandeur.y >= grandeur * 2;

         if (coupureVerticale && coupureHorizontale)
            return Random.Range(0, 2) == 0 ? Direction.Verticale : Direction.Horizontale;
         else if (coupureVerticale)
            return Direction.Verticale;
         else if (coupureHorizontale)
            return Direction.Horizontale;
         return Direction.Aucune;
      }

      static bool AreRoomsConnected(RectangleInfo2d PièceA, RectangleInfo2d PièceB)
      {
         Vector2 distance = PièceA.coordonnées - PièceB.coordonnées;
         distance = distance.Abs();
         Vector2 chevauchementDistance = new(distance.x - (PièceA.grandeur.x + PièceB.grandeur.x)/2,distance.y - (PièceA.grandeur.y + PièceB.grandeur.y)/2);
         return chevauchementDistance.x <= 0 && chevauchementDistance.y <= 0;
      }
   }
}
