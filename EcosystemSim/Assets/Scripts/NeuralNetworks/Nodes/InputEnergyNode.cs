public class InputEnergyNode : InputNode
{
    Creature me;

    public InputEnergyNode(Creature me)
    {
        this.me = me;
    }

    public override float GetInput()
    {
        return me.energy / me.maxEnergy;
    }
}