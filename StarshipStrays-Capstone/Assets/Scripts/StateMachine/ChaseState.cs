using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : StateBase
{
  public float stopChasingAtDistance = 5.0f;

  public override StateType GetStateType { get { return StateType.Chase; } }

  public override StateType OnStateUpdate()
  {
    //Only works on NavMeshs vvv
    //_myAgent._navAgent.SetDestination(_myAgent.target.position);
    //_myAgent._navAgent.Move(_myAgent.target.position);

    _myAgent.transform.position = new Vector3(_myAgent.target.position.x, _myAgent.target.position.y);

    Debug.Log("Agent" + _myAgent.transform.position);
    Debug.Log(transform.position);

    if (Vector2.Distance(_myAgent.transform.position, transform.position) >= stopChasingAtDistance)
    {
      return StateType.Idle;
    }
    return GetStateType;
  }
}