using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemSystem
{
    public class StoreManager : MonoBehaviour
    {
        public static StoreManager instance;

        private List<ItemSpawnPoint> storeItems = new List<ItemSpawnPoint>();

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            storeItems = GetComponentsInChildren<ItemSpawnPoint>().ToList();
        }

        public bool BuyItem()
        {
            return true;
        }
    }
}