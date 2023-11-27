using UnityEngine;

namespace PuzzleSystem
{
    public class ResetButton : MonoBehaviour
    {
        [SerializeField] private BasePuzzleManager _puzzleManager;

        private void Start()
        {
            if(_puzzleManager == null)
            {
                _puzzleManager = GetComponentInParent<BasePuzzleManager>();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _puzzleManager.ResetSwitches();
            }
        }
    }
}