
using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDestructable
{
    public SO_Enemy _enemy;
    private int _playerDamage;

    private readonly SO_Enemy.EnemyTypes _enemyType;
    private readonly SO_Enemy.WeaponType _weaponType;

    public event Action OnDestroy;

    // Start is called before the first frame update
    void Start()
    {
        //Creates new objects for each enemy so there information is not the same across the board
        //For example one enemy will have 8 health when hit and the other will have 10
        _enemy = ScriptableObject.CreateInstance<SO_Enemy>();

        var _player = FindAnyObjectByType<PlayerController>(); //Get the players damage
        _playerDamage = _player.damage;

        InitType();
        WeaponCheck();
        HealthCheck();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Used to double check that the enemy is being used exists
    /// </summary>
    private void InitType()
    {
        if (_enemy == null)
        {
            Debug.Log("Enemy is null");
            return;
        }
    }

    /// <summary>
    /// Double checks enemy health so it is the same across the board
    /// </summary>
    private void HealthCheck()
    {
        if (_enemyType == SO_Enemy.EnemyTypes.Common)
        {
            _enemy.health = 10;
        }
        else if (_enemyType == SO_Enemy.EnemyTypes.Heavy)
        {
            _enemy.health = 15;
        }
        else if (_enemyType == SO_Enemy.EnemyTypes.Range)
        {
            _enemy.health = 10;
        }
        else if (_enemyType == SO_Enemy.EnemyTypes.Boss)
        {
            _enemy.health = 50;
        }
        //Should never be reached but put in place in case a new type is added and not updated here
        else
        {
            _enemy.health = 10;
        }
    }

    /// <summary>
    /// Checks different combinations and sees if anything weird pops up
    /// </summary>
    private void WeaponCheck()
    {
        //Common
        if (_enemyType == SO_Enemy.EnemyTypes.Common && _weaponType == SO_Enemy.WeaponType.Melee)
        {
            _enemy.damage = 1;
        }
        else if (_enemyType == SO_Enemy.EnemyTypes.Common && _weaponType == SO_Enemy.WeaponType.Range)
        {
            _enemy.damage = 1;
        }

        //Heavy
        if (_enemyType == SO_Enemy.EnemyTypes.Heavy && _weaponType == SO_Enemy.WeaponType.Melee)
        {
            _enemy.damage = 3;
        }
        //Dummy check
        else if (_enemyType == SO_Enemy.EnemyTypes.Heavy && _weaponType == SO_Enemy.WeaponType.Range)
        {
            Debug.LogWarning("Heavy enemy using range weapon!");
            _enemy.damage = 3;
        }

        //Range
        if (_enemyType == SO_Enemy.EnemyTypes.Range && _weaponType == SO_Enemy.WeaponType.Range)
        {
            _enemy.damage = 1;
        }
        //Dummy Check
        else if (_enemyType == SO_Enemy.EnemyTypes.Range && _weaponType == SO_Enemy.WeaponType.Melee)
        {
            Debug.LogWarning("Range enemy using melee weapon!");
            _enemy.damage = 1;
        }

        //Boss
        if (_enemyType == SO_Enemy.EnemyTypes.Boss && _weaponType == SO_Enemy.WeaponType.Melee)
        {
            _enemy.damage = 5;
        }
        else if (_enemyType == SO_Enemy.EnemyTypes.Boss && _weaponType == SO_Enemy.WeaponType.Range)
        {
            _enemy.damage = 5;
        }

        else
        {
            _enemy.damage = 2;
        }
    }

    /// <summary>
    /// Damage the enemy once they get hit by the bullet
    /// The damage being dealt is dependent on the players damage amount
    /// </summary>
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player Bullet"))
        {
            _enemy.health -= _playerDamage;
            Destroy(other.gameObject);

            if (_enemy.health == 0)
            {
                OnDestroy?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}