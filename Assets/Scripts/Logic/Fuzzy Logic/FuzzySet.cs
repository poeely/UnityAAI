using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzySet
{
    // Each set stores the DOM of the value to be fuzzified.
    protected double dom;
    protected double representativeValue;

    public FuzzySet(double representativeValue)
    {
        dom = 0.0;
        this.representativeValue = representativeValue;
    }
	
    public virtual double CalculateDOM(double value)
    {
        return 0.0;
    }

    public void ORwithDOM(double value)
    {  
        if (value > dom)
            dom = value;
    }

    public void ClearDOM()
    {
        dom = 0.0;
    }

    public void SetDOM(double value)
    {
        dom = value;
    }

    public double GetDOM()
    {
        return dom;
    }

    public double GetRepresentativeValue { get { return representativeValue; } }
    
}
