using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species
{
    // FIELDS
    private List<Creature> creatures = new List<Creature>();
    private Creature representative;

    // PROPERTY
    public int Size
    {
        get
        {
            return creatures.Count;
        }
    }
    public List<Creature> Creatures
    {
        get
        {
            return creatures;
        }
    }
    public Creature Representative
    {
        get
        {
            return representative;
        }
    }

    // CONSTRUCTOR
    public Species(Creature rep)
    {
        representative = rep;
        representative.Species = this;
        creatures.Add(rep);
    }

    public bool Put(Creature creature)
    {
        // %%% ADD VALUABLE HERE
        if (creature.Distance(representative) < 4)
        {
            creature.Species = this;
            creatures.Add(creature);
            return true;
        }

        return false;
    }
}
