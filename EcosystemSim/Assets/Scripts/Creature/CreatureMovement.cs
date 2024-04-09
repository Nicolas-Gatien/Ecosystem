using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMovement
{
    // FIELDS
    private float moveSpeed;
    private float turnSpeed;

    private Transform rotationTransform;
    private Rigidbody2D rb;

    public CreatureMovement(float moveSpeed, float turnSpeed, Transform rotationTransform, Rigidbody2D rb)
    {
        this.moveSpeed = moveSpeed;
        this.turnSpeed = turnSpeed;
        this.rotationTransform = rotationTransform.transform;
        this.rb = rb;

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

        rb.velocity = rotationTransform.right * confidence * moveSpeed;
    }


    public Vector2 GetRiseOverRun()
    {
        return rotationTransform.right;
    }

    public float GetDirection()
    {
        return rotationTransform.rotation.z;
    }

}
