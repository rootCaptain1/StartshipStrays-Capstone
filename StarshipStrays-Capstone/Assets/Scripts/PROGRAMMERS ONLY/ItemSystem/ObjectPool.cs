using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class ObjectPool
    {
        private static Dictionary<GameObject, Pool> _pools;

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (_pools == null)
            {
                _pools = new Dictionary<GameObject, Pool>();
            }

            if (_pools.ContainsKey(prefab) == false)
            {
                _pools[prefab] = new Pool(prefab);
            }

            return _pools[prefab].Spawn(position, rotation);
        }

        public static void Despawn(GameObject objectToRemove)
        {
            PoolMember pm = objectToRemove.GetComponent<PoolMember>();
            if(pm == null)
            {
                GameObject.Destroy(objectToRemove);
            }
            else
            {
                pm.MyPool.Despawn(objectToRemove);
            }
        }

        public static void ClearPools()
        {
            if (_pools != null)
            {
                foreach (var poolPair in _pools)
                {
                    poolPair.Value.Clear();
                }

                _pools.Clear();
            }
        }

        private class Pool
        {
            private int _curIndex = 0;
            private Stack<GameObject> _inactiveObjects;
            private GameObject _prefab;

            public Pool (GameObject prefab)
            {
                _prefab = prefab;
                _inactiveObjects = new Stack<GameObject>();
            }

            public GameObject Spawn(Vector3 position, Quaternion rotation)
            {
                GameObject obj;

                if(_inactiveObjects.Count == 0)
                {
                    obj = GameObject.Instantiate(_prefab.gameObject, position, rotation);
                    obj.name = _prefab.name + "_" + _curIndex;
                    _curIndex++;
                    obj.AddComponent<PoolMember>().MyPool = this;
                }
                else
                {
                    obj = _inactiveObjects.Pop();

                    if(obj == null)
                    {
                        return Spawn(position, rotation);
                    }
                }

                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }

            public void Despawn(GameObject objectToRemove)
            {
                objectToRemove.SetActive(false);
                _inactiveObjects.Push(objectToRemove);
            }

            public void Clear()
            {
                _inactiveObjects.Clear();
            }
        }

        private class PoolMember : MonoBehaviour
        {
            public Pool MyPool { get; set; }
        }
    }
}