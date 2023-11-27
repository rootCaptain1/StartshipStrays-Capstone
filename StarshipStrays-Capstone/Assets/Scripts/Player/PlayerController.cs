using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PersistenceSystem;


[System.Serializable]

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [Header("Player Stats")]
    [SerializeField] private int _money = 0;                                 //The players current money
    public int maxHealth;                                                       //Player's max health
    public int currentHealth;                                                   //Current health the player has
    public int currentSpeed;                                                    //Speed that the player is currently moving at
    public int movementSpeed;                                                   //Total speed for the player to move

    [Header("Bullet Info")]
    [Tooltip("This object will be the parent of the spawned bullet objects")]
    [SerializeField] private GameObject bulletHolder;
    [SerializeField] private int roundCountPerShot = 1;                         //Rounds that will be fired when the player goes to shoot
    [Tooltip("This is how far apart bullets will be between one another " +
        "when there is more than one round count per shot")]
    [SerializeField] private float angleDeviationBetweenShots = 10f;            //The angle that the shots will be apart from each other when firing more than one shot
    public int damage;                                                          //How much damage the player deals
    public float fireRate;                                                      //Fire rate
    public float bulletSpeed = 5.0f;                                            //Speed that the bullet will travel when shot
    public GameObject projectilePrefab;                                         //Prefab for bullet/projectile being shot

    [Header("UI Info")]
    [SerializeField] private PlayerUIManager _playerUIManager;

    [Header("Extra Info")]
    public Image gameOverImage; //refrence to ui in the inspector
    public FacingDirection facingDirection;                                     //Players facing direction

    // Private variables
    private Rigidbody2D _rb;                                                    //Players rigid body
    private Animator _animator;                                                 //Players animator
    private int _enemyDamage;                                                   //Enemy damage
    private bool _shootCoolDown = false; //Cool down for fire rate
    private Dictionary<FacingDirection, string> directions = new Dictionary<FacingDirection, string>()
    {
        { FacingDirection.Up, "North" },
        { FacingDirection.Down, "South" },
        { FacingDirection.Left, "West" },
        { FacingDirection.Right, "East" }
    };

    // Properties
    public int Money { get { return _money; } }

    // Start is called before the first frame update
    void Start()
    {
        _playerUIManager.UpdateUI(this);
        InitializeComponents();
        InitializePlayerStats();
    }

    private void InitializePlayerStats()
    {
        currentHealth = maxHealth; //Dummy check
    }

    private void InitializeComponents()
    {
        _animator = GetComponent<Animator>(); //Get Animator
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckEnemyInformation();

        /*
        // Check if maxHealth is less than or equal to 0
        if (!currentHealth || maxHealth <= 0)
        {
            // Activate the game over UI image
            if (gameOverImage != null)
            {
                gameOverImage.gameObject.SetActive(true);
            }
        }
        */

        CheckShootingInput();
    }

    private void CheckShootingInput()
    {
        if (_shootCoolDown == false && (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space)))
        {
            Shoot();
        }
    }

    private void CheckEnemyInformation()
    {
        //Get enemy information
        var _enemy = FindObjectOfType<SO_Enemy>();

        if (_enemy == null)
        {
            Debug.LogWarning("No enemies around!");
        }
        else
        {
            _enemyDamage = _enemy.damage;
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    //Players movement control/input
    void UpdateMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 position = transform.position;

        UpdateSpeed(horizontal, vertical, position);
        UpdateFacingDirection(horizontal, vertical);
        UpdateAnimations(horizontal, vertical);
    }


    private void UpdateFacingDirection(float horizontal, float vertical)
    {
        if (Mathf.Abs(vertical) == 0 && Mathf.Abs(horizontal) == 0)
        {
            return;
        }

        facingDirection = Mathf.Abs(vertical) > Mathf.Abs(horizontal) ?
            (vertical > 0 ? FacingDirection.Up : FacingDirection.Down) :
            (horizontal > 0 ? FacingDirection.Right : FacingDirection.Left);
        SetSpriteDirection(facingDirection);
    }

    private void UpdateSpeed(float horizontal, float vertical, Vector2 position)
    {
        if (horizontal != 0 || vertical != 0) //If the player is moving in the horizontal or vertical direction
        {
            currentSpeed = movementSpeed;     //Update speed to be 2

            position.x += currentSpeed * horizontal * Time.deltaTime;//Update the X axis
            position.y += currentSpeed * vertical * Time.deltaTime;  //Update the Y axis
            transform.position = position;
        }
        else //Else we are not moving speed = 0
        {
            currentSpeed = 0;
        }
    }

    private void UpdateAnimations(float horizontal, float vertical)
    {
        if (_animator == null) return;
        //Animations
        _animator.SetFloat("Horizontal", horizontal);
        _animator.SetFloat("Vertical", vertical);
        _animator.SetFloat("Speed", currentSpeed);
    }

    void SetSpriteDirection(FacingDirection direction)
    {
        if (_animator == null) return;

        foreach (string dir in directions.Values)
        {
            _animator.SetBool(dir, false);
        }

        _animator.SetBool(directions[direction], true);
    }

    /// <summary>
    /// When player left clicks or hits space bar shoot projectile
    /// </summary>
    void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Ammo please!");
            return;
        }

        CalculateShootingAngles();
        StartCoroutine(StartCooldown());
    }

    private void CalculateShootingAngles()
    {
        if (roundCountPerShot <= 1)
        {
            CreateBullet(transform.rotation);
        }
        else
        {

            float angleOffset = angleDeviationBetweenShots / (roundCountPerShot - 1); // Calculate the offset

            // Calculate the starting angle so that bullets are centered
            float startingAngle = -angleDeviationBetweenShots / 2;

            for (int i = 0; i < roundCountPerShot; i++)
            {
                float currentAngle = startingAngle + angleOffset * i;
                CreateBullet(transform.rotation * Quaternion.Euler(0, 0, currentAngle));
            }
        }
    }


    /// <summary>
    /// Creates a child "Bullet" and gets the placement based on
    /// The parents (Players) transform
    /// </summary>
    public void CreateBullet(Quaternion bulletRotation)
    {
        GameObject clone = Instantiate(projectilePrefab, transform.position, GetBulletRotation(bulletRotation));
        SetBulletProperties(clone);
    }

    private void SetBulletProperties(GameObject clone)
    {
        clone.transform.parent = bulletHolder.transform;
        var proj = clone.GetComponent<Rigidbody2D>();
        proj.velocity = clone.transform.up * bulletSpeed;
        clone.name = "Bullet";
    }

    private Quaternion GetBulletRotation(Quaternion initialBulletRotation)
    {
        Quaternion rotation;
        switch (facingDirection)
        {
            case FacingDirection.Up:
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case FacingDirection.Down:
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case FacingDirection.Left:
                rotation = Quaternion.Euler(0, 0, 90);
                break;
            case FacingDirection.Right:
                rotation = Quaternion.Euler(0, 0, -90);
                break;
            default:
                rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
        if (roundCountPerShot <= 1)
        {
            initialBulletRotation = Quaternion.identity;
        }

        return initialBulletRotation * rotation;
    }

    public void PickupMoney(int amount)
    {
        _money += amount;
        _playerUIManager.UpdateUI(this);
    }

    //Used to heal the player
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        _playerUIManager.UpdateUI(this);
    }

    public void ReduceMoney(int amount)
    {
        _money -= amount;
        if (_money < 0)
        {
            _money = 0;
        }
        _playerUIManager.UpdateUI(this);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("I was hit!");

        if (currentHealth <= 0)  //unity load scene command - this line already calcucaltesnfuwn player health
        {
            currentHealth = 0;
            Destroy(gameObject);

            Debug.Log("GAME OVER!");
        }
        _playerUIManager.UpdateUI(this);
    }

    //Used to upgrade the players max health
    public void UpgradeMaxHealth(int amount)
    {
        maxHealth += amount;
        _playerUIManager.UpdateUI(this);
    }
    //Used to upgrade the players bullet spread
    public void UpgradeBulletSpread(int amount) => roundCountPerShot += amount;

    //Cool down fire rate
    IEnumerator StartCooldown()
    {
        _shootCoolDown = true;
        yield return new WaitForSeconds(fireRate);
        _shootCoolDown = false;
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
    /// Check if player was hit with an enemy bullet
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy Bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(_enemyDamage);
        }
    }

    public void LoadData(GameData data)
    {
        _money = data.money;
    }

    public void SaveData(ref GameData data)
    {
        data.money = _money;
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

