using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

//sergio abreo alvarez
[System.Serializable]
public class Chase : ActionNode
{
    private ZombieBehaviour host;

    protected override void OnStart() {
        host = context.zombie;
        host.ManageChase();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (host.CanChangeState(1.5f))
            return State.Success;

        host.ManageChase();
        return State.Running;
    }
}
