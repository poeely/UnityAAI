using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public struct WeaponStats
{
    public int power;
    public int durability;

    public static WeaponStats Random()
    {
        WeaponStats stats = new WeaponStats();
        stats.power = UnityEngine.Random.Range(1, 100);
        stats.durability = UnityEngine.Random.Range(1, 100);
        return stats;
    }
}

public class Weapon : MonoBehaviour
{
    private WeaponStats stats;

    protected FuzzyModule fm;

    private void Awake()
    {
        InitFuzzyModule();

        Init(WeaponStats.Random());
    }

    protected void InitFuzzyModule()
    {
        fm = new FuzzyModule();
        FuzzyVariable DistToTarget = fm.CreateFLV("DistToTarget");

        FzSet Target_Close = DistToTarget.AddLeftShoulderSet("Target_Close", 0, 25, 150);
        FzSet Target_Medium = DistToTarget.AddTriangleSet("Target_Medium", 25, 50, 300);
        FzSet Target_Far = DistToTarget.AddRightShoulderSet("Target_Far", 150, 300, 500);

        FuzzyVariable AmmoStatus = fm.CreateFLV("Durability");

        FzSet Durability_Low = AmmoStatus.AddLeftShoulderSet("Durability_Low", 0, 0, 10);
        FzSet Durability_Okay = AmmoStatus.AddTriangleSet("Durability_Okay", 0, 10, 30);
        FzSet Durability_Loads = AmmoStatus.AddRightShoulderSet("Durability_Loads", 10, 30, 40);

        FuzzyVariable Desirability = fm.CreateFLV("Desirability");

        FzSet Undersirable = Desirability.AddLeftShoulderSet("Undesirable", 0, 25, 50);
        FzSet Desirable = Desirability.AddTriangleSet("Desirable", 25, 50, 75);
        FzSet VeryDesirable = Desirability.AddRightShoulderSet("VeryDesirable", 50, 75, 100);

        fm.AddRule(new FzAND(Target_Close, Durability_Low), Undersirable);
        fm.AddRule(new FzAND(Target_Close, Durability_Okay), Undersirable);
        fm.AddRule(new FzAND(Target_Close, Durability_Loads), Undersirable);
        fm.AddRule(new FzAND(Target_Medium, Durability_Low), Desirable);
        fm.AddRule(new FzAND(Target_Medium, Durability_Okay), VeryDesirable);
        fm.AddRule(new FzAND(Target_Medium, Durability_Loads), VeryDesirable);
        fm.AddRule(new FzAND(Target_Far, Durability_Low), Undersirable);
        fm.AddRule(new FzAND(Target_Far, Durability_Okay), Desirable);
        fm.AddRule(new FzAND(Target_Far, Durability_Loads), Desirable);
    }

    public void Init(WeaponStats stats)
    {
        this.stats = stats;
    }

    public virtual double GetDesirability(float distToTarget)
    {
        fm.Fuzzify("DistToTarget", distToTarget);
        fm.Fuzzify("Durability", stats.durability);
        return fm.DeFuzzify("Desirability");
    }
}
