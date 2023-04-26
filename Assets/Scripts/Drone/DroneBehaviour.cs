using System;
using Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;


//using Pixelplacement;
using Unity.VisualScripting;
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
    [SerializeField] private int largeurSplineMax = 5; // multiplicateur des vecteurs bi-normaux
    public float ptParMetre = 3.0f;
    private Hashtable args;
    private bool restartSplines = true;
    private Vector3 lastPLayerPoint;
    private Vector3 vecteurY;
    private int rnd;
    [SerializeField] private DroneTurret turret;
    public List<GameObject> attackingZombies;

    void Awake()
    {
        Debug.Assert(maximumDistance > followDistance);
    }
    void Start () {
        MakePath(target);
        attackingZombies = FindObjectOfType<ZombieManager>().AttackingZombies;
    }

    void MakePath(Transform target)
    {
        splineDirection *= -1;
        Debug.Log("QuelqueChose");
        // définir la hauteur du drone en Y
        //rnd = Random.Range(-2, 10);
        
        // Création des points pour la Spline
        Vector3 newPosition = transform.position;
        Vector3 distanceObjet = -(newPosition - (target.transform.position - (target.transform.position - newPosition).normalized * followDistance));
        if (distanceObjet.magnitude / 2 < ptParMetre)
        {
            splinePoints.Clear();
            return;
        }
            
        int nbrPoints = 2*((int) (distanceObjet.magnitude / Mathf.Min(ptParMetre, distanceObjet.magnitude / 2 - GameConstants.ACCEPTABLE_ZERO_VALUE)) / 2) * 2;
        distanceObjet /= nbrPoints;
        Vector3 binormal = Vector3.Cross(distanceObjet, transform.up).normalized * largeurSplineMax;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < nbrPoints; i++)
        {
            splineDirection *= -1;
            Vector3 ptSommet;
            int j = 0;
            while (j < largeurSplineMax) // ça va crash si 9.5, à résoudre
            {
                binormal = Vector3.Cross(distanceObjet, transform.up).normalized * (largeurSplineMax - j);
                ptSommet = newPosition + splineDirection * binormal;
                RaycastHit hit;
                Debug.Log("Bro for real ?");
                Debug.DrawRay(ptSommet,
                    (newPosition - distanceObjet / 2) - ptSommet, Color.green, 10, true); print("Hit");
                Debug.DrawRay(ptSommet, (newPosition + distanceObjet / 2) - ptSommet, Color.blue, 10, true);
                if (Physics.Raycast(ptSommet,
                        (newPosition - distanceObjet / 2) - ptSommet, Vector3.Distance((newPosition - distanceObjet / 2), ptSommet) + 1) || Physics.Raycast(ptSommet,
                        (newPosition + distanceObjet / 2) - ptSommet, Vector3.Distance((newPosition + distanceObjet / 2) , ptSommet) + 1))
                {
                    Debug.Log("Collision imminente");
                    j++;
                }
                else
                {
                    break;
                    
                }
            }
            Debug.Log(j + "," + largeurSplineMax + "," + binormal);

            //newPosition = new Vector3(newPosition.x, rnd, newPosition.z);
            points.Add(newPosition - distanceObjet / 2 + splineDirection * binormal / 2);
            points.Add(newPosition + splineDirection * binormal);
            newPosition += distanceObjet;
        }
        points.Add(newPosition);
        lastPLayerPoint = points[points.Count - 1];
        float time = 1/speed;//(distanceObjet.magnitude * nbrPoints) / speed;
        
        splinePoints =new Queue<Vector3>(points);
        
        // Instanciation du trajet iTween
        args = new Hashtable();
        //args.Add("path", splinePoints);
        args.Add("time", time);
        args.Add("oncomplete", "MoveToNextPoints" );

        args.Add("easetype", iTween.EaseType.linear);
        if (restartSplines)
        {
            restartSplines = false;
            MoveToNextPoints();
        }
        //iTween.MoveTo(gameObject,args);
    }

    void MoveToNextPoints()
    {
        splineDirection *= -1;
        if (splinePoints.Count == 0)
        {
            restartSplines = true;
            Debug.Log("no more points");
            return;
            
        }

        Vector3[] nextPoints;
        splineDirection *= -1;
        nextPoints = new[] { splinePoints.Dequeue(), splinePoints.Dequeue(), splinePoints.Peek() };
        if (splinePoints.Count == 1)
            splinePoints.Dequeue();
        args.Remove("path");
        args.Add("path", nextPoints);
        iTween.MoveTo(gameObject,args);
        lastPt = nextPoints[nextPoints.Length - 1];
        Debug.Log("fuckuSerg");


    }

    public void FollowPlayer ()
    {
        Debug.Log("Allo");
        if (//Algos.GetVectorAbs(transform.position - lastPt).magnitude <= GameConstants.OVERLAP_TOLERANCE &&
            Algos.GetVectorAbs(target.transform.position - lastPLayerPoint).magnitude > maximumDistance)
            
        {
            if (Algos.GetVectorAbs(transform.position - target.position).magnitude > maximumDistance)
                MakePath(target);
            else
                splinePoints.Clear();
        }
                
        
        
    }

    void Update()
    {
        FollowPlayer();
        if (target2 != null)
            turret.TryToAttack(target2);
    }

    void AttackZombie()
    {
        
    }
}