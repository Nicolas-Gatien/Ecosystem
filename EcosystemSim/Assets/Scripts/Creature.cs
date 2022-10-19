using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // FIELDS
    [Header("Traits")]
    [Header("Energy Traits")]
    public float maxEnergy = 500;
    private float _energy;

    [Header("Health Traits")]
    public float regenerationRate;
    private float recoveryTime;
    public float startRecoveryTime;
    public float timeBtwDamage;
    private float timeBeforeNextDamage;
    public float maxHealth = 50;
    public float health;

    [Header("Movement Traits")]
    public float baseSpeed;
    private float trueSpeed;
    public float turnSpeed;

    [Header("Vision Traits")]
    public float fieldOfView = 90;
    public float rangeOfView = 5;

    [Header("General Traits")]
    public float size;
    [Range(0, 1)]
    [SerializeField] private float _boneDensity = 1;
    public float maturityAge;
    private float age;

    private float mass;
    public Color color;

    // vision
    public float viewRadius;
    public LayerMask foodMask;
    public LayerMask creatureMask;

    // components
    private CreatureMovement movement;
    private Rigidbody2D rb;
    private SpriteRenderer rend;

    // ai
    private Genome genome;
    private Species species;
    private Calculator calculator;

    public Neat neat;
    [HideInInspector] public bool canBreed;
    public GameObject creatureObject;
    public GameObject foodPrefab;

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

    private float energy
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
                if (age > maturityAge)
                {
                    canBreed = true;
                }
                _energy = maxEnergy;
                return;
            }

            _energy = value;
        }
    }
    public float boneDensity
    {
        get
        {
            return _boneDensity;
        }
        set
        {
            if(value > 1)
            {
                _boneDensity = 1;
                return;
            }
            if (value < 0.1f)
            {
                _boneDensity = 0.1f;
                return;
            }
            _boneDensity = value;
        }
    }

    // METHODS
    private void Start()
    {
        canBreed = false;

        movement = GetComponent<CreatureMovement>();
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();

        energy = maxEnergy / 2;
        health = maxHealth;

        mass = (size * size * size) * boneDensity;
        trueSpeed = (baseSpeed / mass) * size;

        movement.moveSpeed = trueSpeed;
        movement.turnSpeed = turnSpeed;

        transform.localScale = new Vector3(size, size, size);
    }

    private RaycastHit2D GetNearestInLayer(LayerMask mask)
    {
        Vector2 dir = DirectionOfCreature();
        RaycastHit2D[] obj = Physics2D.CircleCastAll(transform.position, fieldOfView, dir, rangeOfView, mask);

        float smallestDistance = 0;
        int nearestIndex = 0;
        for (int i = 0; i < obj.Length; i++)
        {
            if (obj[i].transform == transform)
            {
                continue;
            }
            if (Vector2.Distance(transform.position, obj[i].point) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(transform.position, obj[i].point);
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
        Vector2 nearest = GetNearestInLayer(mask).point;

        return Vector2.Distance(transform.position, nearest);
    }
    private float GetAngleToNearest(LayerMask mask)
    {
        Vector2 nearest = GetNearestInLayer(mask).point;

        return Vector2.Angle(transform.position, nearest);
    }

    private int GetNumInLayer(LayerMask mask)
    {
        Vector2 dir = DirectionOfCreature();


        if (mask == gameObject.layer)
        {
            return Physics2D.CircleCastAll(transform.position, fieldOfView, dir, rangeOfView, mask).Length - 1;
        }

        return Physics2D.CircleCastAll(transform.position, fieldOfView,dir, rangeOfView, mask).Length;
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
            rb.velocity.sqrMagnitude,
            1
        );

        rb.mass = mass;
        rend.sortingOrder = -(int)(transform.position.y * 5);

        rend.color = color;

        movement.Move((float)outputs[0]);
        movement.Turn((float)outputs[1]);

        age += Time.deltaTime;
        recoveryTime -= Time.deltaTime;
        timeBeforeNextDamage -= Time.deltaTime;

        energy -= (((mass * (rb.velocity.sqrMagnitude * 0.1f) * boneDensity) / PercentageLeft(health, maxHealth)) + 1) * Time.deltaTime;

        if (timeBeforeNextDamage <= 0)
        {
            if (energy > 0)
            {
                if (recoveryTime <= 0)
                {
                    health += regenerationRate;
                }
            }
            else
            {
                health -= 1 / boneDensity;
                recoveryTime = startRecoveryTime;
            }
            timeBeforeNextDamage = timeBtwDamage;
        }

        if (health <= 0)
        {
            Die();
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    void Die()
    {
        int foods = (int)(mass / 2) + 1;
        for (int i = 0; i < foods; i++)
        {
            Instantiate(foodPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public double Distance(Creature other)
    {
        return genome.Distance(other.Genome);
    }
    public void Mutate()
    {
        genome.Mutate(this);
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
        if (collision.gameObject.CompareTag("Food"))
        {
            energy += 50 / mass;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Creature"))
        {
            Creature other = collision.gameObject.GetComponent<Creature>();
            if (canBreed == true && other.canBreed == true)
            {
                canBreed = false;
                energy -= 50;

                int babies = UnityEngine.Random.Range(0, 3) + 1;

                for (int i = 0; i < babies; i++)
                {
                    if (UnityEngine.Random.Range(0f,1f) < 0.5)
                    {
                        if (UnityEngine.Random.Range(0f, 1f) < 0.5)
                        {
                            SpawnOffSpringBasedOnParent(this);
                        } else
                        {
                            SpawnOffSpringBasedOnParent(other);
                        }
                    }else
                    {
                        SpawnOffSprintBasedOnBothParents(other);
                    }
                }

            }
        }
    }

    void SpawnOffSpringBasedOnParent(Creature parent)
    {
        Creature child = Instantiate(creatureObject, transform.position, Quaternion.identity).GetComponent<Creature>();
        Genome childGenes = parent.Genome;
        child.Genome = childGenes;
        child.color = parent.color;
        child.size = parent.size;
        child.trueSpeed = parent.trueSpeed;
        child.turnSpeed = parent.turnSpeed;
        child.maxEnergy = parent.maxEnergy;
        child.maxHealth = parent.maxHealth;
        child.regenerationRate = parent.regenerationRate;
        child.Mutate();
    }

    void SpawnOffSprintBasedOnBothParents(Creature other)
    {
        Creature child1 = Instantiate(creatureObject, transform.position, Quaternion.identity).GetComponent<Creature>();
        Genome childGenes1 = genome.CrossOver(this.Genome, other.Genome);
        child1.Genome = childGenes1;
        child1.color = new Color((color.r + other.color.r) / 2, (color.g + other.color.g) / 2, (color.b + other.color.b) / 2);
        child1.size = (size + other.size) / 2;
        child1.trueSpeed = (trueSpeed + other.trueSpeed) / 2;
        child1.turnSpeed = (turnSpeed + other.turnSpeed) / 2;
        child1.maxEnergy = (maxEnergy + other.maxEnergy) / 2;
        child1.maxHealth = (maxHealth + other.maxHealth) / 2;
        child1.regenerationRate = (regenerationRate + other.regenerationRate) / 2;
        child1.Mutate();
    }

    Vector2 DirectionOfCreature()
    {
        return movement.rotationTransform.right;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, DirectionOfCreature());
    }
}