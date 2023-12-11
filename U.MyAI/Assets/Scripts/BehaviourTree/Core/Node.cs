using Pathfinding;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State state = State.Running;
    public bool started = false;
    
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public Blackboard blackboard;
    [HideInInspector] public Unit unit;
    

    [TextArea] public string description;

    public State Update()
    {
        if (!started)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate();

        if (state is State.Failure or State.Success)
        {
            OnStop();
            started = false;
        }

        return state;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    } 

    protected abstract void OnStart();
    protected abstract State OnUpdate();
    protected abstract void OnStop();

}
