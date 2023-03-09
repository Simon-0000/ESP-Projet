using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

// sergio abreo alvarez
[System.Serializable]
public class Patrol : ActionNode
{
    public float fieldOfView;
    public GameObject target;
    public Vector3 hostPosition;
    private int counter;

    protected override void OnStart() {
        counter = 0;
        hostPosition = context.transform.position;
        context.agent.speed = context.zombie.speed;
        context.agent.destination = context.zombie.patrolLocations[counter];
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (CanStopPatrol())
            return State.Success;

        if (hostPosition == context.agent.destination)
        {
            context.agent.destination = context.zombie.patrolLocations[counter++];
            if (counter == context.zombie.patrolLocations.Count)
                counter = 0;
        }
        return State.Running;
    }

    public bool CanStopPatrol()
    {
        Vector3 direction = target.transform.position - hostPosition;
        return Vector3.Angle(direction, context.transform.forward) < fieldOfView;
    }
}
