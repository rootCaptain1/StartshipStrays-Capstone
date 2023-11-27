using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public class RootNode : BTBaseNode
  {
    [Output(connectionType = ConnectionType.Override)] public bool exit;

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
      BTBaseNode child = GetPort("exit").Connection.node as BTBaseNode;
      state = child.Update();
      return state;
    }

    public override void InitNode(BTAgent owner)
    {
      base.InitNode(owner);
      state = State.Running;
    }
  }
}