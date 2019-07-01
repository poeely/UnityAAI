using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Proxy class for FuzzySet to be used as a term
public class FzSet : FuzzyTerm
{
    FuzzySet set;

    public FzSet(FuzzySet set)
    {
        this.set = set;
    }

    public override FuzzyTerm Clone()
    {
        return new FzSet(set);
    }

    public override double GetDOM()
    {
        return set.GetDOM();
    }

    public override void ClearDOM()
    {
        set.ClearDOM();
    }

    public override void ORwithDOM(double value)
    {
        set.ORwithDOM(value);
    }
}