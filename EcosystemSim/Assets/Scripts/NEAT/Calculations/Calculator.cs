using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator
{
    // FIELDS
    private List<Node> inputNodes = new List<Node>();
    private List<Node> hiddenNodes = new List<Node>();
    private List<Node> outputNodes = new List<Node>();

    // CONSTRUCTOR
    public Calculator(Genome g)
    {
        List<NodeGene> nodes = g.getNodes;
        List<ConnectionGene> connections = g.getConnections;

        IDictionary<int, Node> nodeDict = new Dictionary<int, Node>();

        foreach (NodeGene n in nodes)
        {
            Node node = new Node(n.X);
            nodeDict.Add(n.InnovationNumber, node);

            if (n.X <= 0.1f)
            {
                inputNodes.Add(node);
            }else if (n.X >= 0.9f){
                outputNodes.Add(node);
            }else
            {
                hiddenNodes.Add(node);
            }
        }

        hiddenNodes = hiddenNodes.OrderBy(node => node.X).ToList();

        foreach (ConnectionGene c in connections)
        {
            NodeGene from = c.From;
            NodeGene to = c.To;

            Node nodeFrom = nodeDict[from.InnovationNumber];
            Node nodeTo = nodeDict[to.InnovationNumber];

            Connection con = new Connection(nodeFrom, nodeTo);
            con.Weight = c.Weight;
            con.Enabled = c.Enabled;

            nodeTo.Connections.Add(con);
        }

    }

    public double[] Calculate(params double[] input)
    {
        if (input.Length != inputNodes.Count)
        {
            Debug.LogError("DATA DOES NOT FIT");
        }

        for (int i = 0; i < inputNodes.Count; i++)
        {
            inputNodes[i].Output = input[i];
        }

        foreach (Node n in hiddenNodes)
        {
            n.Calculate();
        }

        double[] outputs = new double[outputNodes.Count];
        for (int i = 0; i < outputNodes.Count; i++)
        {
            outputNodes[i].Calculate();
            outputs[i] = outputNodes[i].Output;
        }

        return outputs;
    }
}
