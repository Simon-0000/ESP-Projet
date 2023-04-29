using System;
using Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;
using System.Linq;


//using Pixelplacement;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class DroneBehaviour : MonoBehaviour
{

    public Transform followTarget;// le joueur à suivre
    public Transform shootingTarget;
    public List<GameObject> shootingTargets;
    List<GameObject> unavailableShootingTargets = new();

    public float speed = 1.0f; // la vitesse du drone
    public float maximumDistance = 10.0f;
    Vector3 lastPt;
    public VectorRange possibleSplineSize;
    private Queue<Vector3> splinePoints = new(); // les points pour la spline
    private int splineDirection = 1; // -1 à droite, 1 à gauche
    [SerializeField] private int largeurSplineMax = 5; // multiplicateur des vecteurs bi-normaux
    public float ptParMetre = 3.0f;
    private Hashtable args;
    private bool restartSplines = true;
    private Vector3 lastPLayerPoint;
    [SerializeField] private DroneTurret turret;

    public void Start()
    {
        StartCoroutine(InstantiateDrone());
        shootingTargets = FindObjectOfType<ZombieManager>().AttackingZombies;

    }
    IEnumerator InstantiateDrone()
    {
        Physics.SyncTransforms();
        yield return new WaitForSeconds(0.25f);
        lastPt = transform.position;
        MakePath(followTarget);
    }

    void MakePath(Transform target)
    {
        //splineDirection *= -1;
        // définir la hauteur du drone en Y
        //rnd = Random.Range(-2, 10);

        // Création des points pour la Spline
        Vector3 newPosition = lastPt;
        Vector3 distanceObjet = -(newPosition - (target.transform.position - (target.transform.position - newPosition).normalized * maximumDistance / 2));
        if (distanceObjet.magnitude / 2 < ptParMetre)
        {
            //splinePoints.Clear();
            return;
        }

        int nbrPoints = 2 * ((int)(distanceObjet.magnitude / Mathf.Min(ptParMetre, distanceObjet.magnitude / 2 - GameConstants.ACCEPTABLE_ZERO_VALUE)) / 2) * 2;
        distanceObjet /= nbrPoints;
        Vector3 binormal = Vector3.Cross(distanceObjet, transform.up).normalized * largeurSplineMax;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < nbrPoints; i++)
        {
            splineDirection *= -1;
            Vector3 ptSommet = new();
            int j = 0;
            while (j < largeurSplineMax) // ça va crash si 9.5, à résoudre
            {
                binormal = Vector3.Cross(distanceObjet, transform.up).normalized * (largeurSplineMax - j);
                ptSommet = newPosition + distanceObjet / 2 + splineDirection * binormal;
            //    RaycastHit hit;
              //  Debug.DrawRay(ptSommet,
               //  (newPosition) - ptSommet, Color.green, 10, true); 
            //    Debug.DrawRay(ptSommet, (newPosition + distanceObjet) - ptSommet, Color.blue, 10, true);
                // if (Physics.Raycast(ptSommet,
                //         (newPosition - distanceObjet / 2) - ptSommet, Vector3.Distance((newPosition - distanceObjet / 2), ptSommet) ) || Physics.Raycast(ptSommet,
                //        (newPosition + distanceObjet / 2) - ptSommet, Vector3.Distance((newPosition + distanceObjet / 2) , ptSommet)))
                if (Physics.Linecast(ptSommet, newPosition) || Physics.Linecast(ptSommet, newPosition + distanceObjet))
                {
                    j++;
                }
                else
                {
                    break;

                }
            }

            //newPosition = new Vector3(newPosition.x, rnd, newPosition.z);
            // points.Add(newPosition+(ptSommet-newPosition)/2);
            points.Add(ptSommet);
            points.Add(newPosition + distanceObjet);

            newPosition += distanceObjet;
        }
        points.Add(newPosition);
        float time = 1 / speed;//(distanceObjet.magnitude * nbrPoints) / speed;

        splinePoints = new Queue<Vector3>(points);

        // Instanciation du trajet iTween
        args = new Hashtable();
        //args.Add("path", splinePoints);
        args.Add("time", time);
        args.Add("oncomplete", "MoveToNextPoints");
        args.Add("orienttopath", true);
        args.Add("looktime", time);
        args.Add("lookahead", 0.5f);

        args.Add("easetype", iTween.EaseType.linear);
        if (restartSplines)
        {
            MoveToNextPoints();
            restartSplines = false;

        }
        lastPLayerPoint = target.transform.position;

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

        Vector3[] nextPoints;
        splineDirection *= -1;
        nextPoints = new[] { splinePoints.Dequeue(), splinePoints.Dequeue() };
        if (splinePoints.Count == 1)
            splinePoints.Dequeue();
        args.Remove("path");
        args.Add("path", nextPoints);
        iTween.MoveTo(gameObject, args);

        //iTween.MoveTo(gameObject, args);
        lastPt = nextPoints[nextPoints.Length - 1];
    }


    public void FollowPlayer()
    {
        if (//Algos.GetVectorAbs(transform.position - lastPt).magnitude <= GameConstants.OVERLAP_TOLERANCE &&
            Vector3.Distance(followTarget.transform.position, lastPLayerPoint) > maximumDistance)

        {
            if (Algos.GetVectorAbs(transform.position - followTarget.position).magnitude > maximumDistance)
                MakePath(followTarget);
            else
                splinePoints.Clear();
        }
        // if(restartSplines == false)
    }
    void Update()
    {
        FollowPlayer();
        ManageNextAttack();
    }
    void ManageNextAttack()
    {
        if (shootingTarget == null)
        {
            unavailableShootingTargets.Clear();
            do
            {
                unavailableShootingTargets.RemoveAll(target => target == null);
                shootingTarget = GetClosestShootingTarget(unavailableShootingTargets);
                if (shootingTarget == null)
                    break;
                unavailableShootingTargets.Add(shootingTarget.gameObject);
            } while (!turret.TryToAttack(shootingTarget));
        }
        else if (!turret.TryToAttack(shootingTarget))
        {
            shootingTarget = null;
        }
    }

    public Transform GetClosestShootingTarget(List<GameObject> ignoreTargets)
    {
        List<GameObject> possibleTargets = shootingTargets.Where(t => t!= null && !ignoreTargets.Contains(t)).ToList();
        if (possibleTargets.Count == 0)
            return null;
        Transform nextShootingTarget = possibleTargets[0].transform;
        for (int i = 1; i < possibleTargets.Count; ++i)
            if (nextShootingTarget == null || TargetIsCloserThan(possibleTargets[i].transform, nextShootingTarget, followTarget.position))
                nextShootingTarget = possibleTargets[i].transform;
        return nextShootingTarget;
    }

    bool TargetIsCloserThan(Transform targetA,Transform targetB,Vector3 pointToEvaluateDistance)
    {
        return Vector3.Distance(targetA.position, pointToEvaluateDistance) < Vector3.Distance(targetB.position, pointToEvaluateDistance);
    }
}