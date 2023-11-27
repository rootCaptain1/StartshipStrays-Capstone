using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace AICore
{
  [RequireComponent(typeof(NavMeshAgent))]
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Collider2D))]
  public class AIAgentBase : MonoBehaviour
  {
    [SerializeField, Range(0f, 360f)] protected float _fov = 60.0f;      //Angle of what can be seen
    [SerializeField, Range(0f, 1f)] protected float _sightRange = 1.0f;  //How far of what can be seen

    [SerializeField] protected AISensor _aiSensor;
    [SerializeField] protected AITrigger _aiTrigger;

    [SerializeField] protected Target _currentTarget = new();
    [SerializeField] protected Target _visualTarget = new();

    public LayerMask visMask;

    protected NavMeshAgent _navAgent;
    protected bool _hasReachedDestination = false;

    public AITrigger GetAITrigger { get { return _aiTrigger; } }

    public Target GetCurrentTarget { get { return _currentTarget; } }
    public NavMeshAgent GetNavAgent { get { return _navAgent; } }
    public bool HasReachedDestination { get { return _hasReachedDestination; } set { _hasReachedDestination = value; } }

    public AgentFacing agentFacing;

    /// <summary>
    /// Determines what way the enemy is facing and
    /// returns the proper facing direction
    /// </summary>
    public Vector3 GetFacingVector
    {
      get
      {
        return agentFacing switch
        {
          AgentFacing.Up => transform.up,
          AgentFacing.Down => -transform.up,
          AgentFacing.Left => -transform.right,
          AgentFacing.Right => transform.right,
          _ => Vector3.zero,
        };
      }
    }

    public void Movement()
    {
      float vert = _navAgent.velocity.y;
      float horz = _navAgent.velocity.x;

      if(Mathf.Abs(vert) > Mathf.Abs(horz))
      {
        if(vert > 0)
        {
          agentFacing = AgentFacing.Up;
        }
        else
        {
          agentFacing = AgentFacing.Down;
        }
      }
      else
      {
        if (horz > 0)
        {
          agentFacing = AgentFacing.Right;
        }
        else
        {
          agentFacing = AgentFacing.Left;
        }
      }
    }

    /// <summary>
    /// Get and return the position of the senor
    /// being cast out by the agent
    /// </summary>
    public Vector3 GetSensorPosition
    {
      get
      {
        if (_aiSensor == null)
        {
          return Vector3.zero;
        }
        Vector3 pos = _aiSensor.transform.position;
        pos.x += _aiSensor.GetCircleCollider2D.offset.x * _aiSensor.transform.lossyScale.x;
        pos.y += _aiSensor.GetCircleCollider2D.offset.y * _aiSensor.transform.lossyScale.y;

        return pos;
      }
    }

    /// <summary>
    /// Gets the radius of the sensor being cast out
    /// </summary>
    public float GetSensorRadius
    {
      get
      { 
        if(_aiSensor == null)
        {
          return 0.0f;
        }
        float sensorRadius = _aiSensor.GetCircleCollider2D.radius;
        float radius = Mathf.Max(sensorRadius * _aiSensor.transform.lossyScale.x, sensorRadius * _aiSensor.transform.lossyScale.y);
        return Mathf.Max(radius, sensorRadius * _aiSensor.transform.position.z);
      }
    }

    //Get the NavMeshAgent on start
    protected virtual void Start()
    {
      _navAgent = GetComponent<NavMeshAgent>();
    }

    //Set the target and go to its position
    public void SetTarget(Vector3 p, Collider2D c, float d, float t, TargetType tt)
    {
      _currentTarget.Set(p, c, d, t, tt);
      _aiTrigger.transform.position = _currentTarget.GetPosition;
    }

    /// <summary>
    /// If what we are colliding with does not have a player tag then return
    /// If the tag is equal to "Player" then set the target to the players position
    /// </summary>
    public void OnSensorEvent(TriggerEventType tet, Collider2D other)
    {
      Debug.Log(tet);

      if(other == null || tet == TriggerEventType.Exit)
      {
        return;
      }

      if(other.CompareTag("Player"))
      {
        if (IsColliderVisible(other) == true)
        {
          Debug.Log("I see the player!");
          _visualTarget.Set(other.transform.position, other, Vector3.Distance(transform.position, other.transform.position), Time.time, TargetType.Visual);
        }
      }
    }

    //Clear out the target
    protected virtual void FixedUpdate()
    {
      ClearTargets();
      Movement();
    }

    protected void ClearTargets()
    {
      _visualTarget.Clear();
    }

    /// <summary>
    /// If the target is in site continue to pursue target
    /// Else clear out the target and forget about it
    /// </summary>
    public void AssessTargets()
    {
      if (_visualTarget != null && _visualTarget.GetTargetType == TargetType.Visual)
      {
        SetTarget(_visualTarget.GetPosition, _visualTarget.GetCollider2D, _visualTarget.Distance, _visualTarget.GetTime, _visualTarget.GetTargetType);
      }
      else if (_currentTarget.GetTargetType != TargetType.Waypoint)
      {
        _currentTarget.Clear();
      }
    }

    /// <summary>
    /// Checks if the other collider that gets hit is within the raycast (FOV)
    /// If it is outside of the FOV then set isColliderVisible to false
    /// Else set to true
    /// </summary>
    protected bool IsColliderVisible(Collider2D other)
    {
      Vector3 direction = other.transform.position - transform.position;
      float angle = Vector3.Angle(GetFacingVector, direction);

      if (angle > _fov * 0.5f)
      {
        return false;
      }

      RaycastHit2D hit = Physics2D.Raycast(GetSensorPosition, direction.normalized, GetSensorRadius * _sightRange);

      if (hit.collider == other.CompareTag("Player"))  //Specific to Player because enemy will not care about Canine
      {
        Debug.Log("Player!!!");
        return true;
      }
      return false;
    }

    //Draws the solid arc that gets casted out from the Agent to see the player
    private void OnDrawGizmos()
    {
      if(_aiSensor == null)
      {
        return;
      }
      Color colour = new(1f, 0f, 0f, 0.15f); //Color red
      UnityEditor.Handles.color = colour;

      Vector3 rotatedForward = Quaternion.Euler(0f, 0f, _fov * 0.5f) * -GetFacingVector;
      //transform.rotation = Quaternion.AngleAxis(GetSensorRadius * _sightRange, Vector2.right);

      UnityEditor.Handles.DrawSolidArc(GetSensorPosition, Vector3.forward, rotatedForward, _fov, GetSensorRadius * _sightRange);

      Gizmos.color = Color.red;

      if (_navAgent != null)
      {
        Gizmos.DrawLine(transform.position, _navAgent.destination);
      }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject.CompareTag("Player"))
      {
        other.gameObject.gameObject.SetActive(false);
      }
    }

    public enum AgentFacing
    {
      Up,
      Down,
      Left,
      Right
    }
  }
}