using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateBase
{
  public float chaseIfTargetIsInDistance = 5.0f;

  public override StateType GetStateType { get { return StateType.Idle; } }

  public override StateType OnStateUpdate()
  {
    if (Vector2.Distance(_myAgent.transform.position, transform.position) <= chaseIfTargetIsInDistance)
    {
      Debug.Log("PLAYER!");
      return StateType.Chase;
    }
    return GetStateType;
  }
}
