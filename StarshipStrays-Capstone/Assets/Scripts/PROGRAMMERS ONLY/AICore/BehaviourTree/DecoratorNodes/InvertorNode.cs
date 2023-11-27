using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public class InvertorNode : DecoratorNode
  {
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
      BTBaseNode child = GetPort("exit").Connection.node as BTBaseNode;

      switch(child.Update())
      {
        case State.Running:
          state = State.Running;
          break;
        case State.Success:
          state = State.Failure;
          break;
        case State.Failure:
          state = State.Success;
          break;
      }
      return state;
    }
  }
}