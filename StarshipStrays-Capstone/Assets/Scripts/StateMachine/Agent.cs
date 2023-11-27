using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
  public Transform target;
  public NavMeshAgent _navAgent;

  private StateBase _curState;
  private readonly Dictionary<StateBase.StateType, StateBase> _states = new Dictionary<StateBase.StateType, StateBase>();

  // Start is called before the first frame update
  void Start()
  {
    StateBase[] allStates = GetComponents<StateBase>();
    _navAgent = GetComponent<NavMeshAgent>();

    foreach (StateBase state in allStates)
    {
      if (_states.ContainsKey(state.GetStateType) == false)
      {
        _states.Add(state.GetStateType, state);
        state.InitState(this);
      }
    }

    ChangeState(StateBase.StateType.Idle);
  }

  // Update is called once per frame
  void Update()
  {
    if (_curState != null)
    {
      ChangeState(_curState.OnStateUpdate());
    }
    Debug.Log("curState" + _curState.ToString());
  }

  private void ChangeState(StateBase.StateType newState)
  {
    if (_curState != null && newState == _curState.GetStateType)
    {
      return;
    }

    _curState?.OnStateExit();
    _curState = _states[newState];
    _curState?.OnStateEnter();
  }
}
