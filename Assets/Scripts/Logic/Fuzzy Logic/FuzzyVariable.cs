using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyVariable
{
    // Fuzzy sets that comprise this FLV
    private Dictionary<string, FuzzySet> memberSets;

    // Range of this FLV
    private double minRange;
    private double maxRange;

    public FuzzyVariable()
    {
        memberSets = new Dictionary<string, FuzzySet>();
        minRange = maxRange = 0.0;
    }

    private void AdjustRangeToFit(double min, double max)
    {
        if (min < minRange)
            minRange = min;
        if (max > maxRange)
            maxRange = max;
    }

    public FzSet AddLeftShoulderSet(string name, double min, double peak, double max)
    {
        return AddNewSet(name, min, peak, max, new FuzzySet_LeftShoulder(peak, min, max));
    }

    public FzSet AddRightShoulderSet(string name, double min, double peak, double max)
    {
        return AddNewSet(name, min, peak, max, new FuzzySet_RightShoulder(peak, min, max));
    }

    public FzSet AddTriangleSet(string name, double min, double peak, double max)
    {
        return AddNewSet(name, min, peak, max, new FuzzySet_Triangle(peak, min, max));
    }

    private FzSet AddNewSet(string name, double min, double peak, double max, FuzzySet fuzzySet)
    {
        if (memberSets.ContainsKey(name))
        {
            Debug.LogError("[Fuzzy]: FLV already has a member of name " + name);
            return null;
        }

        memberSets.Add(name, fuzzySet);
        AdjustRangeToFit(min, max);
        return new FzSet(fuzzySet);
    }

    public void  Fuzzify(double value)
    {
        if(value < minRange || value > maxRange)
        {
            Debug.LogError("[FUZZY]: Value is out of range.");
            return;
        }

        foreach (KeyValuePair<string, FuzzySet> pair in memberSets)
            pair.Value.SetDOM(pair.Value.CalculateDOM(value));
    }

    public double DefuzzifyMaxAv()
    {
        double bottom = 0.0;
        double top = 0.0;

        foreach (KeyValuePair<string, FuzzySet> pair in memberSets)
        {
            bottom += pair.Value.GetDOM();
            top += pair.Value.GetRepresentativeValue * pair.Value.GetDOM();
        }

        if(bottom == 0.0)
            return 0.0;

        return (top / bottom);
    }
}
