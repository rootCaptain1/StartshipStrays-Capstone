using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(menuName = "Item/Heal Item")]
    public class RefillHealthItem : ItemBase
    {
        public int healAmount;

        public override bool CanUse(PlayerController player)
        {
            if(player.currentHealth >= player.maxHealth)
            {
                Debug.Log("I don't need more health");
                return false;
            }
            return true;
        }

        public override void Use(PlayerController player)
        { 
            // Call the player.heal or add health to the player in some way
            player.Heal(healAmount);
            Debug.Log("Healing!");
        }
    }
}