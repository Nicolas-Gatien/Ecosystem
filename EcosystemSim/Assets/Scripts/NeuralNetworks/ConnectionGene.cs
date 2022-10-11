using UnityEngine;
using System.Collections;

public class ConnectionGene
{
    public Node from;
    public Node to;
    public float weight;
    public bool enabled;
    public int innovation;

    public ConnectionGene(Node from, Node to)
    {
        this.from = from;
        this.to = to;
        weight = Random.Range(-2f, 2f);
        enabled = true;
    }
}
