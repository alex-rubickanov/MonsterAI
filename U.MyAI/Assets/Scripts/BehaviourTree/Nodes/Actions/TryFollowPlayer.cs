using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryFollowPlayer : ActionNode
{

    
    protected override void OnStart()
    {
        unit.Follow(unit.playerTransform);
    }

    protected override State OnUpdate()
    {
        SeePlayer();
        if (seePlayer)
        {
            return State.Running;
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                unit.StopAllCoroutines();
                return State.Success;
            }
        }
        
        return unit.isCoroutineFinished ? State.Success : State.Running;
    }

    protected override void OnStop()
    {
        
    }
    
    
}
