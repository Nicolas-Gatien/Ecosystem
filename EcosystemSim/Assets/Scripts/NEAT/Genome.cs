using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{
    // FIELDS
    private List<ConnectionGene> connections = new List<ConnectionGene>();
    private List<NodeGene> nodes = new List<NodeGene>();

    private Neat neat;
    private Calculator calculator;

    // PROPERTIES
    public List<ConnectionGene> getConnections{
        get{
            List<ConnectionGene> sortedConnections = new List<ConnectionGene>();
            sortedConnections = connections.OrderBy(gene => gene.InnovationNumber).ToList();

            return sortedConnections;
        }
    }
    public List<NodeGene> getNodes {
        get {
            return nodes;
        }
    }
    public Neat getNeat {
        get {
            return neat;
        }
    }

    // CONSTRUCTOR
    public Genome(Neat neat) {
        this.neat = neat;
    }

    // METHODS
    public double Distance(Genome g2) {
        Genome g1 = this;

        int highestInnovationGene1 = 0;
        if (g1.getConnections.Count != 0)
        {
            highestInnovationGene1 = g1.getConnections[g1.getConnections.Count - 1].InnovationNumber;
        }
        int highestInnovationGene2 = 0;
        if (g2.getConnections.Count != 0)
        {
            highestInnovationGene2 = g2.getConnections[g2.getConnections.Count - 1].InnovationNumber;
        }

        if (highestInnovationGene1 < highestInnovationGene2)
        {
            Genome g = g1;
            g1 = g2;
            g2 = g;
        }

        int indexG1 = 0;
        int indexG2 = 0;

        int disjoint = 0;
        int excess = 0;
        double weightDiff = 0;
        int similar = 0;

        while (indexG1 < g1.getConnections.Count && indexG2 < g2.getConnections.Count)
        {
            ConnectionGene gene1 = g1.getConnections[indexG1];
            ConnectionGene gene2 = g2.getConnections[indexG2];

            int innov1 = gene1.InnovationNumber;
            int innov2 = gene2.InnovationNumber;

            if (innov1 == innov2)
            {
                similar++;
                
                if (gene1.Weight > gene2.Weight)
                {
                    weightDiff += gene1.Weight - gene2.Weight;
                }                
                if (gene2.Weight > gene1.Weight)
                {
                    weightDiff += gene2.Weight - gene1.Weight;
                }

                indexG1++;
                indexG2++;
            }

            if (innov1 > innov2)
            {
                disjoint++;
                innov2++;
            }
            if (innov2 > innov1)
            {
                disjoint++;
                innov1++;
            }
        }

        weightDiff /= similar;

        excess += g1.getConnections.Count - indexG1;

        double N = Math.Max(g1.getConnections.Count, g2.getConnections.Count);
        if (N < 20)
        {
            N = 1;
        }

        // MAKE SURE TO ADD TRAIT DIFF
        return (
            neat.C1 * (disjoint / N)) 
            + (neat.C2 * (excess / N)) 
            + (neat.C3 * weightDiff
            );
    }
    public static Genome CrossOver(Genome g1, Genome g2) {
        Neat neat = g1.getNeat;

        Genome child = neat.EmptyGenome();

        int indexG1 = 0;
        int indexG2 = 0;

        while (indexG1 < g1.getConnections.Count && indexG2 < g2.getConnections.Count)
        {
            ConnectionGene gene1 = g1.getConnections[indexG1];
            ConnectionGene gene2 = g2.getConnections[indexG2];

            int innov1 = gene1.InnovationNumber;
            int innov2 = gene2.InnovationNumber;

            if (innov1 == innov2)
            {
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                {
                    child.getConnections.Add(Neat.GetConnection(gene1));
                }else
                {
                    child.getConnections.Add(Neat.GetConnection(gene2));
                }

                indexG1++;
                indexG2++;
            }

            if (innov1 > innov2)
            {
                child.getConnections.Add(Neat.GetConnection(gene2));
                innov2++;
            }
            if (innov2 > innov1)
            {
                child.getConnections.Add(Neat.GetConnection(gene1));
                innov1++;
            }
        }

        while (indexG1 < g1.getConnections.Count)
        {
            ConnectionGene gene1 = g1.getConnections[indexG1];
            child.getConnections.Add(Neat.GetConnection(gene1));
            indexG1++;
        }

        foreach (ConnectionGene gene in child.getConnections)
        {
            child.getNodes.Add(gene.From);
            child.getNodes.Add(gene.To);
        }

        return child;
    }

    public void GenerateCalculator()
    {
        calculator = new Calculator(this);
    }
    public double[] Calculate(params double[] ar)
    {
        if (calculator == null)
        {
            GenerateCalculator();
        }
        return calculator.Calculate(ar);
    }

    // %%% CLEAN THIS UP
    public void Mutate() {
        if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
        {
            MutateLink();
        }
        if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
        {
            MutateLinkToggle();
        }
        if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
        {
            MutateNode();
        }
        if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
        {
            MutateWeightRandom();
        }
        if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
        {
            MutateWeightShift();
        }
    }

    public void MutateLink()
    {
        for (int i = 0; i < 100; i++)
        {
            NodeGene a = GetRandomNode(nodes);
            NodeGene b = GetRandomNode(nodes);

            if (a.X == b.X)
            {
                continue;
            }

            ConnectionGene con;
            if (a.X > b.X)
            {
                con = new ConnectionGene(a, b);
            }else
            {
                con = new ConnectionGene(b, a);
            }

            if (connections.Contains(con))
            {
                continue;
            }
            con = neat.GetConnection(con.From, con.To);

            con.Weight = UnityEngine.Random.Range(-2f, 2f);
            connections.Add(con);

            return;
        }
    }
    public void MutateNode()
    {
        ConnectionGene con = GetRandomConnect(connections);
        if (con == null)
        {
            return;
        }

        NodeGene from = con.From;
        NodeGene to = con.To;

        NodeGene middle = neat.GetNode();
        middle.X = (from.X + to.X) / 2;
        middle.Y = (from.Y + to.Y) / 2;

        ConnectionGene con1 = neat.GetConnection(from, middle);
        ConnectionGene con2 = neat.GetConnection(middle, to);

        con1.Weight = 1;
        con2.Weight = con.Weight;
        con2.Enabled = con.Enabled;

        connections.Remove(con);
        connections.Add(con1);
        connections.Add(con2);

        nodes.Add(middle);
    }
    public void MutateWeightShift()
    {
        ConnectionGene con = GetRandomConnect(connections);
        if (con != null)
        {
            con.Weight += UnityEngine.Random.Range(-0.3f, 0.3f);
        }
    }
    public void MutateWeightRandom()
    {
        ConnectionGene con = GetRandomConnect(connections);
        if (con != null)
        {
            con.Weight = UnityEngine.Random.Range(-2f, 2f);
        }
    }
    public void MutateLinkToggle()
    {
        ConnectionGene con = GetRandomConnect(connections);
        if (con != null)
        {
            con.Enabled = !con.Enabled;
        }
    }

    private ConnectionGene GetRandomConnect(List<ConnectionGene> genes)
    {
        int index = UnityEngine.Random.Range(0, genes.Count);
        return genes[index];
    }

    private NodeGene GetRandomNode(List<NodeGene> genes)
    {
        int index = UnityEngine.Random.Range(0, genes.Count);
        return genes[index];
    }
}
