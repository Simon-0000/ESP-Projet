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
         public static Vector3 GetRandomVector(Vector3 startAt, Vector3 endAt) =>
            new Vector3(Random.Range(startAt.x, endAt.x), Random.Range(startAt.y, endAt.y), Random.Range(startAt.z, endAt.z));
    }
}
