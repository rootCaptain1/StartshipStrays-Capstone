using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public class SequencerNode : CompositeNode
  {
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
      int curIndex = 0;

      foreach (NodePort port in Outputs)
      {
        if(port.Connection == null || port.Connection.node == null || port.Connection.node is BTBaseNode == false)
        {
          continue;
        }
        BTBaseNode child = port.Connection.node as BTBaseNode;

        switch (child.Update())
        {
          case State.Running:
            return State.Running;
          case State.Success:
            curIndex++;
            continue;
          case State.Failure:
            return State.Failure;
        }
      }
      return curIndex == _portCount ? State.Success : State.Running;
    }
  }
}