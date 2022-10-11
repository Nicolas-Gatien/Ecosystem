using UnityEngine;
using System.Collections;

public class InputHealthNode : InputNode
{
    Creature me;

    public InputHealthNode(Creature me)
    {
        this.me = me;
    }

    public override float GetInput()
    {
        return me.health / me.maxHealth;
    }
}
