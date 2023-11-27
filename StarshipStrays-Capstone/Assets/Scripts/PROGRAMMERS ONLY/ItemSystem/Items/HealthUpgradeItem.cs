using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(menuName = "Item/Health Upgrade")]
    public class HealthUpgradeItem : ItemBase
    {
        public int healthUpgradeAmount;

        public override bool CanUse(PlayerController player) => true;

        public override void Use(PlayerController player)
        {
            // Upgrade the players max health in some way
            player.UpgradeMaxHealth(healthUpgradeAmount);
            Debug.Log("Upgrading health!");
        }
    }
}