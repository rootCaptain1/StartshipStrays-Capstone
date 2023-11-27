using UnityEngine;
using System.Collections.Generic;
using PuzzleSystem;

namespace DungeonGeneration
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Door : MonoBehaviour
    {
        [SerializeField] private Direction _doorDirection; // The direction in which the door is facing
        private Room _parentRoom; // The room to which this door belongs
        private BoxCollider2D _collider;
        public bool _locked;
        private Color _doorColor;

        // Dictionary to map directions to their opposites
        private static readonly Dictionary<Direction, Direction> OppositeDirections = new Dictionary<Direction, Direction>
        {
            { Direction.Left, Direction.Right },
            { Direction.Right, Direction.Left },
            { Direction.Up, Direction.Down },
            { Direction.Down, Direction.Up }
        };

        // Property to access the door direction
        public Direction DoorDirection => _doorDirection;

        private void Start()
        {
            // Get the parent room of this door
            _parentRoom = GetComponentInParent<Room>();
            _collider = GetComponent<BoxCollider2D>();
            _doorColor = GetComponent<SpriteRenderer>().color;
        }

        private void Update()
        {
            if (_locked)
            {
                _collider.isTrigger = false;

                // TESTING (Animation will play here to lock the door)
                GetComponent<SpriteRenderer>().color = Color.gray;
            }
            else
            {
                _collider.isTrigger = true;

                // TESTING (Animation will play here to unlock the door)
                GetComponent<SpriteRenderer>().color = _doorColor;
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the entering object is tagged as "Player"
            if (other.CompareTag("Player"))
            {
                // Calculate the new position for the player after passing through the door
                Vector3 displacementPosition = GetDisplacementPosition(other, _doorDirection);
                other.transform.position = displacementPosition;

                // Move the camera to focus on the new room
                CameraController.instance.MoveCamera(_doorDirection);
                _parentRoom.GetAdjacentRoom(_doorDirection).EnterRoom();
            }
        }

        // Calculate the new position for an object after passing through the door
        private Vector3 GetDisplacementPosition(Collider2D objectCollider, Direction direction)
        {
            // Initialize the displacement position
            Vector3 displacementPosition = Vector3.zero;

            // Get the adjacent room based on the direction
            Room adjacentRoom = _parentRoom.GetAdjacentRoom(direction);

            // Get the adjacent door in the adjacent room
            Door adjacentDoor = adjacentRoom?.GetChildDoor(OppositeDirections[direction]);

            // Calculate the position based on the adjacent door's position and size
            Vector2Int multiplier = new Vector2Int(GetXMultiplier(direction), GetYMultiplier(direction));

            if (adjacentDoor != null)
            {
                // Get the size (half extents) of the collider of the adjacent door
                Vector2 doorExtents = adjacentDoor.GetComponent<Collider2D>().bounds.extents;

                // Get the size (half extents) of the collider of the object (e.g., player) that is passing through the door
                Vector2 objectExtents = objectCollider.GetComponent<Collider2D>().bounds.extents;

                // Calculate the new position for the object after passing through the door:
                // - Calculate the X coordinate: Start with the X coordinate of the adjacent door, then add or subtract based on the direction
                //   and the combined extents of the door and the object. A small offset (0.1f) is added for separation
                // - Calculate the Y coordinate in a similar way based on the Y coordinate of the adjacent door
                // - The Z coordinate is set to 0 since this is a 2D game
                displacementPosition = new Vector3(
                    adjacentDoor.transform.position.x + multiplier.x * (doorExtents.x + objectExtents.x + 0.1f),
                    adjacentDoor.transform.position.y + multiplier.y * (doorExtents.y + objectExtents.y + 0.1f),
                    0
                );
            }

            return displacementPosition;
        }

        public void LockDoor() => _locked = true;
        public void UnlockDoor() => _locked = false;

        // Helper function to get the X-axis multiplier based on the direction
        private int GetXMultiplier(Direction direction)
        {
            return direction == Direction.Left ? -1 : (direction == Direction.Right ? 1 : 0);
        }

        // Helper function to get the Y-axis multiplier based on the direction
        private int GetYMultiplier(Direction direction)
        {
            return direction == Direction.Down ? -1 : (direction == Direction.Up ? 1 : 0);
        }
    }
}
