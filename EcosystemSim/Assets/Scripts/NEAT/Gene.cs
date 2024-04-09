using UnityEngine;
using System.Collections;

public class Gene
{
    // FIELDS
    private int innovationNumber;

    // PROPERTIES
    public int InnovationNumber
    {
        get
        {
            return innovationNumber;
        }
        set
        {
            innovationNumber = value;
        }
    }

    // CONSTRUCTOR
    public Gene(int innovationNumber)
    {
        this.innovationNumber = innovationNumber;
    }
    public Gene()
    {

    }


}
