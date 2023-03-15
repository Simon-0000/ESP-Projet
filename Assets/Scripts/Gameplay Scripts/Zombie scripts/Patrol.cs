using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

// sergio abreo alvarez
[System.Serializable]
public class Patrol : ActionNode
{

    private ZombieBehaviour host;
    protected override void OnStart() {
        host = context.zombie;
        host.ManagePatrol();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if ( host.CanChangeState(6))
            return State.Success;

        host.ManagePatrol();
        return State.Running;
    }
}
