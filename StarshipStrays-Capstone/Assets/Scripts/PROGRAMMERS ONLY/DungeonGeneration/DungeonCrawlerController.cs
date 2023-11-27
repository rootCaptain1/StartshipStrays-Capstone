using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGeneration
{
    // Used to identify the direction in which the crawlers will be moving
    // Also used to identify the camera transition direction
    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    public class DungeonCrawlerController
    {
        // Store the amount of grid positions going out from the starting position
        private Vector2 _grid;

        // Define the room size based on the room dimensions
        public static readonly Vector2 ROOMSIZE = new Vector2(20f, 13f);

        // Stores a unique set of locations that will be found by the crawlers
        private HashSet<Vector2> _positionsVisited = new HashSet<Vector2>();
                
        // Stores the direction associated with the amount the crawlers should move per step
        // The rooms are this size in each direction appropriately as discovered by the gizmos attached to the room
        // Used for crawling, as well as camera transition positioning
        public static readonly Dictionary<Direction, Vector2> directionMovementMap = new Dictionary<Direction, Vector2>()
        {
            // Associate the direction with the size of the rooms
            // Will be used to place the rooms in the right location
            {Direction.Up, new Vector2(0, ROOMSIZE.y) },        // The top extent of the room
            {Direction.Left, new Vector2(-ROOMSIZE.x, 0) },     // The left extent of the room
            {Direction.Down, new Vector2(0, -ROOMSIZE.y) },     // The bottom extent of the room
            {Direction.Right, new Vector2(ROOMSIZE.x, 0) }      // The right extent of the room
        };

        // Constructor to initialize the grid
        public DungeonCrawlerController(float gridX, float gridY)
        {
            this._grid.x = ROOMSIZE.x * gridX;
            this._grid.y = ROOMSIZE.y * gridY;
        }

        /// <summary>
        /// Make sure the grid stays within the boundary of the grid
        /// </summary>
        public Vector2 ClampToGrid(Vector2 position)
        {
            float x = Mathf.Clamp(position.x, -_grid.x, _grid.x);
            float y = Mathf.Clamp(position.y, -_grid.y, _grid.y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Obtains and sets a list of positions will be found by the crawlers
        /// </summary>
        public List<Vector2> GetDungeonRoomLocations(DungeonGenerationData dungeonData)
        {
            // List to hold our crawlers' positions
            List<DungeonCrawler> dungeonCrawlers = new List<DungeonCrawler>();

            // Start all of the crawlers at (0, 0)
            for (int i = 0; i < dungeonData.numberOfCrawlers; i++)
            {
                dungeonCrawlers.Add(new DungeonCrawler(Vector2.zero));
            }

            // Set the iterations that the crawlers will move by getting a random number between the min and max
            int iterations = Random.Range(dungeonData.minIterations, dungeonData.MaxIterations);
            for (int i = 0; i < iterations; i++)
            {
                // For how many crawlers are set in the DungeonGenerationData,
                // move with each and store their position in our positionsVisited
                foreach (DungeonCrawler dungeonCrawler in dungeonCrawlers)
                {
                    Vector2 newPos = dungeonCrawler.Move(directionMovementMap);
                    newPos = ClampToGrid(newPos);
                    _positionsVisited.Add(newPos);
                }
            }

            // Return the entire list of unique locations visited by the cralers
            return _positionsVisited.ToList();
        }

        /// <summary>
        /// Remove a door at the given location and return the list back to the user
        /// </summary>
        public List<Vector2> RemoveRoom(Vector2 location)
        {
            _positionsVisited.Remove(location);
            return _positionsVisited.ToList();
        }
    }
}