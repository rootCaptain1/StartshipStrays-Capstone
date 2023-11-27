using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using AICore;

namespace BehaviorTree
{
  public class SetNextWaypoint : ActionNode
  {
    public string waypointName;
    public string waypointIndex;
    public WaypointType waypointType;

    private Transform[] waypoints; //Array of points the agent will wander to while "Idle"

    public enum WaypointType
    {
      Linear,
      Random
    }

    protected override void OnStart()
    {
      if(waypoints == null || _owner.GetBlackBoard.ContainsKey(waypointIndex) == false)
      {
        return;
      }

      switch (waypointType)
      {
        case WaypointType.Linear:
          int v = (int)_owner.GetBlackBoard[waypointIndex];
          v++;

          if(v > waypoints.Length - 1)
          {
            v = 0;
          }
          _owner.GetBlackBoard[waypointIndex] = v;
          break;
        case WaypointType.Random:
          _owner.GetBlackBoard[waypointIndex] = Random.Range(0, waypoints.Length);
          break;
      }
    }

    protected override void OnStop()
    {
     //Not needed
    }

    protected override State OnUpdate()
    {
      if(_owner.GetBlackBoard.ContainsKey(waypointName) == false || _owner.GetBlackBoard.ContainsKey(waypointIndex) == false)
      {
        return State.Failure;
      }

      Transform newPoint = waypoints[(int)_owner.GetBlackBoard[waypointIndex]];

      _owner.SetTarget(
        newPoint.position,
        null,
        Vector3.Distance(_owner.transform.position, newPoint.position),
        Time.time, 
        TargetType.Waypoint);

      return State.Success;
    }

    public override void InitNode(BTAgent owner)
    {
      base.InitNode(owner);

      if (_owner.GetBlackBoard.ContainsKey(waypointName))
      {
        waypoints = _owner.GetBlackBoard[waypointName] as Transform[];
      }
    }
  }
}