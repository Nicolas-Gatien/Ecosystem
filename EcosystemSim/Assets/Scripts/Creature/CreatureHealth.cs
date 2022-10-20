using System;
using UnityEngine;

public class CreatureHealth
{
    private float maxHp;
    private float hp;
    private float regenerationStrength;

    private Creature creature;

    private Timer recoveryTimer;
    private Timer damageTimer;

    public float health
    {
        set
        {
            if (value > maxHp)
            {
                hp = maxHp;
            }
            else
            {
                hp = value;
                if (hp <= 0)
                {
                    creature.Die();
                }
            }
        }
        get
        {
            return hp;
        }
    }

    public CreatureHealth(float maxHealth, float timeBeforeHealing, float timeBtwDamage, float regenStrength, Creature creature)
    {
        this.maxHp = maxHealth;
        this.regenerationStrength = regenStrength;
        hp = maxHealth;

        this.creature = creature;

        recoveryTimer = new Timer(timeBeforeHealing);
        damageTimer = new Timer(timeBtwDamage);
    }

    public float PercentageLeft()
    {
        return hp / maxHp;
    }

    public void TickTimers(float tick)
    {
        recoveryTimer.Tick(tick);
        damageTimer.Tick(tick);
    }

    public void Update()
    {
        if (damageTimer.HasReachedZero() == false)
        {
            return;
        }

        if (creature.energy > 0)
        {
            if (recoveryTimer.HasReachedZero())
            {
                health += regenerationStrength;
            }
        }
        else
        {
            health -= 1;
            recoveryTimer.Reset();
        }
        damageTimer.Reset();
    }
}

