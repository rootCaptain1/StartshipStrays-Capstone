using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(SpriteRenderer))]
public class CanineBase : MonoBehaviour
{
  public SO_Canine canine;
  public GameObject caninePrefab;
  public SpriteRenderer canineSprite;

  private int _playerHealth;
  private int _playerSpeed;
  private int _playerDamage;

  // Start is called before the first frame update
  void Start()
  {
    //Get player variables from player
    var _player = FindAnyObjectByType<TestPlayerController>(); 
    _playerHealth = _player.maxHealth;
    _playerSpeed = _player.movementSpeed;
    _playerDamage = _player.damage;

    CanineCheck();
    BoosterCheck();
  }

  // Update is called once per frame
  void Update()
  {

  }

  //Check that the canine prefab is set
  void CanineCheck()
  {
    if(caninePrefab == null)
    {
      Debug.Log("Dog on the loose!");
      return;
    }
  }

  //Check what boost the player will receive from the canine
  int BoosterCheck()
  {
    if (canine.boost == SO_Canine.Boost.Speed)
    {
      Debug.Log("Player speed is up!");
      return _playerSpeed += canine.speedBoost;
    }
    else if (canine.boost == SO_Canine.Boost.Damage)
    {
      Debug.Log("Player damage is up!");
      return _playerDamage += canine.damageBoost;
    }
    else if (canine.boost == SO_Canine.Boost.Health)
    {
      Debug.Log("Player health is up!");
      return _playerHealth += canine.healthBoost;
    }
    else //Check
    {
      Debug.Log("Player does not get buff");
    }
    return 0;
  }
}