using UnityEngine;
using System.Collections;

public class Node
{
    public NodeType type;
    public string id;

    public Node(string id)
    {
        this.id = id;
    }

    public Node()
    {
        this.id = " ";
    }

    public virtual float ProcessInput(float input)
    {
        return input;
    }
}
