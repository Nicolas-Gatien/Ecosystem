using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // FIELDS
    // traits
    [Header("Traits")]
    public float maxEnergy = 500;
    private float _energy;

    public float maxHealth = 50;
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

    public Neat neat;
    [HideInInspector] public bool canBreed;
    public GameObject creatureObject;

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
            return Physics2D.OverlapCircleAll(transform.position, viewRadius, creatureMask).Length - 1;
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
                if (creatures[i].transform == transform)
                {
                    continue;
                }
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

            if (foods.Length > 0)
            {
                return Vector2.Angle(transform.position, foods[indexOfNearest].transform.position);
            }

            return 0;

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
                if (creatures[i].transform == transform)
                {
                    continue;
                }

                if (Vector2.Distance(transform.position, creatures[i].transform.position) < smallestDistance)
                {
                    smallestDistance = Vector2.Distance(transform.position, creatures[i].transform.position);
                    indexOfNearest = i;
                }
            }


            return Vector2.Angle(transform.position, creatures[indexOfNearest].transform.position);
        }
    }

    public float energy
    {
        get
        {
            return _energy;
        }
        set
        {
            if (value < 0)
            {
                _energy = 0;
                return;
            }

            if (value > maxEnergy)
            {
                canBreed = true;
                _energy = maxEnergy;
                return;
            }

            _energy = value;
        }
    }

    // METHODS
    private void Start()
    {
        canBreed = false;

        


        movement = GetComponent<CreatureMovement>();
        rb = GetComponent<Rigidbody2D>();

        energy = maxEnergy;
        health = maxHealth;

        movement.moveSpeed = moveSpeed;
        movement.turnSpeed = turnSpeed;

        transform.localScale = new Vector3(size, size, size);
    }

    private void Update()
    {
        double[] outputs = Calculate(
            EnergyPercentage,
            HealthPercentage,
            NumOfFoodsInRadius,
            NumOfCreaturesInRadius,
            DistanceToNearestFood,
            DistanceToNearestCreatures,
            AngleToNearestFood,
            AngleToNearestCreature,
            1
        );

        movement.Move((float)outputs[0]);
        movement.Turn((float)outputs[1]);

        energy -= (((size * (rb.velocity.sqrMagnitude * 0.1f)) / HealthPercentage) + 1) * Time.deltaTime;

        if (energy > 0)
        {
            health += regenerationRate;
        }else
        {
            health -= 1;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            energy += 50 / size;
            Destroy(collision.gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Creature"))
        {
            if (canBreed == true)
            {
                Genome geno = Instantiate(creatureObject, transform.position, Quaternion.identity).GetComponent<Creature>().Genome;
                Creature creature = collision.gameObject.GetComponent<Creature>();
                //geno = Genome.CrossOver(this.genome, creature.genome);
                geno.Mutate();
            }
            canBreed = false;
        }
    }

}
