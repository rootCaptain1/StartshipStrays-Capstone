using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Agent))]
public abstract class StateBase : MonoBehaviour
{
  protected Agent _myAgent;

  public abstract StateType GetStateType { get; }

  public void InitState(Agent myAgent)
  {
    _myAgent = myAgent;
  }

  public virtual void OnStateEnter() { }

  public abstract StateType OnStateUpdate();

  public virtual void OnStateExit() { }

  public enum StateType
  {
    Idle,
    Chase
  }
}
