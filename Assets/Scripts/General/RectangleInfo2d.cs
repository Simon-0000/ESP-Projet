//Auteurs: Simon Asmar
//Explication: Ce struct est utilisé pour définir un rectangle dans un espace 2d (ex d'utilisation: diviser les 
//             dimensions des pièces en 2d)

using UnityEngine;

namespace Assets
{
   public struct RectangleInfo2d
   {
      public RectangleInfo2d(Vector2 size, Vector2 coordinates) 
      {
         this.size = size;
         this.coordinates = coordinates;
      }
      public Vector2 size, coordinates;
      public Vector2 TopLeftCoordinates => coordinates + new Vector2(- size.x/2,size.y/2);
      public Vector2 TopRightCoordinates => coordinates + new Vector2(size.x/2,size.y/2);
      public Vector2 BottomLeftCoordinates => coordinates + new Vector2(-size.x/2,-size.y/2);
      public Vector2 BottomRightCoordinates => coordinates + new Vector2(size.x/2,-size.y/2);
   }
}
