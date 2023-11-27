using UnityEngine;

namespace PuzzleSystem
{
    public class PressureButton : Switch
    {
        private Color _baseColor;

        private void Start()
        {
            _puzzleManager = GetComponentInParent<SwitchPuzzleManager>();
            _switchSR = GetComponent<SpriteRenderer>();
            if(_switchSR != null)
            {
                _baseColor = _switchSR.color;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(_switchSR == null)
            {
                return;
            }

            if (!Active && collision.CompareTag("Player"))
            {
                TurnOn();
            }
            else if (Active && collision.CompareTag("Player"))
            {   
                TurnOff();
            }
        }

        public override void TurnOff()
        {
            // Switch sprite, change color, play sound, and/or play animation
            base.TurnOff();
            _switchSR.color = _baseColor;
            Notify(false);
        }

        public override void TurnOn()
        {
            // Switch sprite, change color, play sound, and/or play animation
            base.TurnOn();
            _switchSR.color = Color.grey;
            Notify(true);
        }
    }
}