using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T>
{
    void Enter(T owner);
    void Exit(T owner);

    void Update(T owner);
    void FixedUpdate(T owner);

    void HandleMsg(T owner, MessageEventArgs e);
    void DrawGizmos(T owner);
}


/// <summary>
/// A generic state
/// 
///  Author: Vincent Versnel
/// </summary>
/// <typeparam name="T">Type reference to owner of the StateMachine</typeparam>
public abstract class State<T> : IState<T>
{
    // This is to prevent switching of states before callbacks are returned. (PathRequester)
    // TODO: Find alternative solution
    public bool readyToSwitch = true;

    public virtual void Enter(T owner)
    {

    }

    public virtual void Exit(T owner)
    {

    }

    public virtual void Update(T owner)
    {

    }

    public virtual void FixedUpdate(T owner)
    {

    }

    public virtual void HandleMsg(T owner, MessageEventArgs e)
    {

    }

    public virtual void DrawGizmos(T owner)
    {

    }
}

public class DefaultState<T> : State<T>
{

}