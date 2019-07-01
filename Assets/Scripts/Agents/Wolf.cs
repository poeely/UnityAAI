using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Agent, IHerdeEntity
{
    private StateMachine<Wolf> stateMachine;
    public StateMachine<Wolf> GetStateMachine { get { return stateMachine; } }

    //Stateloader doesn't work in build
    //public static Dictionary<string, State<Wolf>> states;

    private Herde herde;
    public Herde Herde { get { return herde; } }

    public Sheep Target = null;

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine<Wolf>(this);
        
        //StateLoader doesnt work in build of the simulation (does work in unityeditor).
        //if(states == null)
        //    states = new Dictionary<string, State<Wolf>>();

        //ScriptLoader.LoadScript<Wolf>("FollowHerdeState", states);
        //ScriptLoader.LoadScript<Wolf>("RetrieveSheepState", states);
        //ScriptLoader.LoadScript<Wolf>("PushBackSheepState", states);
        //ScriptLoader.LoadScript<Wolf>("GlobalWolfState", states);

    }

    private void Start()
    {
        stateMachine.SetGlobalState(GlobalWolfState.GetInstance);
        stateMachine.SetCurrentState(FollowHerdeState.GetInstance);
    }

    public void Init(Herde herde)
    {
        this.herde = herde;

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

    private void OnDrawGizmos()
    {
        stateMachine.DrawGizmos();
    }

    public StateMachine<Wolf> GetFSM { get { return stateMachine; } }
}


public class GlobalWolfState : State<Wolf>
{
    private static GlobalWolfState instance;
    public static GlobalWolfState GetInstance
    {
        get
        {
            if (instance == null)
                instance = new GlobalWolfState();
            return instance;
        }
    }

    public override void Enter(Wolf owner)
    {
        base.Enter(owner);
        owner.GetSteering.ObstacleAvoidenceOn();
    }
}

public class FollowHerdeState : State<Wolf>
{
    private static FollowHerdeState instance;
    public static FollowHerdeState GetInstance
    {
        get
        {
            if (instance == null)
                instance = new FollowHerdeState();
            return instance;
        }
    }

    Vector3 targetPos = new Vector3();

    public override void Enter(Wolf owner)
    {
        base.Enter(owner);
        Debug.Log("[Wolf]: Start Following Herde.");
    }

    public override void FixedUpdate(Wolf owner)
    {
        base.FixedUpdate(owner);

        Sheep target = owner.Herde.CheckHerdCohesion();

        if (target != null)
        {
            owner.Target = target;
            owner.GetStateMachine.ChangeState(RetrieveSheepState.GetInstance);
        }

        Vector3 centroid = owner.Herde.HerdeCentroid();
        targetPos = centroid - owner.Herde.Ogre.Position;
        targetPos.Normalize();
        targetPos *= (owner.Herde.MaxHerdeRadius + 10.0f);
        targetPos = centroid + targetPos;
        owner.GetSteering.ArriveOn(targetPos);
    }

    public override void Exit(Wolf owner)
    {
        base.Exit(owner);
        Debug.Log("[Wolf]: End Following Herde");
        owner.GetSteering.ArriveOff();
    }

    public override void DrawGizmos(Wolf owner)
    {
        base.DrawGizmos(owner);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, 1f);

    }
}

public class RetrieveSheepState: State<Wolf>
{
    private static RetrieveSheepState instance;
    public static RetrieveSheepState GetInstance
    {
        get
        {
            if (instance == null)
                instance = new RetrieveSheepState();
            return instance;
        }
    }

    public override void Enter(Wolf owner)
    {
        base.Enter(owner);
        owner.GetSteering.FleeOn(owner.Herde.HerdeCentroid());
        Debug.Log("[wolf]: Enter RetrieveSheepState.");
    }

    public override void FixedUpdate(Wolf owner)
    {
        base.FixedUpdate(owner);
        Vector3 centroid = owner.Herde.HerdeCentroid();

        owner.GetSteering.FleeOn(centroid);

        Vector3 dir = owner.Target.Position - centroid;
        dir.Normalize();

        Vector3 desiredPos = owner.Target.Position + dir * 8.0f;

        float disttoTarget = Vector3.Distance(owner.Position, desiredPos);


        owner.GetSteering.ArriveOn(desiredPos);
        if (Vector3.Distance(owner.Target.Position, centroid) < owner.Herde.MaxHerdeRadius)
            owner.GetStateMachine.ChangeState(FollowHerdeState.GetInstance);

    }

    public override void Exit(Wolf owner)
    {
        base.Exit(owner);
        Debug.Log("Exit RetrieveSheepState");
        owner.GetSteering.FleeOff();
        owner.GetSteering.ArriveOff();
    }
}

//public class PushBackSheepState: State<Wolf>
//{
//    private static PushBackSheepState instance;
//    public static PushBackSheepState GetInstance
//    {
//        get
//        {
//            if (instance == null)
//                instance = new PushBackSheepState();
//            return instance;
//        }
//    }

//    public override void Enter(Wolf owner)
//    {
//        base.Enter(owner);

//        Debug.Log("[wolf]: Entered PushBackState.");
//    }

//    public override void FixedUpdate(Wolf owner)
//    {
//        base.FixedUpdate(owner);
//        Vector3 centroid = owner.Herde.HerdeCentroid();
//        Vector3 dir = owner.Target.Position - centroid;
//        dir.Normalize();

//        Vector3 desiredPos = owner.Target.Position + (dir * 8.0f);
//        owner.GetSteering.ArriveOn(desiredPos);

//        if (Vector3.Distance(owner.Target.Position, owner.Herde.HerdeCentroid()) < owner.Herde.MaxHerdeRadius)
//        {
//            owner.GetStateMachine.ChangeState(FollowHerdeState.GetInstance);
//        }
//    }

//    public override void Exit(Wolf owner)
//    {
//        base.Exit(owner);
//        owner.GetSteering.ArriveOff();
//        Debug.Log("[Wolf]: Exit PushBackState");
        
//    }
//}
