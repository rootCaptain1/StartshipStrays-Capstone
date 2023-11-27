using UnityEngine;

namespace ItemSystem
{
    public class ItemSpawnPoint : MonoBehaviour
    {
        [SerializeField] private bool _isStoreItem;
        // Start is called before the first frame update
        void Start()
        {
            ItemManager itemManager = FindObjectOfType<ItemManager>();
            if (itemManager != null)
            {
                itemManager.SpawnItem(transform.position, Quaternion.identity, _isStoreItem);
            }
        }
    }
}