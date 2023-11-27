using UnityEngine;

namespace PuzzleSystem
{
    public class ColorButton : Switch
    {
        private void Start()
        {
            _puzzleManager = GetComponentInParent<ColorPuzzleManager>();
            _switchSR = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ColorPuzzleManager puzzleManager = _puzzleManager as ColorPuzzleManager;
            if (puzzleManager == null || _switchSR == null)
            {
                return;
            }

            if (!Active && collision.CompareTag("Player"))
            {
                puzzleManager.TargetColorObjectColor += _switchSR.color;
                TurnOn();
                Notify(true);
            }
            else if(Active && collision.CompareTag("Player"))
            {
                puzzleManager.TargetColorObjectColor -= _switchSR.color;
                TurnOff();
                Notify(false);
            }
        }
    }
}
