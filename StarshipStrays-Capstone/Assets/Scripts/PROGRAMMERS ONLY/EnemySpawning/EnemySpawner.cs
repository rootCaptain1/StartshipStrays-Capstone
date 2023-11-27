using System.Collections.Generic;
using UnityEngine;
using DungeonGeneration;
using ItemSystem;

namespace EnemySpawning
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
        [SerializeField][Range(1, 100)] private int randomChance = 10;
        private Room _parentRoom;
        private GameObject _spawnedEnemy;
        private IDestructable _enemyDestructable;

        private void OnEnable()
        {
            _parentRoom = GetComponentInParent<Room>();

            if (_parentRoom != null)
            {
                _parentRoom.RoomEnteredEvent += SpawnEnemy;
            }
        }
        private void OnDisable()
        {
            if (_parentRoom != null)
            {
                _parentRoom.RoomEnteredEvent -= SpawnEnemy;
            }
        }

        private void SpawnEnemy()
        {
            _spawnedEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position, Quaternion.identity);

            _enemyDestructable = _spawnedEnemy.GetComponent<IDestructable>();
            if (_enemyDestructable != null)
            {
                _enemyDestructable.OnDestroy += OnEnemyDestroyed;
            }
        }

        private void OnEnemyDestroyed()
        {
            _parentRoom.EnemyKilled(); // Notify the room that an enemy is killed

            // Spawn the item in the center of the room based on a random chance
            int randomNum = Random.Range(1, 101);
            // Spawn the item using the item manager
            ItemManager itemManager = FindObjectOfType<ItemManager>();
            Vector3 itemSpawnPosition = _spawnedEnemy.transform.position;
                        
            if (_parentRoom.EnemyCount <= 0)
            {
                if (itemManager != null)
                {
                    if (randomNum <= randomChance)
                    {
                        itemManager.SpawnItem(_parentRoom.transform.position, Quaternion.identity);

                        // Play animation or particle system to have item appear
                    }
                }
            }
            else
            {
                if (randomNum <= randomChance * 2)
                {
                    itemManager.SpawnCoin(itemSpawnPosition);
                }
            }

            // Unsubscribe from the event when the enemy is destroyed
            _enemyDestructable.OnDestroy -= OnEnemyDestroyed;
        }
    }
}