using System;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // FIELDS
    private double x;
    private double output;
    private List<Connection> connections = new List<Connection>();

    // PROPERTIES
    public double X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
        }
    }
    public double Output
    {
        get
        {
            return output;
        }
        set
        {
            output = value;
        }
    }
    public List<Connection> Connections
    {
        get
        {
            return connections;
        }
        set
        {
            connections = value;
        }
    }

    // CONSTRUCTOR
    public Node(double x)
    {
        this.x = x;
    }

    // METHODS
    public void Calculate()
    {
        double sum = 0;
        foreach (Connection con in connections)
        {
            if (con.Enabled)
            {
                sum += con.Weight * con.From.Output;
            }
        }
        output = ActivationFunction(sum);
    }

    private double ActivationFunction(double x)
    {
        return Math.Tanh(x);
    }
}
