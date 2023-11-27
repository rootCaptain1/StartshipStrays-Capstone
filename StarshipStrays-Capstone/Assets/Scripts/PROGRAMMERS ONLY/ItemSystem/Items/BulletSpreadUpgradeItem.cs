using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(menuName = "Item/Bullet Spread Upgrade")]
    public class BulletSpreadUpgradeItem : ItemBase
    {
        public int spreadUpgradeAmount;

        public override bool CanUse(PlayerController player) => true;

        public override void Use(PlayerController player)
        {
            // Add more bullets fired to the players shots
            player.UpgradeBulletSpread(spreadUpgradeAmount);
            Debug.Log("Adding more firepower! (More rounds per shot)");
        }
    }
}