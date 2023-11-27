using UnityEngine;
using DungeonGeneration;
using System.Collections.Generic;
using System.Linq;

namespace PuzzleSystem
{
    public abstract class BasePuzzleManager : MonoBehaviour
    {
        private Door _puzzleDoorToPrizeRoom;

        private void OnEnable()
        {
            DungeonGenerator.instance.OnGeneratorStop += InitPuzzleDoor;
        }

        private void OnDisable()
        {
            DungeonGenerator.instance.OnGeneratorStop -= InitPuzzleDoor;
        }

        private void InitPuzzleDoor()
        {
            Room puzzleRoom = GetComponentInParent<Room>();
            _puzzleDoorToPrizeRoom = puzzleRoom.GetDoorToAdjacentRoom(RoomType.Prize);
        }

        public abstract void CheckPuzzle();
        public abstract void ResetSwitches();

        protected void CompletePuzzle()
        {
            OpenPrizeRoom();
        }

        private void OpenPrizeRoom()
        {
            if(_puzzleDoorToPrizeRoom != null)
            {
                _puzzleDoorToPrizeRoom.UnlockDoor();
            }
        }
    }
}