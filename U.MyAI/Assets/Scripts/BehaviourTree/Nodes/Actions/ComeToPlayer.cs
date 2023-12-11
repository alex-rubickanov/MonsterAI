using System.Collections;
using UnityEngine;

public class ComeToPlayer : ActionNode
{
    private bool finished = false;
    
    protected override void OnStart()
    {
        //finished = false;
        //unit.OnCoroutineFinishedCallback += Finish;
        unit.GoTo(unit.playerTransform);
    }

    protected override State OnUpdate()
    {
        return unit.isCoroutineFinished ? State.Success : State.Running;
    }

    protected override void OnStop()
    {
        
    }

    private void Finish()
    {
        finished = true;
    }
}
