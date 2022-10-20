using System;

public class Timer
{
    public float maxValue;
    public float currentValue;

    public Timer(float maxValue)
    {
        this.maxValue = maxValue;
        currentValue = maxValue;
    }

    public void Tick(float value)
    {
        currentValue -= value;
    }

    public bool HasReachedZero()
    {
        if (currentValue <= 0)
        {
            return true;
        }

        return false;
    }

    public void Reset()
    {
        currentValue = maxValue;
    }
}
