using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyTerm
{
    public virtual FuzzyTerm Clone()
    {
        return new FuzzyTerm();
    }

    public virtual double GetDOM()
    {
        return 0;
    }

    public virtual void ClearDOM()
    {
        
    }

    public virtual void ORwithDOM(double value)
    {

    }
}
