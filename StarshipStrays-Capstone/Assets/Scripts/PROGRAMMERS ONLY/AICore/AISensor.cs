using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AICore
{
  [RequireComponent(typeof(CircleCollider2D))]
  public class AISensor : MonoBehaviour
  {
    [SerializeField] private AIAgentBase _myAgent;
    [SerializeField] private CircleCollider2D _collider;

    /// <summary>
    /// Get the CircleCollider2D if one is not provided
    /// then return the new collider
    /// </summary>
    public CircleCollider2D GetCircleCollider2D
    {
      get
      {
        if(_collider == null)
        {
          _collider = GetComponent<CircleCollider2D>();
        }
        return _collider;
      }
    }

    /// <summary>
    /// Set the CircleCollider2D trigger to true
    /// If the parent has the AIAgentBase script then get it
    /// Otherwise provide an error to the console
    /// </summary>
    private void Awake()
    {
      GetCircleCollider2D.isTrigger = true; //Dummy proof

      if(GetComponentInParent<AIAgentBase>() != null)
      {
        _myAgent = GetComponentInParent<AIAgentBase>();
      }
      else
      {
        Debug.LogError("ERROR: AI Sensor requires a parent object with an AI agent attached");
      }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if(_myAgent == null)
      {
        return;
      }
      _myAgent.OnSensorEvent(TriggerEventType.Enter, other);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
      if (_myAgent == null)
      {
        return;
      }

      Debug.Log("trigger stay");

      _myAgent.OnSensorEvent(TriggerEventType.Stay, other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (_myAgent == null)
      {
        return;
      }
      Debug.Log("Exit");
      _myAgent.OnSensorEvent(TriggerEventType.Exit, other);
    }
  }

  public enum TriggerEventType
  {
    Enter,
    Stay,
    Exit
  }
}
