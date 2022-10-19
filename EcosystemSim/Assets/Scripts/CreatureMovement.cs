using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMovement : MonoBehaviour
{
    // FIELDS
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float turnSpeed;

    public Transform rotationTransform;

    Rigidbody2D rb;

    // METHODS
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Turn(float confidence)
    {
        rotationTransform.Rotate(0, 0, turnSpeed * confidence);
    }
    public void Move(float confidence)
    {
        if (confidence > 1)
        {
            confidence = 1;
        }

        if (confidence < -1)
        {
            confidence = -1;
        }

        rb.velocity = rotationTransform.right * moveSpeed * confidence;
    }

}
