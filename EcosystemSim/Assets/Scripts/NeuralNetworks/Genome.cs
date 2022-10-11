using UnityEngine;
using System.Collections;

public class Genome
{
    public Node[] nodes;
    public ConnectionGene[] connections;

    public Genome(Node[] nodes, ConnectionGene[] connections)
    {
        this.nodes = nodes;
        this.connections = connections;
    }
}
