using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  [NodeTint(50, 0, 0)] //Color for the decorator node to be displayed on the behavior tree

  public abstract class DecoratorNode : BTBaseNode
  {
    [Input(connectionType = ConnectionType.Override)] public bool enter;
    [Output(connectionType = ConnectionType.Override)] public bool exit;
  }
}