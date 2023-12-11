using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayer : DecoratorNode
{
    private bool seePlayer;
    private float timer;

    
    protected override void OnStart()
    {
        timer = unit.followTimeAfterEscape;
    }

    protected override State OnUpdate()
    {
        
    }

    protected override void OnStop()
    {
        
    }
    
    private void SeePlayer()
    {
        Physics.Raycast(unit.transform.position, (unit.playerTransform.position - unit.transform.position), out RaycastHit hit, unit.mask);
        if (hit.collider == null)
        {
            seePlayer = true;
            timer = unit.followTimeAfterEscape;
        }
        else
        {
            seePlayer = false;
        }
    }
}
