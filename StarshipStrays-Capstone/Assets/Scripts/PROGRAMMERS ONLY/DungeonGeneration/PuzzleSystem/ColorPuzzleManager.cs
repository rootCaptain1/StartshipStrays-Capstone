using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace PuzzleSystem
{
    public class ColorPuzzleManager : BasePuzzleManager
    {
        [Header("Objects")]
        [SerializeField] private SpriteRenderer _colorToReplicate;
        [SerializeField] private SpriteRenderer _targetColorObject;
        private List<SpriteRenderer> _buttons = new List<SpriteRenderer>();
        private GridManager _gridManager;

        public Color TargetColorObjectColor { get { return _targetColorObject.color; } set { _targetColorObject.color = new Color(value.r, value.g, value.b, 1); } }

        private void Start()
        {
            _gridManager = GetComponent<GridManager>();
            _gridManager?.GenerateGrid();
            InitButtons();
        }

        private void InitButtons()
        {
            _buttons = GetComponentsInChildren<ColorButton>()
                    .Select(button => button.GetComponent<SpriteRenderer>())
                    .Where(renderer => renderer != null)
                    .ToList();
            InitButtonColors();
        }

        private void InitButtonColors()
        {
            Color color1 = GetRandomColor();
            Color color2 = GetRandomColor();
            Color replicateColor = new Color(
                color1.r + color2.r,
                color1.g + color2.g,
                color1.b + color2.b,
                1);

            SetButtonColors(color1, color2, replicateColor);
        }

        private void SetButtonColors(Color color1, Color color2, Color replicateColor)
        {
            foreach (SpriteRenderer button in _buttons)
            {
                if (button != null)
                {
                    button.color = GetRandomColor();
                }
            }

            int firstButtonIndex = Random.Range(0, _buttons.Count);
            int secondButtonIndex;
            do
            {
                secondButtonIndex = Random.Range(0, _buttons.Count);
            } while (secondButtonIndex == firstButtonIndex);

            _colorToReplicate.color = replicateColor;
            _buttons[firstButtonIndex].color = color1;
            _buttons[secondButtonIndex].color = color2;

            // TESTING
            _buttons[firstButtonIndex].name = "1";
            _buttons[secondButtonIndex].name = "2";
        }

        private Color GetRandomColor()
        {
            float randomR = Random.Range(0.1f, .6f) * 10 / 10f;
            float randomG = Random.Range(0.1f, .6f) * 10 / 10f;
            float randomB = Random.Range(0.1f, .6f) * 10 / 10f;

            return new Color(randomR, randomG, randomB);
        }

        private bool ColorsApproximatelyEqual(Color color1, Color color2)
        {
            // Convert colors to hexadecimal representation
            string hexColor1 = ColorToHex(color1);
            string hexColor2 = ColorToHex(color2);

            // Compare the hexadecimal representations
            return hexColor1.Equals(hexColor2);
        }

        private string ColorToHex(Color color)
        {
            // Convert each color component (R, G, B) to its hexadecimal representation
            int r = Mathf.RoundToInt(color.r * 255);
            int g = Mathf.RoundToInt(color.g * 255);
            int b = Mathf.RoundToInt(color.b * 255);

            return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        public override void CheckPuzzle()
        {
            if (ColorsApproximatelyEqual(_colorToReplicate.color, _targetColorObject.color))
            {
                CompletePuzzle();
            }
        }

        public override void ResetSwitches()
        {
            _targetColorObject.color = Color.black;
            foreach(SpriteRenderer buttonRenderer in _buttons)
            {
                Switch sw = buttonRenderer.GetComponent<Switch>();
                if (sw != null)
                {
                    sw.TurnOff();
                }
            }
        }
    }
}