using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration
{
    public class DungeonCrawler
    {
        public Vector2 Position { get; set; } // Store the current position of the crawler

        public DungeonCrawler(Vector2 startPos)
        {
            Position = startPos;
        }

        public Vector2 Move(Dictionary<Direction, Vector2> directionMovementMap)
        {
            // Get a random direction
            // Move the amount stored in the direction map
            // Return the position
            Direction toMove = (Direction)Random.Range(0, directionMovementMap.Count);
            Position += directionMovementMap[toMove];
            return Position;
        }
    }
}