public class StepNode : Node
{
    public override float ProcessInput(float input)
    {
        if (input > 0)
        {
            return 1;
        }

        return 0;
    }
}
