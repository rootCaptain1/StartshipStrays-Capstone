using UnityEngine;

namespace PersistenceSystem
{
    [System.Serializable]
    public class GameData
    {
        public int money;

        public GameData()
        {
            money = 0;
        }
    }
}