using DungeonGeneration;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace ItemSystem
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private List<GameObject> items = new List<GameObject>();
        List<GameObject> spawnedItems = new List<GameObject>();
        
        private void Start()
        {
            DungeonGenerator.instance.OnGeneratorReset += ClearObjectPools;
        }
        private void OnDisable()
        {
            DungeonGenerator.instance.OnGeneratorReset -= ClearObjectPools;
        }

        private void ClearObjectPools()
        {
            foreach(GameObject item in spawnedItems)
            {
                Destroy(item);
            }
            spawnedItems.Clear();
            Utilities.ObjectPool.ClearPools();
        }

        public void SpawnCoin(Vector3 spawnPosition)
        {
            SpawnItemInternal(_coinPrefab, spawnPosition, Quaternion.identity);
        }

        public GameObject SpawnItem(Vector3 spawnPosition, Quaternion rotation, bool isStoreItem = false)
        {
            int rng = Random.Range(0, items.Count);
            GameObject objectToSpawn = items[rng];
            return SpawnItemInternal(objectToSpawn, spawnPosition, rotation, isStoreItem);
        }

        public void SpawnItem(ItemBase itemBase, Vector3 spawnPosition, Quaternion rotation, bool isStoreItem = false)
        {
            GameObject objectToSpawn = GetGameObjectFromItemBase(itemBase);
            if (objectToSpawn != null)
            {
                SpawnItemInternal(objectToSpawn, spawnPosition, rotation);
            }
        }

        private GameObject SpawnItemInternal(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion rotation, bool isStoreItem = false)
        {
            GameObject spawnedItem = Utilities.ObjectPool.Spawn(objectToSpawn, spawnPosition, rotation);
            spawnedItem.transform.parent = transform;
            Pickup pickup = spawnedItem.GetComponent<Pickup>();
            if (pickup != null)
            {
                pickup.IsStoreItem = isStoreItem; // Flag the item as a store item
            }
            spawnedItems.Add(spawnedItem);
            return spawnedItem;
        }

        private GameObject GetGameObjectFromItemBase(ItemBase itemBase)
        {
            foreach (GameObject item in spawnedItems)
            {
                Pickup pickup = item.GetComponent<Pickup>();
                if (pickup != null && pickup.Item == itemBase)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
