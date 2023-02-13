using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
   public struct RectangleInfo2d
   {
      public RectangleInfo2d(Vector2 grandeur, Vector2 coordonnées) 
      {
         this.grandeur = grandeur;
         this.coordonnées = coordonnées;
      }
      public Vector2 grandeur, coordonnées;
      public Vector2 CoordonnéesHautGauche => coordonnées + new Vector2(- grandeur.x/2,grandeur.y/2);
      public Vector2 CoordonnéesHautDroit => coordonnées + new Vector2(grandeur.x/2,grandeur.y/2);
      public Vector2 CoordonnéesBasGauche => coordonnées + new Vector2(-grandeur.x/2,-grandeur.y/2);
      public Vector2 CoordonnéesBasDroit => coordonnées + new Vector2(grandeur.x/2,-grandeur.y/2);
   }
}
