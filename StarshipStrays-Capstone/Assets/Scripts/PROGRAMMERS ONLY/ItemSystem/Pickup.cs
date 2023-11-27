using UnityEngine;

namespace ItemSystem
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private ItemBase item;
        private PlayerController _player;

        public bool IsStoreItem { get; set; } = false;
        public ItemBase Item { get { return item; } }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _player = collision.GetComponent<PlayerController>();
            if (collision.tag.Equals("Player"))
            {
                if (_player != null && item.CanUse(_player))
                {
                    if (_player.Money >= item.itemStoreCost && IsStoreItem)
                    {
                        Debug.Log("Purchased");
                        _player.ReduceMoney(item.itemStoreCost);
                    }
                    else if (IsStoreItem)
                    {
                        Debug.Log("Got no money");
                        return;
                    }
                    Debug.Log("Item used");
                    item.Use(_player);
                    Utilities.ObjectPool.Despawn(gameObject);
                }
            }
        }
    }
}