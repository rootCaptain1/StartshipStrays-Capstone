using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public class DebugAction : ActionNode
  {
    public string message;
    public TriggerType triggerType;

    protected override void OnStart()
    {
      if(triggerType.HasFlag(TriggerType.Start) == true)
      {
        Debug.Log("On Start: " + message);
      }
    }

    protected override void OnStop()
    {
      if (triggerType.HasFlag(TriggerType.Stop) == true)
      {
        Debug.Log("On Stop: " + message);
      }
    }

    protected override State OnUpdate()
    {
      if (triggerType.HasFlag(TriggerType.Update) == true)
      {
        Debug.Log("On Update: " + message);
      }

      return State.Success;
    }

    [System.Flags]
    public enum TriggerType
    {
      Start = 1,
      Update = 2,
      Stop = 4
    }
  }
}