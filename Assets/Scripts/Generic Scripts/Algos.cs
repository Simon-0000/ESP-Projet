using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    static class Algos
    {
        
        public static Vector3 GetVectorSign(Vector3 src)
        {
            return new Vector3(Mathf.Sign(src.x), Mathf.Sign(src.y), Mathf.Sign(src.z));
        }
        public static Vector2 GetVectorSign(Vector2 src)
        {
            return new Vector2(Mathf.Sign(src.x), Mathf.Sign(src.y));
        }


        public static Vector3 GetVectorAbs(Vector3 src)
        {
            return new Vector3(Mathf.Abs(src.x), Mathf.Abs(src.y), Mathf.Abs(src.z));
        }
        public static Vector2 GetVectorAbs(Vector2 src)
        {
            return new Vector2(Mathf.Abs(src.x), Mathf.Abs(src.y));
        }
    }
}
