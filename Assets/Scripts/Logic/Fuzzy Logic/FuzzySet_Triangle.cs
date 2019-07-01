using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzySet_Triangle : FuzzySet
{
    private double peakPoint;
    private double leftOffset;
    private double rightOffset;

    public FuzzySet_Triangle(double mid, double left, double right) : base(mid)
    {
        peakPoint = mid;
        leftOffset = left;
        rightOffset = right;
    }

    public override double CalculateDOM(double value)
    {
        // Prevent Divide by Zero errors
        if ((rightOffset == 0.0 || leftOffset == 0.0) && peakPoint == value)
            return 1.0;

        // Calc DOM if left of center
        if((value <= peakPoint) && (value >= (peakPoint - leftOffset)))
        {
            double grad = 1.0 / leftOffset;
            return grad * (value - (peakPoint - leftOffset));
        }

        // Calc DOM if right of center
        if((value > peakPoint) && (value < (peakPoint + rightOffset)))
        {
            double grad = 1.0 / -rightOffset;
            return grad * (value - peakPoint) + 1.0;
        }

        return 0.0;
    }
}
