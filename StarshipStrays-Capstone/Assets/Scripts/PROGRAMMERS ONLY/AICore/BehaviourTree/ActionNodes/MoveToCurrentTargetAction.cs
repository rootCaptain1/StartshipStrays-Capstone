using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
	public class MoveToCurrentTargetAction : ActionNode
	{

    protected override void OnStart()
    {
      //Do not need
    }

    protected override State OnUpdate()
    {
      if(_owner.GetNavAgent.pathPending == false)
      {
        _owner.GetNavAgent.SetDestination(_owner.GetCurrentTarget.GetPosition);
      }
      else
      {
        return State.Running;
      }

      return State.Success;
    }

    protected override void OnStop()
    {
      //Do not need
    }
  }
}