using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGeneration
{
    public enum RoomType
    {
        Regular = 1,
        Starting = 2,
        Store = 3,
        Puzzle = 4,
        Prize = 5,
        Boss = 6
    }

    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomType _roomType;                // Stores the type of room that this room is (e.g., Boss, Starting, Prize)

        [Header("Gizmo Information")]
        [SerializeField] private Vector2 _bounds;                   // Stores the size of the room
        [SerializeField] private Color _boundsColor;                // Stores the color for the bounds gizmo
        [SerializeField] private bool _drawGizmos;                  // Allows the user to decide if they want to show the gizmos or not

        private List<Door> _doors = new List<Door>();
        private Dictionary<Direction, Room> _adjacentRooms = new Dictionary<Direction, Room>();
        private bool hasVisitedRoom = false;

        public event Action RoomEnteredEvent;

        public RoomType RoomType => _roomType;                      // Get the rooms type
        public Vector2 RoomPosition { get; set; }
        public int EnemyCount { get; private set; }

        private void OnEnable()
        {
            DungeonGenerator.instance.OnGeneratorStop += InitializeRoomInfo;
            EnemyCount = GetComponentsInChildren<EnemySpawning.EnemySpawner>().Count();
        }
        private void OnDisable()
        {
            DungeonGenerator.instance.OnGeneratorStop -= InitializeRoomInfo;
        }

        private void InitializeRoomInfo()
        {
            SetAdjacentRooms();
            _doors = GetComponentsInChildren<Door>().ToList();

            if (_roomType == RoomType.Puzzle)
            {
                // Lock the puzzle room door to the prize room
                GetDoorToAdjacentRoom(RoomType.Prize).LockDoor();
            }
        }
        public void EnemyKilled()
        {
            EnemyCount--;
            // Check if all enemies are dead
            if (EnemyCount <= 0)
            {
                UnlockDoors(); // Unlock the doors when all enemies are dead
            }
        }

        private void SetAdjacentRooms()
        {
            // Obtain the list of rooms in the dungeon
            List<Room> dungeonRooms = FindObjectsByType<Room>(FindObjectsSortMode.None).ToList();

            foreach (Direction direction in DungeonCrawlerController.directionMovementMap.Keys)
            {
                // Obtain the position of the adjacent room
                Vector2 adjacentRoomPosition = RoomPosition + DungeonCrawlerController.directionMovementMap[direction];

                // Find the adjacent room from the list of rooms
                Room adjacentRoom = dungeonRooms.FirstOrDefault(room => room.RoomPosition == adjacentRoomPosition);

                // Check if the adjacent room is not null before adding it
                if (adjacentRoom != null)
                {
                    _adjacentRooms.Add(direction, adjacentRoom);
                }
            }
        }

        public void RemoveDoors(List<Direction> nonAdjacentRooms)
        {
            // Obtain the doors that will need to be deleted
            // Check to make sure the door is not null
            List<Door> doorsToRemove = GetComponentsInChildren<Door>()
                .Where(door => door != null && nonAdjacentRooms.Contains(door.DoorDirection))
                .ToList();

            // Delete the doors
            foreach (Door doorToRemove in doorsToRemove)
            {
                Destroy(doorToRemove.gameObject);
            }
        }

        private void LockDoors()
        {
            foreach (Door door in _doors)
            {
                door.LockDoor();
            }
        }

        private void UnlockDoors()
        {
            foreach (Door door in _doors)
            {
                door.UnlockDoor();
            }
        }

        public Room GetAdjacentRoom(Direction direction)
        {
            return _adjacentRooms[direction];
        }

        public Room GetAdjacentRoom(RoomType roomType)
        {
            foreach (Direction direction in _adjacentRooms.Keys)
            {
                Room adjacentRoom = _adjacentRooms[direction];
                if (adjacentRoom.RoomType == roomType)
                {
                    return adjacentRoom;
                }
            }

            return null; // Return null if no adjacent room with the specified type is found.
        }

        public Door GetDoorToAdjacentRoom(RoomType roomType)
        {
            foreach (Direction direction in _adjacentRooms.Keys)
            {
                Room adjacentRoom = _adjacentRooms[direction];
                if (adjacentRoom.RoomType == roomType)
                {
                    Door adjacentDoor = GetChildDoor(direction);
                    return adjacentDoor;
                }
            }

            return null; // Return null if no door to the specified adjacent room is found.
        }

        public Door GetChildDoor(Direction direction)
        {
            // Obtain the doors attached to this object
            List<Door> doors = GetComponentsInChildren<Door>().ToList();

            // Return the door found with the correct direction
            return doors.First(door => door.DoorDirection == direction);
        }

        public void EnterRoom()
        {
            if (hasVisitedRoom == false)
            {
                RoomEnteredEvent?.Invoke();

                // Spawn the enemies
                if(EnemyCount > 0)
                {
                    LockDoors();
                }
                hasVisitedRoom = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
                return;

            DrawRoomBounds();
        }

        private void DrawRoomBounds()
        {
            // Set the color for room bounds
            Gizmos.color = _boundsColor;

            // Draw the bounds wire cube
            Gizmos.DrawWireCube(transform.position, _bounds);
        }
    }
}