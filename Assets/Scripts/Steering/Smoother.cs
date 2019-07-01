using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoother
{
    private Vector3[] history;
    private int iNextUpdateSlot;

    public Smoother(int SampleSize)
    {
        history = new Vector3[SampleSize];
        iNextUpdateSlot = 0;
    }

    public Vector3 Update(Vector3 MostRecentValue)
    {
        if (iNextUpdateSlot >= (history.Length - 1))
            iNextUpdateSlot = 0;

        history[iNextUpdateSlot++] = MostRecentValue;

        Vector3 sum = Vector3.zero;
        
        foreach(Vector3 v in history)
        {
            sum += v;
        }

        return sum / history.Length;
    }
}
