using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // TRAITS
    [Header("Traits")]
    public float maxEnergy;
    public float energy;

    public float maxHealth;
    public float health;

    // COMPONENTS
    private CreatureMovement movement;
    private Node movementNode;
    private Node turningNode;

    // BRAIN
    public Genome genes;

    private void Start()
    {
        movement = GetComponent<CreatureMovement>();

        List<Node> nodes = new List<Node>
        {
            new InputEnergyNode(this),
            new InputHealthNode(this),
            new OutputMovementNode(),
            new OutputTurningNode()
        };

        movementNode = nodes[2];
        turningNode = nodes[3];

        ConnectionGene[] connections = new ConnectionGene[4]
        {
            new ConnectionGene(nodes[0], nodes[2]),
            new ConnectionGene(nodes[1], nodes[2]),
            new ConnectionGene(nodes[0], nodes[3]),
            new ConnectionGene(nodes[1], nodes[3])
        };

        genes = new Genome(nodes.ToArray(), connections);
    }

    private void Update()
    {
        
    }
}
