using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICore
{
  [System.Serializable]
  public class Target
  {
    private Vector3 _position;
    private Collider2D _collider;
    private float _distance;
    private float _time;
    [SerializeField] private TargetType _targetType;

    public Vector3 GetPosition { get { return _position; } }
    public Collider2D GetCollider2D { get { return _collider; } }
    public float Distance { get { return _distance; } set { _distance = value; } }
    public float GetTime { get { return _time; } }
    public TargetType GetTargetType { get { return _targetType; } }


    public void Set(Vector3 p, Collider2D c, float d, float t, TargetType tt)
    {
      _position = p;
      _collider = c;
      _distance = d;
      _time = t;
      _targetType = tt;
    }

    public void Clear()
    {
      _position = Vector3.zero;
      _collider = null;
      _distance = float.MaxValue; //If we say the distance is 0 then that means we have reached our destination
      _time = 0f;
      _targetType = TargetType.None;
    }
  }
    public enum TargetType
    {
      None,
      Visual,
      Waypoint
    }
}
