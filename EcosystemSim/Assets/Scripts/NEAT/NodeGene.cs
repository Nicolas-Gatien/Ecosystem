using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGene : Gene
{
    // FIELDS
    private double x;
    private double y;

    // PROPERTIES
    public double X
    {
        get { return x; }
        set { x = value; }
    }
    public double Y
    {
        get { return y; }
        set { y = value; }
    }

    // CONSTRUCTOR
    public NodeGene(int innovationNumber) : base(innovationNumber)
    {

    }
}
