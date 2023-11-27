using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BehaviorTree
{
  public class WaitAction : ActionNode
  {
    public WaitType waitType;     //Wait for a set amount of time or random
    public Vector2 waitDuration;  //How long the agent will wait before going to next target

    private float _value;
    private float _startTime;

    public enum WaitType
    {
      SetDuration,
      RandomDuration
    }

    /// <summary>
    /// If the wait type is a set amount of time then grab that value
    /// Then wait that amount of time
    /// Else set a random range of time then wait that amount of time
    /// </summary>
    protected override void OnStart()
    {
      if (waitType == WaitType.SetDuration)
      {
        _value = waitDuration.x;
      }
      else
      {
        _value = Random.Range(waitDuration.x, waitDuration.y);
      }

      _startTime = Time.time;
    }

    /// <summary>
    /// Take the time then return if the agent is still waiting
    /// or has successfully waited the appropriate amount of time
    /// </summary>
    protected override State OnUpdate()
    {
      return ((Time.time - _startTime) > _value) ? State.Success : State.Running;
    }

    protected override void OnStop()
    {
      //Do not need
    }
  }
}