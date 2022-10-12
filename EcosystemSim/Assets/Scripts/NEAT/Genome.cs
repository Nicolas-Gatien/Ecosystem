using System.Collections;
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
        return 0;
    }

    public static Genome crossOver(Genome g1, Genome g2) {
        return null;
    }

    public void mutate() {

    }
}
