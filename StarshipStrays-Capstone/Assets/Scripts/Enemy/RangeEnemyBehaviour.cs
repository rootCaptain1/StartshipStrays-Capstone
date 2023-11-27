using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyBehaviour : MonoBehaviour
{
    private float shotCooldown = 1; //Cool down rate
    public float startShotCooldown; //Cool down rate between shots

    public float bulletSpeed = 35; //Bullet speed
    public GameObject bullet; //Bullet prefab

    private Transform _target;//Target transform ("Player")


    private Animator animator; // Animator Component
    private bool isMoving; // Logic to see if the enemy is moving



    void Start()
    {
        shotCooldown = startShotCooldown;
        animator = GetComponent<Animator>(); // call that animator component shi
    }

    void Update()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateAnimatorParameters(); //update animator para based on the players position relative to the player
        Shoot();


    }
    void UpdateAnimatorParameters()
    {
        Vector2 direction = (_target.position - transform.position).normalized;

        // Calculate angles to determine direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Update bool parameters
        animator.SetBool("isNorth", angle > 45 && angle <= 135);
        animator.SetBool("isSouth", angle < -45 && angle >= -135);
        animator.SetBool("isWest", angle > 135 || angle <= -135);
        animator.SetBool("isEast", angle > -45 && angle <= 45);

        // Update float parameters
        animator.SetFloat("isHorizontal", direction.x);
        animator.SetFloat("isVertical", direction.y);
    }

    public void Shoot()
    {
        if (bullet == null)
        {
            Debug.LogWarning("Range enemy has no ammo!");
        }
        if (shotCooldown <= 0)
        {
            animator.SetTrigger("Fire"); //trigger the firing animation
            ChildInstatiate(); //instantiate and shoot bullet
            shotCooldown = startShotCooldown;
        }
        else
        {
            shotCooldown -= Time.deltaTime;
        }
    }
    //the rotation

    public void ChildInstatiate()
    {
        Vector2 direction = new(_target.transform.position.x - transform.position.x, _target.transform.position.y - transform.position.y);

        GameObject clone = Instantiate(bullet, transform.position, transform.rotation);
        var proj = clone.GetComponent<Rigidbody2D>();
        proj.AddForce(direction.normalized * 10 * bulletSpeed);
        clone.name = "Enemy Bullet";
    }
    //the bullet child


}
