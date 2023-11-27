using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public class RepeatNode : DecoratorNode
  {
    protected override void OnStart()
    {
      //Not needed
    }

    protected override void OnStop()
    {
      //Not needed
    }

    protected override State OnUpdate()
    {
      BTBaseNode child = GetPort("exit").Connection.node as BTBaseNode;
      child.Update();
      return State.Running;
    }
  }
}