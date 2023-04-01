using System;
using Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;


//using Pixelplacement;
using Unity.VisualScripting;
using UnityEditor.TextCore.Text;
using Random = UnityEngine.Random;

public class DroneBehaviour : MonoBehaviour {

    public Transform target; // le joueur à suivre
    public Transform target2; // le zombie le plus près
    public float speed = 1.0f; // la vitesse du drone
    public float followDistance = 6.0f; // la distance à laquelle se tenir par rapport au joueur
    public float maximumDistance = 7.0f;
    private Vector3 lastPt = Vector3.zero;
    private Vector3 lastTargetPos;
    public VectorRange possibleSplineSize;
    private Queue<Vector3> splinePoints = new(); // les points pour la spline
    private int splineDirection = 1; // -1 à droite, 1 à gauche
    const int KBN = 5; // multiplicateur des vecteurs bi-normaux
    public float ptParMetre = 0.2f;
    private Hashtable args;
    private bool restartSplines = true;
    private Vector3 lastPLayerPoint;
    private Vector3 vecteurY;
    private int rnd;

    void Awake()
    {
        Debug.Assert(maximumDistance > followDistance);
    }
    void Start () {
        MakePath(target);
    }

    void MakePath(Transform target)
    {
        // définir la hauteur du drone en Y
        rnd = Random.Range(-2, 10);
        
        // Création des points pour la Spline
        Vector3 newPosition = transform.position;
        Vector3 distanceObjet = -(newPosition - (target.transform.position - (target.transform.position - newPosition).normalized * followDistance));
        if (distanceObjet.magnitude / 2 < ptParMetre)
        {
            splinePoints.Clear();
            return;
        }
            
        int nbrPoints = ((int) (distanceObjet.magnitude / Mathf.Min(ptParMetre, distanceObjet.magnitude / 2 - GameConstants.ACCEPTABLE_ZERO_VALUE)) / 2) * 2;
        distanceObjet /= nbrPoints;
        Vector3 binormal = Vector3.Cross(distanceObjet, transform.up).normalized * KBN;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < nbrPoints; i++)
        {
            splineDirection *= -1;
            newPosition += distanceObjet + splineDirection * binormal;
            newPosition = new Vector3(newPosition.x, rnd, newPosition.z);
            points.Add(newPosition);
        }

        lastPLayerPoint = points[points.Count - 1];
        float time = speed;//(distanceObjet.magnitude * nbrPoints) / speed;
        
        splinePoints =new Queue<Vector3>(points);
        
        // Instanciation du trajet iTween
        args = new Hashtable();
        //args.Add("path", splinePoints);
        args.Add("time", time);
        args.Add("oncomplete", "MoveToNextPoints" );

        args.Add("easetype", iTween.EaseType.easeInSine);
        if (restartSplines)
        {
            restartSplines = false;
            MoveToNextPoints();
        }
        //iTween.MoveTo(gameObject,args);
    }

    void MoveToNextPoints()
    {
        if (splinePoints.Count == 0)
        {
            restartSplines = true;
            Debug.Log("no more points");
            return;
            
        }
        Vector3[] nextPoints = new[] { splinePoints.Dequeue(), splinePoints.Dequeue() };
        args.Remove("path");
        args.Add("path", nextPoints);
        iTween.MoveTo(gameObject,args);
        lastPt = nextPoints[nextPoints.Length - 1];


    }

    void Update ()
    {
        if (//Algos.GetVectorAbs(transform.position - lastPt).magnitude <= GameConstants.OVERLAP_TOLERANCE &&
            Algos.GetVectorAbs(target.transform.position - lastPLayerPoint).magnitude > maximumDistance)
            
        {
            if (Algos.GetVectorAbs(transform.position - target.position).magnitude > maximumDistance)
                MakePath(target);
            else
                splinePoints.Clear();
        }
                
        
        
    }
}