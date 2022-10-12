using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neat
{
    private List<ConnectionGene> allConnections = new List<ConnectionGene>();
    private List<NodeGene> allNodes = new List<NodeGene>();

    private int inputSize;
    private int outputSize;
    private int maxClients;

    public double C1;
    public double C2;
    public double C3;

    public Neat(int inputSize, int outputSize, int clients)
    {
        this.Reset(inputSize, outputSize, clients);
    }

    public Genome EmptyGenome()
    {
        Genome g = new Genome(this);
        for (int i = 0; i < inputSize + outputSize; i++)
        {
            g.getNodes.Add(GetNode(i + 1));
        }
        return g;
    }
    public void Reset(int inputSize, int outputSize, int clients)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.maxClients = clients;

        allConnections.Clear();
        allNodes.Clear();

        for (int i = 0; i < inputSize; i++)
        {
            NodeGene n = GetNode();
            n.X = 0.1f;
            n.Y = (i + 1) / (double)(inputSize + 1);
        }

        for (int i = 0; i < outputSize; i++)
        {
            NodeGene n = GetNode();
            n.X = 0.9f;
            n.Y = (i + 1) / (double)(outputSize + 1);
        }
    }

    public static ConnectionGene GetConnection(ConnectionGene con)
    {
        ConnectionGene c = new ConnectionGene(con.From, con.To);
        c.Weight = con.Weight;
        c.Enabled = con.Enabled;
        return c;
    }
    public ConnectionGene GetConnection(NodeGene node1, NodeGene node2)
    {
        ConnectionGene connectionGene = new ConnectionGene(node1, node2);

        if (allConnections.Contains(connectionGene))
        {
            connectionGene.InnovationNumber = allConnections[allConnections.IndexOf(connectionGene)].InnovationNumber;
        }else
        {
            connectionGene.InnovationNumber = allConnections.Count + 1;
            allConnections.Add(connectionGene);
        }

        return connectionGene;
    }

    public NodeGene GetNode()
    {
        NodeGene n = new NodeGene(allNodes.Count + 1);
        allNodes.Add(n);
        return n;
    }
    public NodeGene GetNode(int id)
    {
        if (id <= allNodes.Count)
        {
            return allNodes[id - 1];
        }
        return GetNode();
    }
}
