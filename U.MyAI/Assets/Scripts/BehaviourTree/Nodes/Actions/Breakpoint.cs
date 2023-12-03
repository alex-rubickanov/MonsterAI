using UnityEditor;

public class Breakpoint : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override State OnUpdate()
    {
        EditorApplication.isPaused = true;
        return State.Success;
    }

    protected override void OnStop()
    {
    }
}