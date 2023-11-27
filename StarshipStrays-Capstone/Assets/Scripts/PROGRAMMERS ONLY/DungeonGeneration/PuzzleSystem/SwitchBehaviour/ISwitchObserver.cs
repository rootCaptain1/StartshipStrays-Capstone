namespace PuzzleSystem
{
    public delegate void SwitchActivated(bool isActive);

    public interface ISwitchObserver
    {
        event SwitchActivated OnSwitchActivated;
    }
}