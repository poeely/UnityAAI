using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzySet_LeftShoulder : FuzzySet
{
    private double peakPoint;
    private double rightOffset;
    private double leftOffset;

    public FuzzySet_LeftShoulder(double peak, double left, double right) : base(((peak - left) + peak) / 2)
    {
        peakPoint = peak;
        leftOffset = left;
        rightOffset = right;
    }

    public override double CalculateDOM(double value)
    {
        if ((rightOffset == 0.0) && (peakPoint == value))
            return 1.0;

        // Calc DOM if value is right of the peak point
        if((value > peakPoint) && (value < (peakPoint + rightOffset)))
        {
            double grad = 1.0 / -rightOffset;
            return grad * (value - peakPoint) + 1.0;
        }

        // If it is less or equal to peak, DOM is 100%
        if ((value <= peakPoint) && (value >= peakPoint - leftOffset))
            return 1.0;

        return 0.0;
    }
}
