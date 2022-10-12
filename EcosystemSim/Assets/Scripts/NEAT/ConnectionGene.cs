using UnityEngine;
using System.Collections;

public class ConnectionGene : Gene
{
    // FIELDS
    private NodeGene from;
    private NodeGene to;

    private double weight;
    private bool enabled = true;

    // PROPERTIES
    public NodeGene From
    {
        get
        {
            return from;
        }
        set
        {
            from = value;
        }
    }
    public NodeGene To
    {
        get
        {
            return to;
        }
        set
        {
            to = value;
        }
    }
    public double Weight
    {
        get
        {
            return weight;
        }
        set
        {
            weight = value;
        }
    }
    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = value;
        }
    }

    // CONSTRUCTOR
    public ConnectionGene(NodeGene from, NodeGene to)
    {
        this.from = from;
        this.to = to;
    }
}
