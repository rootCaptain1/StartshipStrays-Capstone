namespace ItemSystem
{
    public interface IUseable
    {
        public bool CanUse(PlayerController player);
        public void Use(PlayerController player);
    }
}