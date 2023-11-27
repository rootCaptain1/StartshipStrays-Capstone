using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody2D))]
public class CanineController : MonoBehaviour
{
  public float offsetPosition;
  public float movementSpeed;

  private float currentSpeed;
  private Transform _target;
  private Vector2 moveDirection;
  private Rigidbody2D _rb;  //Canines rigid body

  // Start is called before the first frame update
  void Start()
  {
    _target = GameObject.FindGameObjectWithTag("Player").transform;
    _rb = GetComponent<Rigidbody2D>();
  }

  public void OnMove(Vector2 input)
  {
    moveDirection = input;
  }

  public void OnDisconnect()
  {
    FollowPlayer();
  }

  private void FixedUpdate()
  {
    Move(moveDirection);
  }

  //Follow the player based on their transform
  public void FollowPlayer()
  {
    transform.position = new Vector2(_target.position.x, _target.position.y - offsetPosition);

    Debug.Log("Player: " + _target.position);
  }

  //Canine movement control/input
  public void Move(Vector2 input)
  {
    Vector2 position = transform.position;  //Get players current position

    if (position.x != 0 || position.y != 0)  //If the player is moving in the horizontal or vertical direction
    {
      currentSpeed = movementSpeed;    //Update the movement speed to be the current speed

      position.x += currentSpeed * input.x * Time.deltaTime;//Update the X axis
      position.y += currentSpeed * input.y * Time.deltaTime;//Update the Y axis
      transform.position = position;    //Set new position    
    }
    else //Else we are not moving speed = 0
    {
      currentSpeed = 0;
    }
  }

  public void Shoot()
  {
    //Insert barking audio here
  }
}
