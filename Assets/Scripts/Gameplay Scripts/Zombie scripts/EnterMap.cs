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

    protected override State OnUpdate() {
        if(host.CanEnterMap())
        {
            Debug.Log( host.animator.GetBool("vault"));
            host.animator.SetBool("vault", true);
            Debug.Log( host.animator.GetBool("vault"));
            return State.Success;
        }
        

        return State.Running;
    }
}
