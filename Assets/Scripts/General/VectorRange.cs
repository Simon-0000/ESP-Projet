﻿//Auteurs: Simon Asmar
//Explication: Cette classe a pour but de contenir un «Range» de vecteur (startAt, endAt) et de retourner une
//             valeur entre ces vecteurs
using System;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Assets
{
    [Serializable]
    public class VectorRange
    {
        public Vector3 startAt;
        public Vector3 endAt;
        public VectorRange()
        {
            startAt = Vector3.zero;
            endAt = Vector3.zero;
        }

        public VectorRange(Vector3 startVector, Vector3 endVector)
        {
            startAt = startVector;
            endAt = endVector;
        }
        public Vector3 GetRandomVector() =>
            GetRandomVector(this);

        public static Vector3 GetRandomVector(VectorRange vectorRange) =>
            new Vector3(Random.Range(vectorRange.startAt.x, vectorRange.endAt.x), Random.Range(vectorRange.startAt.y, vectorRange.endAt.y), Random.Range(vectorRange.startAt.z, vectorRange.endAt.z));
    }
}
