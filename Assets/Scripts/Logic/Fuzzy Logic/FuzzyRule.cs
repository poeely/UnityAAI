using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyRule
{
    FuzzyTerm antecedent;
    FuzzyTerm consequence;

    public FuzzyRule(FuzzyTerm antecedent, FuzzyTerm consequence)
    {
        this.antecedent = antecedent.Clone();
        this.consequence = consequence.Clone();
    }

    public virtual void ResetDOMOfConsequent()
    {
        consequence.ClearDOM();
    }

    public void Calculate()
    {
        consequence.ORwithDOM(antecedent.GetDOM());
    }
}
