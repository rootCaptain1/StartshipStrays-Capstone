using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AICore
{
  [RequireComponent(typeof(CircleCollider2D))]
  public class AITrigger : MonoBehaviour
  {
    [SerializeField] private AIAgentBase _myAgent;        //Agent
    [SerializeField] private CircleCollider2D _collider;  //Agent Collider

    /// <summary>
    /// Get the collider and assign the trigger to be true
    /// If it is not already
    /// Check if the parent has the AIAgentBase script
    /// Otherwise print out an error
    /// </summary>
    private void Awake()
    {
      _collider = GetComponent<CircleCollider2D>();
      _collider.isTrigger = true;  //Dummy proof

      if (transform.parent.GetComponentInChildren<AIAgentBase>() != null)
      {
        _myAgent = transform.parent.GetComponentInChildren<AIAgentBase>();
      }
      else
      {
        Debug.LogError("ERROR: Parent of AI Trigger does not have a child with an AI Agent");
      }
    }

    //Set the radius of the collider
    public void SetRadius(float r)
    {
      _collider.radius = r;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
      if (_myAgent == null || other.transform != _myAgent.transform)
      {
        return;
      }
      _myAgent.HasReachedDestination = true;
    }
    public void OnTriggerStay2D(Collider2D other)
    {
      if (_myAgent == null || other.transform != _myAgent.transform)
      {
        return;
      }
      _myAgent.HasReachedDestination = true;
    }
    public void OnTriggerExit2D(Collider2D other)
    {
      if (_myAgent == null || other.transform != _myAgent.transform)
      {
        return;
      }
      _myAgent.HasReachedDestination = false;
    }
  }
}
