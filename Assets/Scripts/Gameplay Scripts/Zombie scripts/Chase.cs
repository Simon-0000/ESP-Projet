using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

//sergio abreao alvarez
[System.Serializable]
public class Chase : ActionNode
{
    public GameObject target;
    public Vector3 hostPosition;

    protected override void OnStart() {
        hostPosition = context.transform.position;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (!((target.transform.position - hostPosition).sqrMagnitude > 1))
            return State.Success;

        return State.Running;
    }
}
