using System;

public interface IDestructable
{
    public event Action OnDestroy;
}
