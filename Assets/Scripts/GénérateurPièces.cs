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
      Vector2 grandeurPièce;
      dimensionsPièce dimensionsMap;

      public GénérateurPièces(dimensionsPièce dimensionMap, Vector2 grandeurPièce)
      {
         this.grandeurPièce = grandeurPièce;
         this.dimensionsMap = dimensionMap;
      }
      public List<Noeud<dimensionsPièce>> GénérerPièces() 
      {
         List<Noeud<dimensionsPièce>> noeudsFeuille = BinarySpacePartitioning.FilterNoeudsFeuille(BinarySpacePartitioning.GénérerBSP(new Noeud<dimensionsPièce>(null, dimensionsMap), TryDiviserPièce));
         CréerLiaisonPhysiquePièces(noeudsFeuille);
         //DFS HERE-------------------------------------------------------------------------------------
         return noeudsFeuille;
      }

      (Noeud<dimensionsPièce>, Noeud<dimensionsPièce>) TryDiviserPièce(Noeud<dimensionsPièce> noeudParent)//Divise une pièce parente en deux pièce pour être mis dans le Queue des pièce à diviser
      {
         Vector2 grandeurEnfantA, grandeurEnfantB;

         switch (TrouverDirectionCoupure(noeudParent.Valeur))
         {
            case Direction.Horizontale:
               grandeurEnfantA = new(noeudParent.Valeur.grandeur.x, TrouverCoupureAléatoire(noeudParent.Valeur.grandeur.y, grandeurPièce.y));
               grandeurEnfantB = new(grandeurEnfantA.x, noeudParent.Valeur.grandeur.y - grandeurEnfantA.y);

               break;
            case Direction.Verticale:
               grandeurEnfantA = new(TrouverCoupureAléatoire(noeudParent.Valeur.grandeur.x, grandeurPièce.x), noeudParent.Valeur.grandeur.y);
               grandeurEnfantB = new(noeudParent.Valeur.grandeur.x - grandeurEnfantA.x, grandeurEnfantA.y);
               break;
            default:
               return (null, null);
         }

         return (new Noeud<dimensionsPièce>(noeudParent, new dimensionsPièce(grandeurEnfantA, noeudParent.Valeur.CoordonnéesBasGauche + grandeurEnfantA/2)),
               new Noeud<dimensionsPièce>(noeudParent, new dimensionsPièce(grandeurEnfantB,  noeudParent.Valeur.CoordonnéesHautDroit - grandeurEnfantB/2)));
      }

      void CréerLiaisonPhysiquePièces(List<Noeud<dimensionsPièce>> piècesNonLiées)
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
         return longueurParent / 2 - Random.Range(0, longueurParent / 2 - longueurMinEnfant);//- Random.Range(0, longueurParent - longueurMinEnfant * 2);
      }

      Direction TrouverDirectionCoupure(dimensionsPièce dimensions)
      {
         //pour couper verticalement ou horizontalement, les pièces résultants ne doivents pas être trop petit
         bool coupureVerticale = dimensions.grandeur.x > grandeurPièce.x * 2;
         bool coupureHorizontale = dimensions.grandeur.y > grandeurPièce.y * 2;

         if (coupureVerticale && coupureHorizontale)
            return Random.Range(0, 2) == 0 ? Direction.Verticale : Direction.Horizontale;
         else if (coupureVerticale)
            return Direction.Verticale;
         else if (coupureHorizontale)
            return Direction.Horizontale;
         return Direction.Aucune;
      }

      static bool AreRoomsConnected(dimensionsPièce PièceA, dimensionsPièce PièceB)
      {
         Vector2 distance = PièceA.coordonnées - PièceB.coordonnées;
         distance = distance.Abs();
         Vector2 chevauchementDistance = new(distance.x - (PièceA.grandeur.x + PièceB.grandeur.x)/2,distance.y - (PièceA.grandeur.y + PièceB.grandeur.y)/2);
         return chevauchementDistance.x <= 0 && chevauchementDistance.y <= 0;
      }
   }
}
