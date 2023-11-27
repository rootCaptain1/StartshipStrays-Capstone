using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public class SelectorNode : CompositeNode
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
      int curIndex = 0;

      foreach (NodePort port in Outputs)
      {
        if(port.Connection == null || port.Connection.node == null|| port.Connection.node is BTBaseNode == false)
        {
          continue;
        }

        BTBaseNode child = port.Connection.node as BTBaseNode;

        switch (child.Update())
        {
          case State.Running:
            return State.Running;
          case State.Success: 
            return State.Success;
          case State.Failure:
            curIndex++;
            continue;
        }
      }

      return curIndex == _portCount ? State.Failure : State.Running;
    }
  }
}