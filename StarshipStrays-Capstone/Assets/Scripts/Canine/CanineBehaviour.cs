using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody2D))]
public class CanineBehaviour : MonoBehaviour
{
  private Transform _transform;
  private Transform _target;

  // Start is called before the first frame update
  void Start()
  {
    _transform = transform;
    _target = GameObject.FindGameObjectWithTag("Player").transform;
  }

  // Update is called once per frame
  void Update()
  {

  }

  private void FixedUpdate()
  {
    Movement();
  }

  void Movement()
  {
    Vector2 direction = (_target.position - transform.position);
    direction = new Vector2 (direction.x, direction.y - 1).normalized;
    _transform.Translate(direction * Time.deltaTime);
  }
}
