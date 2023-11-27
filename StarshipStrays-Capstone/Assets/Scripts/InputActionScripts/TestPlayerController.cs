using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody2D))]
public class TestPlayerController : MonoBehaviour
{
  public int maxHealth;       //Max health we can reach/start with
  public int currentHealth;   //Current health
  public int movementSpeed;   //Speed to go when moving
  public int damage;          //How much damage the player deals
  public float fireRate;      //Fire rate
  public GameObject projectilePrefab; //Bullet prefab
  public FacingDirection facingDirection; //Current facing direction

  private float currentSpeed; //Speed we are currently moving at
  private Animator _animator; //Animator
  private Rigidbody2D _rb;    //Rigid body
  private Vector2 moveDirection; //Vector input
  private bool _startCoolDown = false; //Cool down for fire rate

  // Start is called before the first frame update
  void Start()
  {
    currentHealth = maxHealth; //Double check
    _animator = GetComponent<Animator>(); //Get animator
    _rb = GetComponent<Rigidbody2D>(); //Get rigidbody
  }

  /// <summary>
  /// What they player can do while they are in
  /// the astronaut control
  /// </summary>
  public void OnMove(Vector2 input)
  {
    moveDirection = input;
    Shoot();
  }

  /// <summary>
  /// What will happen when the player 
  /// switches to the canine
  /// </summary>
  public void OnDisconnect()
  {
    moveDirection = Vector2.zero;
  }

  private void FixedUpdate()
  {
    Move(moveDirection);
  }

  //Players movement control/input
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

    //Checks the players facing direction (mostly for clarification and testing)
    if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
    {
      if (input.y > 0)
      {
        facingDirection = FacingDirection.Up;
        SetDirection("North");
      }
      else if (input.y < 0)
      {
        facingDirection = FacingDirection.Down;
        SetDirection("South");
      }
    }
    else
    {
      if (input.x > 0)
      {
        facingDirection = FacingDirection.Right;
        SetDirection("East");
      }
      else if (input.x < 0)
      {
        facingDirection = FacingDirection.Left;
        SetDirection("West");
      }
    }

    //Animations
    _animator.SetFloat("Horizontal", input.x);
    _animator.SetFloat("Vertical", input.y);
    _animator.SetFloat("Speed", currentSpeed);
  }

  void SetDirection(string direction)
  {
    _animator.SetBool("East", false);
    _animator.SetBool("West", false);
    _animator.SetBool("North", false);
    _animator.SetBool("South", false);

    _animator.SetBool(direction, true);
  }

  public void Shoot()
  {
    if (projectilePrefab == null)
    {
      Debug.LogWarning("Ammo please!");
      return;
    }

    //0 = left click
    if (_startCoolDown == false && UnityEngine.Input.GetMouseButtonDown(0) || _startCoolDown == false && UnityEngine.Input.GetKey(KeyCode.Space))
    {
      ChildInstantiate();
      StartCoroutine(CoolDown());
    }
  }

  /// <summary>
  /// Creates a child "Bullet" and gets the placement based on
  /// The parents (Players) transform
  /// </summary>
  public void ChildInstantiate()
  {
    float speed = 5.0f;

    GameObject clone = Instantiate(projectilePrefab, transform.position, transform.rotation);
    var proj = clone.GetComponent<Rigidbody2D>();
    proj.velocity = transform.TransformDirection(GetFacingVector) * speed;
    clone.GetComponent<SpriteRenderer>();
    clone.name = "Bullet";
  }

  //Cool down fire rate
  IEnumerator CoolDown()
  {
    _startCoolDown = true;
    yield return new WaitForSeconds(fireRate);
    _startCoolDown = false;
  }

  //Gets the facing direction
  public Vector3 GetFacingVector
  {
    get
    {
      return facingDirection switch
      {
        FacingDirection.Up => transform.up,
        FacingDirection.Down => -transform.up,
        FacingDirection.Left => -transform.right,
        FacingDirection.Right => transform.right,
        _ => Vector3.zero,
      };
    }
  }

  /// <summary>
  /// Facing positions for player
  /// </summary>
  public enum FacingDirection
  {
    Up,
    Down,
    Left,
    Right
  }
}