using UnityEngine;

namespace ItemSystem
{
    public abstract class ItemBase : ScriptableObject, IUseable
    {
        public int itemStoreCost;
        public abstract bool CanUse(PlayerController player);
        public abstract void Use(PlayerController player);
    }
}