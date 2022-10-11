using UnityEngine;
using System.Collections;
using System;

public class TanhNode : Node
{
    public override float ProcessInput(float input)
    {
        return (float)Math.Tanh(input);
    }
}
