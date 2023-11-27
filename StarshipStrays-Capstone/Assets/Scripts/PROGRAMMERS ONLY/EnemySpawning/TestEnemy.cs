using System;
using UnityEngine;

namespace EnemySpawning
{
    public class TestEnemy : MonoBehaviour, IDestructable
    {
        public event Action OnDestroy;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                OnDestroy?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}