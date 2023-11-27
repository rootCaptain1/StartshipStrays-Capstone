using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PuzzleSystem
{
    public abstract class Switch : MonoBehaviour, ISwitchObserver
    {
        protected BasePuzzleManager _puzzleManager;
        protected SpriteRenderer _switchSR;

        public event SwitchActivated OnSwitchActivated;

        public bool Active { get; private set; } = false;

        public virtual void TurnOn()
        {
            // Play animation (if any)
            // Play sound (if any)
            // Set the switch to be active
            Active = true;
            _puzzleManager.CheckPuzzle();
        }
        public virtual void TurnOff()
        {
            // Play animation (if any)
            // Play sound (if any)
            // Set the switch to be inactive
            Active = false;
        }

        protected void Notify(bool isActive)
        {
            OnSwitchActivated?.Invoke(isActive);
        }
    }
}