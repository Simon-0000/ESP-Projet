using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class EnterMap : ActionNode
{
    private ZombieBehaviour host;
    protected override void OnStart() {
        host = context.zombie;
    }

    protected override void OnStop() {
    }
    bool hasStartedVaulting,hasReachedWindow;
    protected override State OnUpdate() {
        if (!hasReachedWindow)
        {
            if (host.HasReachedPath())
            {
                hasReachedWindow = true;
                host.agent.SetDestination(host.entryLocation.exitWaypoint.position);
            }
        }
        else if (hasReachedWindow && host.agent.hasPath && !hasStartedVaulting)
        {
            host.transform.LookAt(host.entryLocation.exitWaypoint);
            host.animator.SetBool("vault", true);
            hasStartedVaulting = true;
        }
        else if (hasReachedWindow && hasStartedVaulting &&  host.HasReachedPath())
        {
            return State.Success;
        }
       

        


        return State.Running;
    }
}
