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
      public RectangleInfo2d(Vector2 grandeur, Vector2 coordinate) 
      {
         this.Grandeur = grandeur;
         this.coordinates = coordinate;
      }
      public Vector2 Grandeur, coordinates;
      public Vector2 CoordinatesHautGauche => coordinates + new Vector2(- Grandeur.x/2,Grandeur.y/2);
      public Vector2 CoordinatesHautDroit => coordinates + new Vector2(Grandeur.x/2,Grandeur.y/2);
      public Vector2 CoordinatesBasGauche => coordinates + new Vector2(-Grandeur.x/2,-Grandeur.y/2);
      public Vector2 CoordinatesBasDroit => coordinates + new Vector2(Grandeur.x/2,-Grandeur.y/2);
   }
}
