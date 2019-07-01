using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// scriptting doesnt work in the build
//using CSScriptLibrary;

/// <summary>
/// A generic StateMachine
/// 
/// Author: Vincent Versnel
/// </summary>
public class StateMachine<T>
{
    // Ref to class that created this StateMachine
    private T owner;

    // Global state allows additional behaviour during any state
    private State<T> globalState;

    private State<T> currentState;
    private State<T> previousState;

    public State<T> CurrentState { get { return currentState; } }

    public StateMachine(T owner)
    {
        this.owner = owner;

        SetCurrentState(new DefaultState<T>());
        SetGlobalState(new DefaultState<T>());
    }

    public void ChangeState(State<T> state)
    {
        if (currentState == state)
            return;
        if (!currentState.readyToSwitch)
            return;

        // Swap & Exit previousState
        previousState = currentState;
        if (previousState != null)
            previousState.Exit(owner);

        // Enter a new currentState
        currentState = state;
        currentState.Enter(owner);
    }

    // Update states if they are created
    public void Update()
    {
        globalState.Update(owner);
        currentState.Update(owner);
    }

    // Fixed Update states
    public void FixedUpdate()
    {
        globalState.FixedUpdate(owner);
        currentState.FixedUpdate(owner);
    }

    // Sends messages through to global and current state.
    // These states then react correspondingly to the situation.
    public void HandleMessage(MessageEventArgs e)
    {
        globalState.HandleMsg(owner, e);
        currentState.HandleMsg(owner, e);
    }

    public void DrawGizmos()
    {
        globalState.DrawGizmos(owner);
        currentState.DrawGizmos(owner);
    }

    public void SetCurrentState(State<T> state)
    {
        currentState = state;
        currentState.Enter(owner);
    }

    public void SetGlobalState(State<T> state)
    {
        globalState = state;
        globalState.Enter(owner);
    }
}
