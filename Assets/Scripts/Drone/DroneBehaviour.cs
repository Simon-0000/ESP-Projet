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

[RequireComponent(typeof(BoundsManager))]
public class DroneBehaviour : MonoBehaviour
{
    

    public Transform followTarget;// le joueur à suivre
    public Transform shootingTarget;
    public List<GameObject> shootingTargets;
    public float speed = 1.0f; // la vitesse du drone
    public float maximumDistance = 10.0f;
    Vector3 lastPt;
    public VectorRange possibleSplineSize;
    [SerializeField] private float largeurSplineMax = 5; // multiplicateur des vecteurs bi-normaux
    [SerializeField] private int numberOfRaycastPerSpline = 6;
    [SerializeField] private DroneTurret turret;
    public float ptParMetre = 3.0f;

    private Queue<Vector3> splinePoints = new(); // les points pour la spline
    List<GameObject> unavailableShootingTargets = new();
    private Hashtable splineArgs;
    private Vector3 lastPLayerPoint;
    private bool restartSplines = true;
    private float splineOffset;
    private int splineDirection = 1; // -1 à droite, 1 à gauche

    public void Start()
    {
        splineOffset = GetComponent<BoundsManager>().objectBoundsLocal.size.x/2;
        if(FindObjectOfType<ZombieManager>() != null)
            shootingTargets = FindObjectOfType<ZombieManager>().AttackingZombies;
        Debug.Assert(followTarget != null);
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
            float splineSizeDecrement = 0;
            RaycastHit firstSplineHit;
            if(Physics.Raycast(newPosition + distanceObjet/2, Vector3.Cross(distanceObjet, transform.up).normalized *splineDirection, out firstSplineHit, largeurSplineMax)) 
            {
                splineSizeDecrement = largeurSplineMax - firstSplineHit.distance + GameConstants.OVERLAP_TOLERANCE;
            }
            float splineDecrementValue = (largeurSplineMax - splineSizeDecrement) / numberOfRaycastPerSpline - GameConstants.ACCEPTABLE_ZERO_VALUE;

            while (splineSizeDecrement < largeurSplineMax) // ça va crash si 9.5, à résoudre
            {
                binormal = Vector3.Cross(distanceObjet, transform.up).normalized * (largeurSplineMax - splineSizeDecrement);
                ptSommet = newPosition + distanceObjet / 2 + splineDirection * binormal ;
               Debug.DrawRay(ptSommet,
                 (newPosition) - ptSommet, Color.green, 10, true); 
                Debug.DrawRay(ptSommet, (newPosition + distanceObjet) - ptSommet, Color.blue, 10, true);
                // if (Physics.Raycast(ptSommet,
                 //        (newPosition ) - ptSommet, Vector3.Distance(newPosition, ptSommet) ) || Physics.Raycast(ptSommet,
                //        (newPosition + distanceObjet) - ptSommet, Vector3.Distance(newPosition + distanceObjet , ptSommet)))
                if (Physics.Linecast(newPosition, ptSommet) || Physics.Linecast(newPosition + distanceObjet, ptSommet))
                {
                    splineSizeDecrement += splineDecrementValue;
                }
                else
                {
                    break;

                }
            }
            if (splineSizeDecrement >= largeurSplineMax)
                Debug.Log("Erreur");

            //newPosition = new Vector3(newPosition.x, rnd, newPosition.z);
            // points.Add(newPosition+(ptSommet-newPosition)/2);
            points.Add(ptSommet - (binormal.normalized *splineOffset  * splineDirection));
            points.Add(newPosition + distanceObjet);

            newPosition += distanceObjet;
        }
        points.Add(newPosition);
        float time = 1 / speed;//(distanceObjet.magnitude * nbrPoints) / speed;

        splinePoints = new Queue<Vector3>(points);

        // Instanciation du trajet iTween
        splineArgs = new Hashtable();
        //args.Add("path", splinePoints);
        splineArgs.Add("time", time);
        splineArgs.Add("oncomplete", "MoveToNextPoints");
        splineArgs.Add("orienttopath", true);
        splineArgs.Add("looktime", time);
        splineArgs.Add("lookahead", 0.5f);

        splineArgs.Add("easetype", iTween.EaseType.linear);
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
        splineArgs.Remove("path");
        splineArgs.Add("path", nextPoints);
        iTween.MoveTo(gameObject, splineArgs);

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