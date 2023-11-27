using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy")]
[System.Serializable]
public class SO_Enemy : ScriptableObject
  {
    public int health;
    public int damage;
    public int speed;

    public WeaponType weaponType;
    public EnemyTypes enemyTypes;

    public enum EnemyTypes
    {
      Common,
      Heavy,
      Range,
      Boss
    }

    public enum WeaponType
    {
      Melee,
      Range
    }
  }
