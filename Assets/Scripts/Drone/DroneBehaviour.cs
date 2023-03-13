using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static iTween;

public class DroneBehaviour : MonoBehaviour {

    public Transform target; // the player to follow
    public float speed = 3.0f; // the speed of the drone
    public float followDistance = 2.0f; // the distance to maintain from the player
    public float splineTime = 5.0f; // the time for the drone to follow the spline

    private Vector3[] splinePoints; // the points for the spline

    void Start () {
        MakePath();
    }

    void MakePath()
    {
        // create the spline points
        List<Vector3> points = new List<Vector3>();
        points.Add(transform.position);
        for (int i = 0; i < 5; i++) {
            Vector3 point = new Vector3(Random.Range(transform.position.x, target.transform.position.x), Random.Range(transform.position.y, target.transform.position.y), Random.Range(transform.position.z, target.transform.position.z));
            points.Add(point);
        }
        points.Add(target.position);
        splinePoints = points.ToArray();

        // set up the iTween path
        Hashtable args = new Hashtable();
        args.Add("path", splinePoints);
        args.Add("time", splineTime);
        args.Add("easetype", iTween.EaseType.easeInOutSine);
        //args.Add("looptype", iTween.LoopType.loop);
        iTween.MoveTo(gameObject,args);
    }

    void Update () {
        // move the drone towards the player
        //Vector3 targetPosition = target.position - target.forward * followDistance;
        //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);

        // rotate the drone towards the player
        //Vector3 targetRotation = Quaternion.LookRotation(target.position - transform.position).eulerAngles;
        //transform.rotation = Quaternion.Euler(0, targetRotation.y, 0);
        
        
        
    }
}