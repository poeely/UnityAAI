﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzySet_RightShoulder : FuzzySet
{
    private double peakPoint;
    private double leftOffset;
    private double rightOffset;

    public FuzzySet_RightShoulder(double peak, double left, double right) : base(((peak + right) + peak) / 2)
    {
        peakPoint = peak;

        leftOffset = left;
    }

    public override double CalculateDOM(double value)
    {
        if ((leftOffset == 0.0) && (peakPoint == value))
            return 1.0;

        // Calc DOM if value is left
        if((value <= peakPoint) && (value > (peakPoint - leftOffset)))
        {
            double grad = 1.0 / leftOffset;
            return grad * (value - (peakPoint - leftOffset));
        }

        if (value > peakPoint)
            return 1.0;

        return 0.0;
    }
}