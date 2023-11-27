using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(menuName = "Item/Coin")]
    public class CoinItem : ItemBase
    {
        public int coinWorth;
        public override bool CanUse(PlayerController player) => true;

        public override void Use(PlayerController player)
        {
            player.PickupMoney(coinWorth);
            Debug.Log("Picked up some money!");
        }
    }
}