using System;
using System.Collections.Generic;
using UnityEngine;

public enum Deceleration { slow = 3, average = 2, fast = 1 }
public enum TargetPlane { XY, XZ, YZ, XYZ }

public class Steering : MonoBehaviour
{
    private Agent owner;
    private Type ownerType;

    public Agent Owner { get { return owner; } }

    public List<Agent> NearEntities;
    private List<Entity> buffer;

    public TargetPlane targetPlane = TargetPlane.XZ;

    public List<Behaviour> behaviours;
    public SteeringSettings settings;

    public float radius = 3f;

    private AABB range;
    private Smoother smoother;
    private int smootherSampleSize = 20;


    protected virtual void Awake()
    {
        behaviours = new List<Behaviour>();
        owner = GetComponent<Agent>();
        ownerType = owner.GetType();

        smoother = new Smoother(smootherSampleSize);

        range = new AABB(transform.position, Vector3.one * radius);

        NearEntities = new List<Agent>();
        buffer = new List<Entity>();
    }

    private void FixedUpdate()
    {
        buffer.Clear();
        NearEntities.Clear();
        GameWorld.Instance.Octree.Query(range, buffer);

        range.center = transform.position;

        foreach (Entity entity in buffer)
        {
            if (!ReferenceEquals(entity.GetType(), ownerType))
                continue;
            if(Vector3.Distance(Owner.Position, entity.Position) < radius)
                NearEntities.Add((Agent)entity);
        }

        NearEntities.Remove(Owner);
    }

    public MovingEntity GetNearestEntity()
    {
        if (NearEntities.Count == 0)
            return null;
        MovingEntity nearest = NearEntities[0];
        float lowestDist = Vector3.Distance(owner.transform.position, nearest.transform.position);
        for (int i = 0; i < NearEntities.Count; i++)
        {
            Vector3 a = nearest.transform.position;
            Vector3 b = NearEntities[i].transform.position;
            float currentDist = Vector3.Distance(owner.transform.position, b);
            if (currentDist < lowestDist)
            {
                nearest = NearEntities[i];
                lowestDist = currentDist;
            }
        }
        return nearest;
    }

    public Vector3 getSteeringForce()
    {
        Vector3 average = Vector3.zero;

        // Cache behaviours so that the list can be modified during a calculation.
        List<Behaviour> cache = new List<Behaviour>();
        cache.AddRange(behaviours);

        foreach (Behaviour b in cache)
        {
            
            Vector3 accumulitative = b.Calculate(this, settings);
            if (b.pr == 0f)
            {
                Debug.LogWarning("Probability has to be higher than 0.0f");
                continue;
            }

            average += accumulitative;
        }

        average = smoother.Update(average);

        return average;
    }

    public Vector3 GetUsefulTarget(Vector3 target)
    {
        switch (targetPlane)
        {
            case TargetPlane.XY:
                target.z = 0f;
                break;
            case TargetPlane.XZ:
                target.y = 0f;
                break;
            case TargetPlane.YZ:
                target.x = 0f;
                break;
            case TargetPlane.XYZ:
            default:
                break;
        }
        return target;
    }

    //target based steeringbehaviours
    public Transform TargetTrans { get; set; }
    public Vector3 TargetPos { get; set; }

    public void SeekOn(Transform targetTrans)
    {
        if (!behaviours.Contains(Seek.GetInstance()))
            behaviours.Add(Seek.GetInstance());
        TargetTrans = targetTrans;
    }
    public void SeekOn(Vector3 targetPos)
    {
        if (!behaviours.Contains(Seek.GetInstance()))
            behaviours.Add(Seek.GetInstance());
        TargetTrans = null;
        TargetPos = targetPos;
    }
    public void SeekOff()
    {
        behaviours.Remove(Seek.GetInstance());
    }

    public void ArriveOn(Transform targetTrans)
    {
        if (!behaviours.Contains(Arrive.GetInstance())) 
            behaviours.Add(Arrive.GetInstance());

        TargetTrans = targetTrans;
    }
    public void ArriveOn(Vector3 targetPos)
    {
        if (!behaviours.Contains(Arrive.GetInstance()))
            behaviours.Add(Arrive.GetInstance());

        TargetTrans = null;
        TargetPos = targetPos;
    }
    public void ArriveOff()
    {
        behaviours.Remove(Arrive.GetInstance());
    }

