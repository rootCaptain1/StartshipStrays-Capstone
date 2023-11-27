using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public abstract class BTBaseNode : Node
  {
    public bool hasStarted = false;
    public State state;

    protected BTAgent _owner;

    public State Update()
    {
      if(hasStarted == false)
      {
        OnStart();
        hasStarted = true;
      }

      state = OnUpdate();

      if(state == State.Failure || state == State.Success )
      {
        OnStop();
        hasStarted = false;
      }
      return state;
    }

    protected abstract void OnStart();
    protected abstract State OnUpdate();
    protected abstract void OnStop();

    public virtual void InitNode(BTAgent owner)
    {
      _owner = owner;
    }
  }

  public enum State
  {
    Success,
    Failure,
    Running
  }
}