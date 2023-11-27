using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int _playerSpeed;
    [SerializeField] private float inputSmoothing = 0.1f;

    private Rigidbody2D _rb2d;
    private Vector2 _moveDirection = Vector2.zero;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        
        
        // Smooth input
        _moveDirection.x = Mathf.Lerp(_moveDirection.x, horizontalInput, inputSmoothing);
        _moveDirection.y = Mathf.Lerp(_moveDirection.y, verticalInput, inputSmoothing);

        if (_moveDirection.magnitude > 1)
        {
            _moveDirection = _moveDirection.normalized;
        }

        if (anim != null)
        {
            anim.SetFloat("Horizontal", _moveDirection.x);
            anim.SetFloat("Vertical", _moveDirection.y);
            if (_moveDirection.x > 0)
            {
                SetDirection("East");
            }
            else if (_moveDirection.x < 0)
            {
                SetDirection("West");
            }
            if (_moveDirection.y > 0)
            {
                SetDirection("North");
            }
            if (_moveDirection.y < 0)
            {
                SetDirection("South");
            }
        }
    }

    private void FixedUpdate()
    {
        _rb2d.MovePosition(_rb2d.position + _moveDirection * _playerSpeed * Time.fixedDeltaTime);
    }

    void SetDirection(string direction)
    {
        anim.SetBool("East", false);
        anim.SetBool("West", false);
        anim.SetBool("North", false);
        anim.SetBool("South", false);
            
        anim.SetBool(direction, true);
    }
}
