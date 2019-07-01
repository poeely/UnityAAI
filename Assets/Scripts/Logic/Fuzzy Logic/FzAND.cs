

// Atomic FuzzyTerm 
public class FzAND : FuzzyTerm
{
    private FuzzyTerm[] terms;

    public FzAND(params FuzzyTerm[] _terms)
    {
        terms = new FuzzyTerm[_terms.Length];
        for (int i = 0; i < _terms.Length; i++)
            terms[i] = _terms[i].Clone();
    }

    public override FuzzyTerm Clone()
    {
        return new FzAND(terms);
    }

    public override double GetDOM()
    {
        // Start at max double. (in the book he says it starts at 0...)
        double minDom = double.MaxValue;

        foreach (FuzzyTerm t in terms)
            if (t.GetDOM() < minDom)
                minDom = t.GetDOM();

        return minDom;
    }

    public override void ClearDOM()
    {
        foreach (FuzzyTerm t in terms)
            t.ClearDOM();
    }

    public override void ORwithDOM(double value)
    {
        foreach (FuzzyTerm t in terms)
            t.ORwithDOM(value);
    }
}