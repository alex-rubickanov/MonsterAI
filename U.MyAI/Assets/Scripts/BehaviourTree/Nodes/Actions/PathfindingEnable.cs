using Pathfinding;
using UnityEngine;

public class PathfindingEnable : ActionNode
{
    private Unit pathfindingUnit;
    
    protected override void OnStart()
    {
         
    }

    protected override State OnUpdate()
    {
        return State.Running;
    }

    protected override void OnStop()
    {
        
    }
}