    public void PursuitOn(Transform targetTrans)
    {
        if (!behaviours.Contains(Pursuit.GetInstance()))
            behaviours.Add(Pursuit.GetInstance());
        TargetTrans = targetTrans;
    }
    public void PursuitOn(Vector3 targetPos)
    {
        if (!behaviours.Contains(Pursuit.GetInstance()))
            behaviours.Add(Pursuit.GetInstance());
        TargetTrans = null;
        TargetPos = targetPos;
    }
    public void PursuitOff()
    {
        behaviours.Remove(Pursuit.GetInstance());
    }

    //threat based steeringbehaviours
    public Transform ThreatTrans { get; set; }
    public Vector3 ThreatPos { get; set; }

    public void FleeOn(Transform threatTrans)
    { 
        if (!behaviours.Contains(Flee.GetInstance()))
            behaviours.Add(Flee.GetInstance());
        ThreatTrans = threatTrans;
    }
    public void FleeOn(Vector3 threatPos)
    {
        if (!behaviours.Contains(Flee.GetInstance()))
            behaviours.Add(Flee.GetInstance());
        ThreatTrans = null;
        ThreatPos = threatPos;
    }
    public void FleeOff()
    {
        behaviours.Remove(Flee.GetInstance());
    }

    public void EvadeOn(Transform threatTrans)
    {
        if (!behaviours.Contains(Evade.GetInstance()))
            behaviours.Add(Evade.GetInstance());
        ThreatTrans = threatTrans;
    }
    public void EvadeOn(Vector3 threatPos)
    {
        if (!behaviours.Contains(Evade.GetInstance()))
            behaviours.Add(Evade.GetInstance());
        ThreatTrans = null;
        ThreatPos = threatPos;
    }
    public void EvadeOff()
    {
        behaviours.Remove(Evade.GetInstance());
    }

    //void based steeringbehaviours
    public void ObstacleAvoidenceOn()
    {
        if (!behaviours.Contains(ObstacleAvoidance.GetInstance()))
            behaviours.Add(ObstacleAvoidance.GetInstance());
    }
    public void ObstacleAvoidenceOff()
    {
        behaviours.Remove(ObstacleAvoidance.GetInstance());
    }

    public void WanderOn()
    {
        if (!behaviours.Contains(Wander.GetInstance()))
            behaviours.Add(Wander.GetInstance());
    }
    public void WanderOff()
    {
        behaviours.Remove(Wander.GetInstance());
    }

    public void StopOn()
    {
        if (!behaviours.Contains(Stop.GetInstance()))
            behaviours.Add(Stop.GetInstance());
    }
    public void StopOff()
    {
        behaviours.Remove(Stop.GetInstance());
    }

    //group based steeringbehaviours
    public void AlignmentOn()
    {
        if (!behaviours.Contains(Alignment.GetInstance()))
            behaviours.Add(Alignment.GetInstance());
    }
    public void AlignmentOff()
    {
        behaviours.Remove(Alignment.GetInstance());
    }

    public void CohesionOn()
    {
        if (!behaviours.Contains(Cohesion.GetInstance()))
            behaviours.Add(Cohesion.GetInstance());
    }
    public void CohesionOff()
    {
        behaviours.Remove(Cohesion.GetInstance());
    }

    public void SeperationOn()
    {
        if (!behaviours.Contains(Separation.GetInstance()))
            behaviours.Add(Separation.GetInstance());
    }
    public void SeperationOff()
    {
        behaviours.Remove(Separation.GetInstance());
    }

    // * * * * Path Following * * * * //
    public Path path { get; private set; }
    private Vector3 goal;
    private bool requestSend;

    public void PathFollowOn(Vector3 goal)
    {
        if(this.goal != goal && (path == null || path != null && path.IsFinished))
        {
            if (requestSend)
                return;
            // Request new path
            this.goal = goal;
            PathRequester.Instance.RequestPath(GameWorld.Instance.NavGraph, owner.transform.position, goal, OnPathReceived);
            requestSend = true;
        }
    }

    public void PathFollowOff()
    {
        behaviours.Remove(PathFollow.GetInstance());
    }

    private void OnPathReceived(Path path)
    {
        if (!behaviours.Contains(PathFollow.GetInstance()))
            behaviours.Add(PathFollow.GetInstance());

        requestSend = false;
        this.path = path;
        this.path.Simplify();
        this.path.SmoothQuick();
    }

    private void OnDrawGizmos()
    {
        if(path != null)
        {
            path.DrawGizmos();
        }
        if (range != null)
            range.DrawGizmos(Color.cyan);
    }
}