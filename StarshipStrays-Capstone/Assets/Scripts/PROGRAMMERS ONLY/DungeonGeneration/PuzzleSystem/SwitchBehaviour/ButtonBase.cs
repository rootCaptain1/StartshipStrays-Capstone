using Unity.VisualScripting;
using UnityEngine;

namespace PuzzleSystem
{
    public abstract class ButtonBase : Switch
    {
        protected SpriteRenderer _buttonSR;
        private void Start()
        {
            _puzzleManager = GetComponentInParent<BasePuzzleManager>();
            _buttonSR = GetComponent<SpriteRenderer>();
        }
    }
}