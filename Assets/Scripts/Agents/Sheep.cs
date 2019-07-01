using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A moving enitity that describes the behaviour of the sheep
/// 
/// Author: Vincent Versnel
/// </summary>
public class Sheep : Agent, IHerdeEntity
{
    protected StateMachine<Sheep> stateMachine;

    protected int hunger;
    protected int hungerDissatisfier;

    public static Dictionary<string, State<Sheep>> states;

    protected Herde herde;

    protected override void Awake()
    {
        base.Awake();

        EntityMessenger.RegisterEntity(ID);

        hungerDissatisfier = Random.Range(100, 300);

        stateMachine = new StateMachine<Sheep>(this);

        //Script loader Doesnt work in build of the simulation
        //if (states == null)
        //{
        //    states = new Dictionary<string, State<Sheep>>();
        //}

        //ScriptLoader.LoadScript<Sheep>("GroupSheepState", states);
        //ScriptLoader.LoadScript<Sheep>("FleeingSheepState", states);
        //ScriptLoader.LoadScript<Sheep>("GrazingSheepState", states);
        //ScriptLoader.LoadScript<Sheep>("WanderingSheepState", states);
        //ScriptLoader.LoadScript<Sheep>("GlobalSheepState", states);

    }

    private void Start()
    {
        stateMachine.SetCurrentState(WanderingSheepState.GetInstance());
        stateMachine.SetGlobalState(GlobalSheepState.GetInstance());
    }

    public void Init(Herde herde)
    {
        this.herde = herde;

    }

    public override void HandleMessage(MessageEventArgs e)
    {
        base.HandleMessage(e);

        stateMachine.HandleMessage(e);
    }

    protected virtual void Update()
    {
        stateMachine.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        stateMachine.FixedUpdate();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        stateMachine.DrawGizmos();
    }

    public int Hunger { get { return hunger; } set { hunger = value; } }
    public int HungerDissatisfier { get { return hungerDissatisfier; } }

    public StateMachine<Sheep> GetFSM { get { return stateMachine; } }
    public Herde Herde { get { return herde; } }
}

public class GlobalSheepState : State<Sheep>
{
    private static GlobalSheepState instance;
    public static GlobalSheepState GetInstance()
    {
        if (instance == null)
            instance = new GlobalSheepState();
        return instance;
    }

    public override void Enter(Sheep owner)
    {
        base.Enter(owner);
        owner.GetSteering.ObstacleAvoidenceOn();
        owner.GetSteering.ThreatTrans = owner.Herde.Wolf.transform;

        EntityMessenger.SubscribeToEntity(owner.Herde.Ogre, owner);
    }

    public override void FixedUpdate(Sheep owner)
    {
        base.FixedUpdate(owner);

        ++owner.Hunger;

        if (Vector3.Distance(owner.Position, owner.GetSteering.ThreatTrans.position) < owner.GetSteering.settings.panicDist)
        {
            owner.GetFSM.ChangeState(FleeingSheepState.GetInstance());
        }
        else if (owner.Hunger > owner.HungerDissatisfier)
            owner.GetFSM.ChangeState(WanderingSheepState.GetInstance());
    }
}

public class FleeingSheepState : State<Sheep>
{
    private static FleeingSheepState instance;
    public static FleeingSheepState GetInstance()
    {

        if (instance == null)
            instance = new FleeingSheepState();
        return instance;
    }

    public override void Enter(Sheep owner)
    {
        base.Enter(owner);
        owner.GetSteering.FleeOn(owner.Herde.Wolf.transform);
    }

    public override void FixedUpdate(Sheep owner)
    {
        base.FixedUpdate(owner);
    }

    public override void Exit(Sheep owner)
    {
        base.Exit(owner);

        owner.GetSteering.FleeOff();
    }
}

public class WanderingSheepState : State<Sheep>
{
    private static WanderingSheepState instance;
    public static WanderingSheepState GetInstance()
    {
        if (instance == null)
            instance = new WanderingSheepState();
        return instance;


    }

    public override void Enter(Sheep owner)
    {
        base.Enter(owner);

        owner.GetSteering.WanderOn();
    }

    public override void FixedUpdate(Sheep owner)
    {
        base.FixedUpdate(owner);

        MovingEntity entity = owner.GetSteering.GetNearestEntity();
        if (entity == null)
            return;

        float dist = Vector3.Distance(owner.transform.position, entity.transform.position);

        if (dist > 2)
            owner.GetFSM.ChangeState(GroupSheepState.GetInstance());

    }

    public override void Exit(Sheep owner)
    {
        base.Exit(owner);

        owner.GetSteering.WanderOff();
    }
}

public class GroupSheepState : State<Sheep>
{
    private static GroupSheepState instance;
    public static GroupSheepState GetInstance()
    {

        if (instance == null)
            instance = new GroupSheepState();
        return instance;

    }

    public override void Enter(Sheep owner)
    {
        base.Enter(owner);

        owner.GetSteering.AlignmentOn();
        owner.GetSteering.CohesionOn();
        owner.GetSteering.SeperationOn();
        owner.GetSteering.WanderOn();
        owner.GetSteering.ArriveOn(owner.Herde.Ogre.transform);
    }

    public override void FixedUpdate(Sheep owner)
    {
        base.FixedUpdate(owner);

        MovingEntity entity = owner.GetSteering.GetNearestEntity();
        if (entity == null)
        {
            owner.GetFSM.ChangeState(WanderingSheepState.GetInstance());
            return;
        }

        float dist = Vector3.Distance(owner.transform.position, entity.transform.position);

        if (dist < 1)
            owner.GetFSM.ChangeState(GrazingSheepState.GetInstance());
    }


    public override void HandleMsg(Sheep owner, MessageEventArgs e)
    {
        base.HandleMsg(owner, e);

        if (e.Message == Region.ARRIVED_AT_REGION)
        {
            owner.GetSteering.ArriveOn(owner.Herde.Ogre.transform);
        }
        else if (e.Message == Region.FOUND_NEW_REGION)
        {
            owner.GetSteering.ArriveOff();
        }
    }


    public override void Exit(Sheep owner)
    {
        base.Exit(owner);

        owner.GetSteering.AlignmentOff();
        owner.GetSteering.CohesionOff();
        owner.GetSteering.SeperationOff();
        owner.GetSteering.WanderOff();

        owner.GetSteering.ArriveOff();
        owner.Stop();
    }
}

public class GrazingSheepState : State<Sheep>
{
    private static GrazingSheepState instance;
    public static GrazingSheepState GetInstance()
    {
        if (instance == null)
            instance = new GrazingSheepState();
        return instance;
    }

    public override void Enter(Sheep owner)
    {
        base.Enter(owner);

        owner.Stop();
    }

    public override void FixedUpdate(Sheep owner)
    {
        base.FixedUpdate(owner);

        owner.Hunger -= 2;


        if (owner.Hunger > 2)
            return;

        if (owner.GetSteering.NearEntities.Count == 0)
        {
            owner.GetFSM.ChangeState(WanderingSheepState.GetInstance());
            return;
        }

        MovingEntity entity = owner.GetSteering.GetNearestEntity();
        float dist = Vector3.Distance(owner.transform.position, entity.transform.position);

        if (dist > 2 || owner.Hunger < Random.Range(10, 20))
            owner.GetFSM.ChangeState(GroupSheepState.GetInstance());

    }

    public override void Exit(Sheep owner)
    {
        base.Exit(owner);
    }
}


