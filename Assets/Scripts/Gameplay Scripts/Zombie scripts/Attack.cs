using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

//sergio abreo alvarez
[System.Serializable]
public class Attack : ActionNode
{
    private ZombieBehaviour host;
    protected override void OnStart() {
        host = context.zombie;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.animator.SetBool("attack",true);
        
        host.Attack();
        return State.Success;
    }
}
