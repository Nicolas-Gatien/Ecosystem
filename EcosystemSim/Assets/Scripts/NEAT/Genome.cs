using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{
    // FIELDS
    private List<ConnectionGene> connections = new List<ConnectionGene>();
    private List<NodeGene> nodes = new List<NodeGene>();

    private Neat neat;

    // PROPERTIES
    public List<ConnectionGene> getConnections{
        get{
            return connections;
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
    public double distance(Genome g2) {
        Genome g1 = this;

        int highestInnovationGene1 = g1.getConnections[g1.getConnections.Count - 1].InnovationNumber;
        int highestInnovationGene2 = g2.getConnections[g2.getConnections.Count - 1].InnovationNumber;

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
    public static Genome crossOver(Genome g1, Genome g2) {
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

    public void mutate() {

    }
}
