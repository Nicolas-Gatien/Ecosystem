using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField ] private float lifetime;
    private Timer lifeTimer;

    private SpriteRenderer rend;

    private void Start()
    {
        lifeTimer = new Timer(lifetime);
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        lifeTimer.Tick(Time.deltaTime);
        rend.sortingOrder = -(int)(transform.position.y * 5);

        if (lifeTimer.HasReachedZero())
        {
            Destroy(gameObject);
        }
    }
}
