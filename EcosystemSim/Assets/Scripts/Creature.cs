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

    public float size;
    public float moveSpeed;
    public float turnSpeed;

    // COMPONENTS
    private CreatureMovement movement;
    private Rigidbody2D rb;

    private void Start()
    {
        movement = GetComponent<CreatureMovement>();
        rb = GetComponent<Rigidbody2D>();

        movement.moveSpeed = moveSpeed;
        movement.turnSpeed = turnSpeed;
    }
}
