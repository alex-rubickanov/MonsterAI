public class SequencerNode : CompositeNode
{
    protected int currentChild;
    protected override void OnStart()
    {
        currentChild = 0;
    }

    protected override State OnUpdate() {
        for (int i = currentChild; i < children.Count; ++i) {
            currentChild = i;
            var child = children[currentChild];

            switch (child.Update()) {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    continue;
            }
        }

        return State.Success;
    }

    protected override void OnStop()
    {
        
    }
}
