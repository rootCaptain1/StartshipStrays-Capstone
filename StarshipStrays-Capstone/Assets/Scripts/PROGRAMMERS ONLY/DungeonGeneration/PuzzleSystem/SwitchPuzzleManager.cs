using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PuzzleSystem
{
    public class SwitchPuzzleManager : BasePuzzleManager
    {
        private List<Switch> _switches = new List<Switch>();

        public List<Switch> Switches { get { return _switches; } }

        private void Start()
        {
            _switches = GetComponentsInChildren<Switch>().ToList();
        }

        public override void CheckPuzzle()
        {
            foreach (Switch sw in _switches)
            {
                if (!sw.Active)
                {
                    return;
                }
            }
            CompletePuzzle();
        }

        public override void ResetSwitches()
        {
            foreach(Switch sw in _switches)
            {
                sw.TurnOff();
            }
        }
    }
}