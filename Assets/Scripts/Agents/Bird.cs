using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : Agent
{
    public AABB aabb;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GetSteering.WanderOn();
        GetSteering.SeperationOn();
        GetSteering.CohesionOn();
        GetSteering.AlignmentOn();

        velocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }

    private void Update()
    {
        GetSteering.settings.weightAlignment = BirdManager.instance.alignment;
        GetSteering.settings.weightCohesion = BirdManager.instance.cohesion;
        GetSteering.settings.weightSeparation = BirdManager.instance.separation;

        maxSpeed = BirdManager.instance.speed;

        if (!aabb.Contains(Position))
            velocity *= -1f;
    }

}
