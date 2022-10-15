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
    float timeLeft;

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

    private Collider2D GetNearestInLayer(LayerMask mask)
    {
        Collider2D[] obj = Physics2D.OverlapCircleAll(transform.position, viewRadius, mask);
        if (obj.Length == 0)
        {
            return null;
        }

        float smallestDistance = 0;
        int nearestIndex = 0;
        for (int i = 0; i < obj.Length; i++)
        {
            if (obj[i].transform == transform)
            {
                continue;
            }
            if (Vector2.Distance(transform.position, obj[i].transform.position) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(transform.position, obj[i].transform.position);
                nearestIndex = i;
            }
        }

        return obj[nearestIndex];
    }

    private float PercentageLeft(float current, float max)
    {
        return current / max;
    }
    private float GetDistanceToNearest(LayerMask mask)
    {
        if (GetNearestInLayer(mask) == null)
        {
            return 0;
        }
        Transform nearest = GetNearestInLayer(mask).transform;

        return Vector2.Distance(transform.position, nearest.position);
    }    
    private float GetAngleToNearest(LayerMask mask)
    {
        if (GetNearestInLayer(mask) == null)
        {
            return 0;
        }
        Transform nearest = GetNearestInLayer(mask).transform;

        return Vector2.Angle(transform.position, nearest.position);
    }

    private int GetNumInLayer(LayerMask mask)
    {
        if (mask == gameObject.layer)
        {
            return Physics2D.OverlapCircleAll(transform.position, viewRadius, mask).Length - 1;
        }

        return Physics2D.OverlapCircleAll(transform.position, viewRadius, mask).Length;
    }

    private void Update()
    {
        double[] outputs = Calculate(
            PercentageLeft(health, maxHealth),
            PercentageLeft(energy, maxEnergy),
            GetNumInLayer(foodMask),
            GetNumInLayer(creatureMask),
            GetDistanceToNearest(foodMask),
            GetDistanceToNearest(creatureMask),
            GetAngleToNearest(foodMask),
            GetAngleToNearest(creatureMask),
            1
        );

        movement.Move((float)outputs[0]);
        movement.Turn((float)outputs[1]);

        timeLeft += Time.deltaTime;
        energy -= (((size * (rb.velocity.sqrMagnitude * 0.1f)) / PercentageLeft(health, maxHealth)) + 1) * Time.deltaTime;

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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Creature"))
        {
            if (canBreed == true && collision.gameObject.GetComponent<Creature>().canBreed == true)
            {
                canBreed = false;
                energy -= 50;
                Genome geno = Instantiate(creatureObject, transform.position, Quaternion.identity).GetComponent<Creature>().Genome;
                Creature creature = collision.gameObject.GetComponent<Creature>();
                geno = Genome.CrossOver(this.genome, creature.genome);
                geno.Mutate();
                Debug.Log("Offspring Created");
                Destroy(gameObject);
            }
        }
    }

}
