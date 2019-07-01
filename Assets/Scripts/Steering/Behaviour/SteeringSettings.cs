using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public struct SteeringSettings
{
    public float prSeek,
        prArrive,
        prFlee,
        prAlignment,
        prCohesion,
        prSeparation,
        prPursuit,
        prEvade,
        prWander,
        prObstacleAvoidence,
        prStop,
        prPathFollow;

    public float weightSeek,
        weightArrive,
        weightFlee,
        weightAlignment,
        weightCohesion,
        weightSeparation,
        weightPursuit,
        weightEvade,
        weightWander,
        weightOvstacleAvoidence,
        weightStop,
        weightPathFollow;

    public LayerMask obstacleLayer;
    public float lerp;
    public float panicDist;
    public Deceleration deceleration;
    public float decelerationRate;
    public Vector3 Offset;
}


