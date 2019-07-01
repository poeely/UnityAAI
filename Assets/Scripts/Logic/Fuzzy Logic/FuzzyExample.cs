using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyExample : MonoBehaviour
{
    FuzzyModule fm;

    private void Awake()
    {
        fm = new FuzzyModule();
        FuzzyVariable DistToTarget = fm.CreateFLV("DistToTarget");

        FzSet Target_Close = DistToTarget.AddLeftShoulderSet("Target_Close", 0, 25, 150);
        FzSet Target_Medium = DistToTarget.AddTriangleSet("Target_Medium", 25, 50, 300);
        FzSet Target_Far = DistToTarget.AddRightShoulderSet("Target_Far", 150, 300, 500);

        FuzzyVariable AmmoStatus = fm.CreateFLV("AmmoStatus");

        FzSet Ammo_Low = AmmoStatus.AddLeftShoulderSet("Ammo_Low", 0, 0, 10);
        FzSet Ammo_Okay = AmmoStatus.AddTriangleSet("Ammo_Okay", 0, 10, 30);
        FzSet Ammo_Loads = AmmoStatus.AddRightShoulderSet("Ammo_Loads", 10, 30, 40);

        FuzzyVariable Desirability = fm.CreateFLV("Desirablility");

        FzSet Undersirable = Desirability.AddLeftShoulderSet("Undesirable", 0, 25, 50);
        FzSet Desirable = Desirability.AddTriangleSet("Desirable", 25, 50, 75);
        FzSet VeryDesirable = Desirability.AddRightShoulderSet("VeryDesirable", 50, 75, 100);

        fm.AddRule(new FzAND(Target_Close, Ammo_Low), Undersirable);
        fm.AddRule(new FzAND(Target_Close, Ammo_Okay), Undersirable);
        fm.AddRule(new FzAND(Target_Close, Ammo_Loads), Undersirable);
        fm.AddRule(new FzAND(Target_Medium, Ammo_Low), Desirable);
        fm.AddRule(new FzAND(Target_Medium, Ammo_Okay), VeryDesirable);
        fm.AddRule(new FzAND(Target_Medium, Ammo_Loads), VeryDesirable);
        fm.AddRule(new FzAND(Target_Far, Ammo_Low), Undersirable);
        fm.AddRule(new FzAND(Target_Far, Ammo_Okay), Desirable);
        fm.AddRule(new FzAND(Target_Far, Ammo_Loads), Desirable);

        
    }
}
