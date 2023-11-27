using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyBehavior : MonoBehaviour
{
    public float attackSpeed; //Attack speed

    private bool _startCoolDown = false; //Attack cool down
    private int _playerHealth;
    private int _enemyDamage; //Depends on assigned SO

    private Transform _transform; //Enemy transform
    private Transform _target;    //Target transform ("Player")

    private Animator animator; // Animator Component
    private bool isMoving; // Logic to see if the enemy is moving


    void Start()
    {
        animator = GetComponent<Animator>(); //get animator component

        //Find the player and enemy objects
        var _player = FindAnyObjectByType<PlayerController>();
        var _enemy = FindObjectOfType<SO_Enemy>();

        _transform = transform;
        _target = GameObject.FindGameObjectWithTag("Player").transform;

        _playerHealth = _player.currentHealth;
        _enemyDamage = _enemy.damage;

        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", 0f);
        animator.SetBool("isMoving", false);

        animator.SetBool("isNorth", false);
        animator.SetBool("isSouth", false);
        animator.SetBool("isEast", false);
        animator.SetBool("isWest", false);

    }





    void Update()
    {
        Movement();

        //update animator parameters
        //UpdateAnimatorParameters();
    }

    //Attack player
    public void OnTriggerEnter2D(Collider2D other)
    {
        AttackPlayer(other);
    }

    //Attack player if cool down allows
    public void OnTriggerStay2D(Collider2D other)
    {
        if (_startCoolDown == false)
        {
            AttackPlayer(other);
            StartCoroutine(CoolDown());
        }
    }

    //Attack player
    private void AttackPlayer(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.TakeDamage(_enemyDamage);
        }
    }

    //Match the enemies transform with players
    void Movement()
    {
        Vector2 direction = (_target.position - transform.position).normalized;
        _transform.Translate(direction * Time.deltaTime);

        //update animator paramteters based on movement direction
        UpdateAnimatorParameters(direction);
    }

    //Cool down fire rate
    IEnumerator CoolDown()
    {
        _startCoolDown = true;
        yield return new WaitForSeconds(attackSpeed);
        _startCoolDown = false;
    }

    //Update Animator parameters based on movement/direction
    void UpdateAnimatorParameters(Vector2 direction)
    {
        //set parameters in the animator window
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetBool("isMoving", direction.magnitude > 0f);

        // Set cardinal direction parameters
        animator.SetBool("isNorth", direction.y > 0 && direction.y > Mathf.Abs(direction.x));
        animator.SetBool("isSouth", direction.y < 0 && Mathf.Abs(direction.y) > Mathf.Abs(direction.x));
        animator.SetBool("isEast", direction.x > 0 && direction.x > Mathf.Abs(direction.y));
        animator.SetBool("isWest", direction.x < 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y));
    }
}

