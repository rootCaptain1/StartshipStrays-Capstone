using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Canine")]
[System.Serializable]
public class SO_Canine : ScriptableObject
{
  public new string name;
  //public float age;
  public float speed;

  public Breed breed;
  public Boost boost;

  public int healthBoost;
  public int speedBoost;
  public int damageBoost;

  public enum Breed
  {
    AustralianShepard,
    AustralianKelpiepe,
    SharPei,
  }

  public enum Boost
  {
    None,
    Health,
    Speed,
    Damage
  }
}
