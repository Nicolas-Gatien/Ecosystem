using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // FIELDS
    // traits
    [Header("Traits")]
    public float maxEnergy;
    public float energy;

    public float maxHealth;
    public float health;

    public float size;
    public float moveSpeed;
    public float turnSpeed;
    public float regenerationRate;

    // vision
    public float viewRadius;
    public LayerMask foodMask;
    public LayerMask creatureMask;

    // components
    private CreatureMovement movement;
    private Rigidbody2D rb;

    // ai
    private Genome genome;
    private Species species;
    private Calculator calculator;

    // PROPERTIES
    public Genome Genome
    {
        get
        {
            return genome;
        }
        set
        {
            genome = value;
        }
    }
    public Species Species
    {
        get
        {
            return species;
        }
        set
        {
            species = value;
        }
    }
    public Calculator Calculator
    {
        get
        {
            return calculator;
        }
    }
    
    private float HealthPercentage
    {
        get
        {
            return health / maxHealth;
        }
    }
    private float EnergyPercentage
    {
        get
        {
            return energy / maxEnergy;
        }
    }

    private int NumOfFoodsInRadius
    {
        get
        {
            return Physics2D.OverlapCircleAll(transform.position, viewRadius, foodMask).Length;
        }
    }
    private int NumOfCreaturesInRadius
    {
        get
        {
            return Physics2D.OverlapCircleAll(transform.position, viewRadius, creatureMask).Length;
        }
    }

    private float DistanceToNearestFood
    {
        get
        {
            Collider2D[] foods = Physics2D.OverlapCircleAll(transform.position, viewRadius, foodMask);
            float smallestDistance = 0;
            for (int i = 0; i < foods.Length; i++)
            {
                if (Vector2.Distance(transform.position, foods[i].transform.position) < smallestDistance) 
                {
                    smallestDistance = Vector2.Distance(transform.position, foods[i].transform.position);
                }
            }

            return smallestDistance;
        }
    }
    private float DistanceToNearestCreatures
    {
        get
        {
            Collider2D[] creatures = Physics2D.OverlapCircleAll(transform.position, viewRadius, creatureMask);
            float smallestDistance = 0;
            for (int i = 0; i < creatures.Length; i++)
            {
                if (Vector2.Distance(transform.position, creatures[i].transform.position) < smallestDistance) 
                {
                    smallestDistance = Vector2.Distance(transform.position, creatures[i].transform.position);
                }
            }

            return smallestDistance;
        }
    }

    private float AngleToNearestFood
    {
        get
        {
            Collider2D[] foods = Physics2D.OverlapCircleAll(transform.position, viewRadius, foodMask);
            float smallestDistance = 0;
            int indexOfNearest = 0;
            for (int i = 0; i < foods.Length; i++)
            {
                if (Vector2.Distance(transform.position, foods[i].transform.position) < smallestDistance)
                {
                    smallestDistance = Vector2.Distance(transform.position, foods[i].transform.position);
                    indexOfNearest = i;
                }
            }


            return Vector2.Angle(transform.position, foods[indexOfNearest].transform.position);
        }
    }
    private float AngleToNearestCreature
    {
        get
        {
            Collider2D[] creatures = Physics2D.OverlapCircleAll(transform.position, viewRadius, creatureMask);
            float smallestDistance = 0;
            int indexOfNearest = 0;
            for (int i = 0; i < creatures.Length; i++)
            {
                if (Vector2.Distance(transform.position, creatures[i].transform.position) < smallestDistance)
                {
                    smallestDistance = Vector2.Distance(transform.position, creatures[i].transform.position);
                    indexOfNearest = i;
                }
            }


            return Vector2.Angle(transform.position, creatures[indexOfNearest].transform.position);
        }
    }

    // METHODS
    private void Start()
    {
        movement = GetComponent<CreatureMovement>();
        rb = GetComponent<Rigidbody2D>();

        movement.moveSpeed = moveSpeed;
        movement.turnSpeed = turnSpeed;
    }

    private void Update()
    {
        double[] outputs = calculator.Calculate(
            NumOfFoodsInRadius, 
            NumOfCreaturesInRadius, 
            DistanceToNearestFood, 
            DistanceToNearestCreatures, 
            AngleToNearestFood, 
            AngleToNearestCreature, 
            HealthPercentage, 
            EnergyPercentage, 
            1
        );
    }

    public double Distance(Creature other)
    {
        return genome.Distance(other.Genome);
    }
    public void Mutate()
    {
        genome.Mutate();
    }

    private void GenerateCalculator()
    {
        calculator = new Calculator(genome);
    }
    public double[] Calculate(params double[] ar)
    {
        if (calculator == null)
        {
            GenerateCalculator();
        }
        return calculator.Calculate(ar);
    }

}
