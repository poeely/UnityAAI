using System.Collections;
using System.Collections.Generic;

using UnityEngine;


// Author: Vincent J.S. Versnel

/// <summary>
/// Defines the behaviour of an Ogre.
/// </summary>
public class Ogre : AgentAnim, IHerdeEntity
{
    ////ScriptLoader doesnt work in the build of the simulation
    //public static Dictionary<string, State<Ogre>> states;
    private StateMachine<Ogre> stateMachine;

    private Herde herde;
    private Region currentRegion;

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine<Ogre>(this);

        ////ScriptLoader doesnt work in build of the simulation
        //if(states == null)
        //    states = new Dictionary<string, State<Ogre>>();

        //    ScriptLoader.LoadScript<Ogre>("GoToNewRegionOgreState", states);
        //    ScriptLoader.LoadScript<Ogre>("GlobalOgreState", states);
        //    ScriptLoader.LoadScript<Ogre>("IdleState", states);

        // Register this entity for messenging
        EntityMessenger.RegisterEntity(ID);
    }

    // Initialize state in Start (Called after all objects are Awake)
    private void Start()
    {
        stateMachine.SetGlobalState(GlobalOgreState.GetInstance());
        stateMachine.ChangeState(GoToNewRegionOgreState.GetInstance());
    }

    public void Init(Herde herde)
    {
        this.herde = herde;
    }

    // Uses fuzzy logic to find the most promising region (region with highest desirability).
    public void FindPromisingRegion()
    {
        Region[,] regions = GameWorld.Instance.Regions;
        Region bestSoFar = null;
        double bestScore = 0f;

        for (int x = 0; x < regions.GetLength(0); x++)
        {
            for (int y = 0; y < regions.GetLength(1); y++)
            {
                float dist = Vector3.Distance(Position, regions[x, y].bounding.center);
                double score = regions[x, y].GetDesirability(dist);
                if(score > bestScore || bestSoFar == null)
                {
                    bestScore = score;
                    bestSoFar = regions[x, y];
                }
            }
        }

        currentRegion = bestSoFar;
    }

    public void ResetRegion()
    {
        currentRegion = null;
    }

    protected void Update()
    {
        stateMachine.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        stateMachine.FixedUpdate();
    }

    protected override void ApplyVelocity()
    {
        base.ApplyVelocity(); 
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
            stateMachine.DrawGizmos();
    }

    public StateMachine<Ogre> GetFSM { get { return stateMachine; } }
    public Region GetCurrentRegion { get { return currentRegion; } }
}

public class GlobalOgreState : State<Ogre>
{
    private static GlobalOgreState instance;
    public static GlobalOgreState GetInstance()
    {
        if (instance == null)
            instance = new GlobalOgreState();
        return instance;
    }

    public override void FixedUpdate(Ogre owner)
    {
        base.FixedUpdate(owner);

        if (owner.GetCurrentRegion != null)
        {
            if (owner.GetCurrentRegion.resourceCount <= 0)
            {
                owner.GetFSM.ChangeState(GoToNewRegionOgreState.GetInstance());
            }
        }
    }
}

public class IdleState : State<Ogre>
{
    private static IdleState instance;
    public static IdleState GetInstance()
    {
        if (instance == null)
            instance = new IdleState();
        return instance;
    }

    public override void Enter(Ogre owner)
    {
        base.Enter(owner);

        owner.Stop();
        owner.GetCurrentRegion.StartLeeching();

        MessageEventArgs e = new MessageEventArgs();
        e.Sender = owner;
        e.Message = Region.ARRIVED_AT_REGION;
        EntityMessenger.SendMessage(e);
        Debug.Log("[OGRE]: Idle.");
    }
}

public class GoToNewRegionOgreState : State<Ogre>
{
    private static GoToNewRegionOgreState instance;
    public static GoToNewRegionOgreState GetInstance()
    {
        if (instance == null)
            instance = new GoToNewRegionOgreState();
        return instance;
    }

    public override void Enter(Ogre owner)
    {
        base.Enter(owner);
        Debug.Log("Enters GoToNewRegion...");
        owner.ResetRegion();
        owner.FindPromisingRegion();
        Vector3 regionCenter = owner.GetCurrentRegion.bounding.center;
        owner.GetSteering.PathFollowOn(regionCenter);

        MessageEventArgs e = new MessageEventArgs();
        e.Sender = owner;
        e.Message = Region.FOUND_NEW_REGION;
        EntityMessenger.SendMessage(e);
    }

    public override void FixedUpdate(Ogre owner)
    {
        base.FixedUpdate(owner);

        if (owner.GetSteering.path != null && owner.GetSteering.path.IsFinished)
            owner.GetFSM.ChangeState(IdleState.GetInstance());
    }

    public override void Exit(Ogre owner)
    {
        base.Exit(owner);
        Debug.Log("Exit GoToNewRegion...");
        owner.GetSteering.PathFollowOff();
    }
}
