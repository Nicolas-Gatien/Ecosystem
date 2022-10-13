using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    // FIELDS
    private Node from;
    private Node to;

    private double weight;
    private bool enabled = true;

    // PROPERTIES
    public Node From
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
    public Node To
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
    public Connection(Node from, Node to)
    {
        this.from = from;
        this.to = to;
    }
}
