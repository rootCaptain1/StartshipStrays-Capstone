using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICore;

namespace BehaviorTree
{
  public class BTAgent : AIAgentBase
  {
    [SerializeField] private BehaviourTreeGraph _btGraph;
    [SerializeField] private Transform[] _wayPoints;
    private Dictionary<string, object> _blackBoard;

    public Dictionary<string, object> GetBlackBoard { get {  return _blackBoard; } }

    protected override void Start()
    {
      base.Start();

      _blackBoard = new Dictionary<string, object>
      {
        { "Waypoints", _wayPoints },
        { "WaypointIndex", 0 }
      };

      if (_btGraph != null)
      {
        _btGraph = _btGraph.Copy() as BehaviourTreeGraph;
        _btGraph.InitBehaviourTree(this);
      }
    }

    protected override void FixedUpdate()
    {
      AssessTargets();

      if (_btGraph != null && _btGraph.RootNode != null)
      {
        _btGraph.Update();
      }

      base.FixedUpdate();
    }
  }
}
