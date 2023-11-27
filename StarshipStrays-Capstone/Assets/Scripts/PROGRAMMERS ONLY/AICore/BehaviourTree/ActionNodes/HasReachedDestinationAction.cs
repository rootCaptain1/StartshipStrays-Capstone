using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public class HasReachedDestinationAction : ActionNode
  {
    protected override void OnStart()
    {
      //Do not need
    }

    protected override void OnStop()
    {
      //Do not need
    }

    //Set HasReachedDestination to success or failure
    protected override State OnUpdate()
    {
      return _owner.HasReachedDestination ? State.Success : State.Failure;
    }
  }
}