using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Heart of the Fuzzy System. Contains FLV's & rules.
/// It has methods for adding FLV's and rules to the module &
/// for running it.
/// 
/// </summary>
public class FuzzyModule
{
    public enum DefuzzifyType
    {
        MaxAv,
        Centroid
    }

    public const int centroidSampleCount = 15;

    // All FLV's used by this module
    private Dictionary<string, FuzzyVariable> variables;

    // All rules used by this module
    private List<FuzzyRule> rules;

    public FuzzyModule()
    {
        variables = new Dictionary<string, FuzzyVariable>();
        rules = new List<FuzzyRule>();
    }

    // Resets DOM's of the rule consequents
    private void ResetDOMOfConsequents()
    {
        foreach (FuzzyRule r in rules)
            r.ResetDOMOfConsequent();
    }

    // Create & add a new FLV
    public FuzzyVariable CreateFLV(string name)
    {
        variables.Add(name, new FuzzyVariable());

        return variables[name];
    }

    // Create & Add a new rule with given antecedent and consequence
    public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
    {
        rules.Add(new FuzzyRule(antecedent, consequence));
    }

    // Fuzzifies the given value with the desired FLV.
    public void Fuzzify(string nameOfFLV, double value)
    {
        if (!variables.ContainsKey(nameOfFLV))
        {
            Debug.LogError("[FUZZY]: FLV: " + nameOfFLV + " does not exist.");
            return;
        }

        variables[nameOfFLV].Fuzzify(value);
    }

    // Defuzzifies the values that have been fuzzified
    public double DeFuzzify(string nameOfFLV)
    {
        if(!variables.ContainsKey(nameOfFLV))
        {
            Debug.LogError("[FUZZY]: FLV: " + nameOfFLV + " does not exist.");
            return 0.0;
        }

        ResetDOMOfConsequents();

        foreach (FuzzyRule r in rules)
            r.Calculate();

        return variables[nameOfFLV].DefuzzifyMaxAv();
    }

}
