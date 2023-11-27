using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration
{
    [CreateAssetMenu(menuName = "DungeonGeneration/DungeonGenerationData")]
    public class DungeonGenerationData : ScriptableObject
    {
        public List<GameObject> roomPrefabs = new List<GameObject>();   // Rooms wanted in dungeon
        public int numberOfCrawlers;                                    // Amount of "Crawlers" to roam and find positions
        [Range(5, 100)] public int minIterations;                       // The minimum amount of iterations that the crawlers will walk
        [Range(5, 100)] [SerializeField] private int _maxIterations;    // The maximum amount of iterations that the crawlers will walk

        public int MaxIterations { 
            get 
            { 
                if(_maxIterations >= minIterations) // Making sure the max is at least as much as the minimum
                    return _maxIterations; 
                return minIterations;
            } 
        }
    }
}