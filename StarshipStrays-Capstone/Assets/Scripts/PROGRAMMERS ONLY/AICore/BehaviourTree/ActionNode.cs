using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  [NodeTint(0, 0, 50)] //Color for the ActionNode to be displayed on the behavior tree

	public abstract class ActionNode : BTBaseNode 
	{
    [Input(connectionType = ConnectionType.Override)] public bool exit;
  }
}