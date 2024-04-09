using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // FIELDS
    [Header("Traits")]
    [Space]
    public float maxHealth = 50;
    public float recoveryTime;
    public float regenerationStrength;
    public float timeBtwDamage;
    [Space]
    public float baseSpeed;
    public float turnSpeed;
    [Space]
    public float fieldOfView = 90;
    public float rangeOfView = 5;
    [Space]
    public float maxSize;
    public float maturityAge;
    [Space]
    public float maxEnergy = 500;
    [Space]
    public Color color;
    [Space]
    public float stomacheSize;
    public float digestionSpeed;
    [Space]
    public Transform rotationTransform;

    private float _energy;
    private float trueSpeed;
    private float age;
    private float mass;
    public LayerMask foodMask;
    public LayerMask creatureMask;

    private CreatureMovement movement;
    private CreatureHealth health;

    private Rigidbody2D rb;
    private SpriteRenderer rend;
    private Genome genome;
    private Species species;
    private Calculator calculator;
    public Neat neat;
    public bool canBreed;
    public GameObject creatureObject;
    public GameObject foodPrefab;
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


    // METHODS
    private void Start()
    {
        canBreed = false;

        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();

        energy = maxEnergy / 2;

        mass = (maxSize * maxSize * maxSize);
        trueSpeed = (baseSpeed / mass) * maxSize;

        rb.mass = mass;

        transform.localScale = new Vector3(maxSize, maxSize, maxSize);

        movement = new CreatureMovement(trueSpeed, turnSpeed, rotationTransform, rb);
        health = new CreatureHealth(maxHealth, recoveryTime, timeBtwDamage, regenerationStrength, this);
    }

    private void Update()
    {
        double[] outputs = Calculate(
            health.PercentageLeft(),
            PercentageLeft(energy, maxEnergy),
            GetNumInLayer(foodMask),
            GetNumInLayer(creatureMask),
            GetDistanceToNearest(foodMask),
            GetDistanceToNearest(creatureMask),
            GetAngleToNearest(foodMask),
            GetAngleToNearest(creatureMask),
            rb.velocity.sqrMagnitude,
            age,
            1
        );

        rend.color = color;
        rend.sortingOrder = -(int)(transform.position.y * 5);

        age += Time.deltaTime;
        energy -= (((mass * (rb.velocity.sqrMagnitude * 0.1f)) / health.PercentageLeft()) + 1) * Time.deltaTime;

        movement.Move((float)outputs[0]);
        movement.Turn((float)outputs[1]);

        health.TickTimers(Time.deltaTime);
        health.Update();
    }

    private RaycastHit2D GetNearestInLayer(LayerMask mask)
    {
        Vector2 dir = movement.GetRiseOverRun();
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
        if (GetNearestInLayer(mask) == false)
        {
            return 0;
        }
        Vector2 nearest = GetNearestInLayer(mask).point;

        return Vector2.Distance(transform.position, nearest);
    }
    private float GetAngleToNearest(LayerMask mask)
    {
        if (GetNearestInLayer(mask) == false)
        {
            return 0;
        }
        Vector2 nearest = GetNearestInLayer(mask).point;

        return Vector2.Angle(transform.position, nearest);
    }
    private int GetNumInLayer(LayerMask mask)
    {
        Vector2 dir = movement.GetRiseOverRun();


        if (mask == gameObject.layer)
        {
            return Physics2D.CircleCastAll(transform.position, fieldOfView, dir, rangeOfView, mask).Length - 1;
        }

        return Physics2D.CircleCastAll(transform.position, fieldOfView, dir, rangeOfView, mask).Length;
    }

    public void Die()
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
        child.maxSize = parent.maxSize;
        child.trueSpeed = parent.trueSpeed;
        child.turnSpeed = parent.turnSpeed;
        child.maxEnergy = parent.maxEnergy;
        child.maxHealth = parent.maxHealth;
        child.regenerationStrength = parent.regenerationStrength;
        child.Mutate();
    }

    void SpawnOffSprintBasedOnBothParents(Creature other)
    {
        Creature child1 = Instantiate(creatureObject, transform.position, Quaternion.identity).GetComponent<Creature>();
        Genome childGenes1 = genome.CrossOver(this.Genome, other.Genome);
        child1.Genome = childGenes1;
        child1.color = new Color((color.r + other.color.r) / 2, (color.g + other.color.g) / 2, (color.b + other.color.b) / 2);
        child1.maxSize = (maxSize + other.maxSize) / 2;
        child1.trueSpeed = (trueSpeed + other.trueSpeed) / 2;
        child1.turnSpeed = (turnSpeed + other.turnSpeed) / 2;
        child1.maxEnergy = (maxEnergy + other.maxEnergy) / 2;
        child1.maxHealth = (maxHealth + other.maxHealth) / 2;
        child1.regenerationStrength = (regenerationStrength + other.regenerationStrength) / 2;
        child1.Mutate();
    }
}