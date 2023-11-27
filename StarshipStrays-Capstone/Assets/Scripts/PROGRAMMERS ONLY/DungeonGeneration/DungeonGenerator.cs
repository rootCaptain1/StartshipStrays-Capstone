using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGeneration
{
    public class DungeonGenerator : MonoBehaviour
    {
        public static DungeonGenerator instance;

        [Header("Dungeon Data")]
        [SerializeField] private DungeonGenerationData _dungeonGenerationData;  // Store the information that the generator will use

        [Header("Grid Information")]
        [Range(2, 100)][SerializeField] private int _gridX;                     // Stores the amount of positions away from the starting position on the X axis
        [Range(2, 100)][SerializeField] private int _gridY;                     // Stores the amount of positions away from the starting position on the Y axis
        [SerializeField] private bool _drawGrid;                                // Allows the user to decide if they want to see the grid or not
        [SerializeField] private Color _gridColor;                              // Stores the color of the grid

        private DungeonCrawlerController _controller;                           // Stores a new Dungeon Crawler Controller for future reference
        private List<Vector2> _dungeonRooms = new List<Vector2>();              // Stores the dungeon rooms that will be spawned and then removed upon spawn
        private List<Room> _takenRooms = new List<Room>();                      // Stores the rooms that have taken positions from _dungeonRooms positions

        private int _maxRegenerationAttempts = 3;

        public event Action OnGeneratorStop;
        public event Action OnGeneratorReset;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(gameObject);
            }

            _controller = new DungeonCrawlerController(_gridX, _gridY);
            StartCoroutine(GenerateDungeon());
        }

        private IEnumerator GenerateDungeon()
        {
            _dungeonRooms = _controller.GetDungeonRoomLocations(_dungeonGenerationData);

            // Spawn the starting room at 0, 0
            SpawnRoom(RoomType.Starting, Vector2.zero);

            // Spawn a boss room
            SpawnRoom(RoomType.Boss, GetBossRoom());

            // Spawn a store room
            SpawnRoom(RoomType.Store, GetRandomRoomPosition());

            // Go through the process of spawning both the puzzle and prize room
            do
            {
                //Spawn the puzzle room
                Vector2 puzzleRoomLocation = GetEdgeRoom();
                SpawnRoom(RoomType.Puzzle, puzzleRoomLocation);

                // Get an empty position next to the puzzle room
                List<Vector2> potentialPrizeRooms = GetAdjacentEmptyLocations(puzzleRoomLocation);

                // Make sure there is at least one room
                if (potentialPrizeRooms.Count > 0)
                {
                    // Use the first empty location next to the puzzle room
                    Vector2 prizeRoomLocation = potentialPrizeRooms[0];

                    // Spawn the prize room
                    SpawnRoom(RoomType.Prize, prizeRoomLocation);
                }
                else
                {
                    // Didn't find an empty adjacent location
                    Debug.LogError("No adjacent empty locations found for the prize room!");
                }
            } while (!_takenRooms.Any(room => room.RoomType == RoomType.Prize));

            // Spawn every other room in the remaining locations
            foreach (Vector2 roomLocation in _dungeonRooms)
            {
                // Make sure a regular room doesn't spawn where any of the special rooms are
                if (roomLocation == Vector2.zero) continue;

                // Spawn rooms for every location that the crawlers walked to
                SpawnRoom(RoomType.Regular, roomLocation);
            }

            RemoveDoors();

            // If there isn't enough spawn locations for the all of the room types or too many rooms are at Vector2.zero
            bool tooFewRooms = transform.childCount <= Enum.GetNames(typeof(RoomType)).Length;
            bool roomsSpawnedIncorrectly = _takenRooms.Where(room => room.RoomPosition == Vector2.zero).Count() > 1;
            if (tooFewRooms || roomsSpawnedIncorrectly)
            {
                if (_maxRegenerationAttempts > 0)
                {

                    if (tooFewRooms)
                    {
                        Debug.LogError("Too few rooms spawned! Room count: " + transform.childCount + " Remaking Dungeon.");
                    }
                    else if (roomsSpawnedIncorrectly)
                    {
                        Debug.LogWarning("Rooms spawned incorrectly! Rooms at Vector2.zero: " +
                            _takenRooms.Where(room => room.RoomPosition == Vector2.zero).Count() +
                            " Remaking Dungeon.");
                    }
                    // Clear the dungeon first
                    ClearDungeon();

                    // Wait for the current frame to finish
                    yield return null;
                    OnGeneratorReset?.Invoke();

                    // Decrease the regeneration attempts
                    _maxRegenerationAttempts--;

                    // Regenerate the dungeon
                    _dungeonRooms = _controller.GetDungeonRoomLocations(_dungeonGenerationData);
                    StartCoroutine(GenerateDungeon());
                    yield break;
                }
            }

            yield return null;
            OnGeneratorStop?.Invoke();
        }

        private void SpawnRoom(RoomType roomType, Vector2 location)
        {
            // Filter the room prefabs list using LINQ to select only the ones with the matching room type
            List<GameObject> availableRooms = _dungeonGenerationData.roomPrefabs
                            .Where(roomPrefab => roomPrefab.TryGetComponent(out Room newRoom) && newRoom.RoomType == roomType)
                            .ToList();

            // Check if we have any available rooms of the specified type
            if (availableRooms.Count > 0)
            {
                // Randomly select a room from the available rooms
                int randomIndex = UnityEngine.Random.Range(0, availableRooms.Count);
                GameObject selectedRoom = availableRooms[randomIndex];

                // Create the new object at the right location provided
                GameObject newRoomObject = Instantiate(selectedRoom, location, Quaternion.identity);
                Room newRoom = newRoomObject.GetComponent<Room>();
                newRoom.RoomPosition = location;
                _takenRooms.Add(newRoom);

                newRoomObject.transform.parent = transform;

                // Update the DungeonGrid with the GameObject reference
                _dungeonRooms = _controller.RemoveRoom(location);
            }
            else
            {
                Debug.Log("No available rooms of type " + roomType.ToString() + " found.");
            }
        }

        /// <summary>
        /// Gets any position in the list that is available for spawning
        /// </summary>
        private Vector2 GetRandomRoomPosition()
        {
            int index = UnityEngine.Random.Range(0, _dungeonRooms.Count - 1);
            return _dungeonRooms[index];
        }

        private Vector2 GetBossRoom()
        {
            // Starting position
            Vector2 startingPosition = Vector2.zero;
            // Queue for BFS traversal
            Queue<Vector2> queue = new Queue<Vector2>();
            // Dictionary to store parent-child relationships
            Dictionary<Vector2, Vector2> parentRooms = new Dictionary<Vector2, Vector2>();
            // Initialize furthest room to starting position
            Vector2 furthestRoom = startingPosition;
            // Track the single path room
            Vector2 singlePathRoom = Vector2.zero;

            // Start BFS traversal from the starting position
            queue.Enqueue(startingPosition);
            // Mark the starting room as its own parent
            parentRooms[startingPosition] = startingPosition;

            while (queue.Count > 0)
            {
                Vector2 currentRoom = queue.Dequeue();

                // Explore adjacent rooms
                foreach (Vector2 adjacentRoom in GetAdjacentRooms(currentRoom))
                {
                    if (!parentRooms.ContainsKey(adjacentRoom))
                    {
                        // Mark the current room as the parent of the adjacent room
                        parentRooms[adjacentRoom] = currentRoom;
                        // Enqueue the adjacent room for further exploration
                        queue.Enqueue(adjacentRoom);
                    }
                }
            }

            // Remove adjacent rooms to the starting room from consideration
            List<Vector2> adjacentRooms = GetAdjacentRooms(startingPosition);
            foreach (var room in adjacentRooms)
            {
                parentRooms.Remove(room);
            }

            // Traverse the parentRooms dictionary to find the furthest room and the single path room
            foreach (var kvp in parentRooms)
            {
                Vector2 roomLocation = kvp.Key;

                // Check if the current room has only one parent (i.e., part of a single path)
                if (GetAdjacentRooms(roomLocation).Count == 1)
                {
                    singlePathRoom = roomLocation;
                }

                // Check if the current room is further than the previously furthest room
                if (CalculateDistance(startingPosition, roomLocation) > CalculateDistance(startingPosition, furthestRoom))
                {
                    furthestRoom = roomLocation;
                }
            }

            // If the furthest room is on a single path, return the single path room; otherwise, return the furthest room
            if (singlePathRoom != Vector2.zero)
            {
                return singlePathRoom;
            }
            else
            {
                return furthestRoom;
            }
        }

        private List<Vector2> GetAdjacentRooms(Vector2 room)
        {
            // List to store the adjacent rooms
            List<Vector2> adjacentRooms = new List<Vector2>();

            // Go over each direction in our direction map
            foreach (Vector2 direction in DungeonCrawlerController.directionMovementMap.Values)
            {
                // Store the potential room position by adding the movement amount from the map
                Vector2 adjacentRoom = room + direction;

                // Check if the crawlers actually found that location and stored it
                // If so, add it to the list of adjacent rooms
                if (_dungeonRooms.Contains(adjacentRoom))
                    adjacentRooms.Add(adjacentRoom);
            }

            return adjacentRooms;
        }

        private List<Vector2> GetAdjacentEmptyLocations(Vector2 roomLocation)
        {
            // List to store the adjacent empty locations
            List<Vector2> adjacentEmptyLocations = new List<Vector2>();

            // Go over each direction in our direction map
            foreach (Vector2 direction in DungeonCrawlerController.directionMovementMap.Values)
            {
                // Calculate the potential empty location by adding the movement amount from the map
                Vector2 adjacentLocation = roomLocation + direction;

                // Check if the potential empty location is within the dungeon bounds and not already taken
                if (!_dungeonRooms.Contains(adjacentLocation) && !_takenRooms.Any(room => room.RoomPosition == adjacentLocation) && !IsRoomOnEdge(adjacentLocation))
                {
                    adjacentEmptyLocations.Add(adjacentLocation);
                }
            }

            return adjacentEmptyLocations;
        }

        private Vector2 GetEdgeRoom()
        {
            List<Vector2> edgeRooms = new List<Vector2>();

            foreach (Vector2 roomLocation in _dungeonRooms)
            {
                // Get adjacent empty locations for the current room
                List<Vector2> emptyRooms = GetAdjacentEmptyLocations(roomLocation);

                // Check if there are at least two empty adjacent locations
                if (emptyRooms.Count >= 2)
                {
                    edgeRooms.Add(roomLocation);
                }
            }

            if (edgeRooms.Count > 0)
            {
                int rng = UnityEngine.Random.Range(0, edgeRooms.Count);
                return edgeRooms[rng];
            }

            // Return a default location if no suitable edge room is found
            return Vector2.zero;
        }
        
        private bool IsRoomOnEdge(Vector2 adjacentLocation)
        {
            float gridWidth = DungeonCrawlerController.ROOMSIZE.x * _gridX;
            float gridHeight = DungeonCrawlerController.ROOMSIZE.y * _gridY;

            bool isOnSideEdge = Mathf.Abs(adjacentLocation.x) >= gridWidth;
            bool isOnTopOrBottom = Mathf.Abs(adjacentLocation.y) >= gridHeight;

            return isOnSideEdge || isOnTopOrBottom;
        }

        private void ClearDungeon()
        {
            // Remove all of the rooms that were spawned
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Clear the dungeon rooms and rooms taken
            _dungeonRooms.Clear();
            _takenRooms.Clear();
        }

        private void RemoveDoors()
        {
            foreach (Room room in _takenRooms)
            {
                // Get all non adjacent doors directions for the room
                List<Direction> nonAdjacentDoors = GetDoorsForRemoval(room.RoomPosition);

                // Remove the doors connect to the room that is not connected to an adjacent room
                room.RemoveDoors(nonAdjacentDoors);
            }
        }

        private List<Direction> GetDoorsForRemoval(Vector2 currentRoom)
        {
            // Used to store the directions poitning towards an adjacent room
            List<Direction> nonAdjacentRoomDirections = new List<Direction>();
            // Obtain the puzzle rooms position for later use
            Vector2 puzzleRoomPosition = _takenRooms.FirstOrDefault(room => room.RoomType == RoomType.Puzzle)?.RoomPosition ?? Vector2.zero;

            // Go through every direction we have in our map
            foreach (Direction direction in DungeonCrawlerController.directionMovementMap.Keys)
            {
                // Store the location of the sum of the current room and the movement amount
                Vector2 adjacentRoom = currentRoom + DungeonCrawlerController.directionMovementMap[direction];

                // Used to store if the room found is adjacent to our current room
                bool isAdjacent = false;

                // Go through every room in our dungeon
                foreach (Room room in _takenRooms)
                {
                    // Check if the room is the adjacent room
                    if (room.RoomPosition == adjacentRoom)
                    {
                        // Remove the doors that are connected to the prize room if they are not the puzzle room (NOT THE PUZZLE ROOM DOORS)
                        if (room.RoomType == RoomType.Prize &&
                            !(_takenRooms.Any(r => r.RoomPosition == currentRoom && r.RoomType == RoomType.Puzzle)))
                        {
                            // Remove the door
                            break;
                        }

                        // Keep the door
                        isAdjacent = true;
                        break;
                    }
                }

                // Remove the prize room doors that aren't connected to the puzzle room (THE PUZZLE ROOM DOORS)
                if(_takenRooms.Any(r => r.RoomPosition == currentRoom &&
                        r.RoomType == RoomType.Prize && adjacentRoom != puzzleRoomPosition))
                {
                    // Remove the door
                    isAdjacent = false;
                }
                
                if (!isAdjacent)
                {
                    // Add the door for removal
                    nonAdjacentRoomDirections.Add(direction);
                }
            }

            // Return all the doors we want removed
            return nonAdjacentRoomDirections;
        }
        
        /// <summary>
        /// The manhattan distance formula
        /// Used because we do not want the Euclidian distance that Unity provides with Vector2.Distance
        /// </summary>
        /// <returns>The sum of distances between the x and y coordinates</returns>
        private int CalculateDistance(Vector2 start, Vector2 end)
        {
            return Mathf.Abs((int)(start.x - end.x)) + Mathf.Abs((int)(start.y - end.y));
        }

        private void OnDrawGizmos()
        {
            // Don't draw the grid if we don't want it
            if (_drawGrid == false) return;

            Gizmos.color = _gridColor;

            // Get the room width and height from our crawler controller to show the grid locations
            float width = DungeonCrawlerController.ROOMSIZE.x;
            float height = DungeonCrawlerController.ROOMSIZE.y;

            // Display the grid with the starting location (0, 0) being the center
            for (int x = -_gridX; x <= _gridX; x++)
            {
                for (int y = -_gridY; y <= _gridY; y++)
                {
                    Gizmos.DrawWireCube(new Vector3(x * width, y * height), new Vector3(width, height));
                }
            }
        }

    }
}