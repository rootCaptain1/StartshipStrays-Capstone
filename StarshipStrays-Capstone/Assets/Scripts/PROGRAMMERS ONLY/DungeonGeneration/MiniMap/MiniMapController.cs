using DungeonGeneration;
using System.Collections.Generic;
using UnityEngine;

namespace MiniMap
{
    public class MiniMapController : MonoBehaviour
    {
        private Room _parentRoom;
        private List<GameObject> _miniMapObjects;

        private void Start()
        {
            _parentRoom = GetComponent<Room>();
            _miniMapObjects = new List<GameObject>();
            Transform[] foundObjects = GetComponentsInChildren<Transform>();

            foreach (Transform item in foundObjects)
            {
                if (item.gameObject.layer == LayerMask.NameToLayer("MiniMap"))
                {
                    _miniMapObjects.Add(item.gameObject);
                    SetGameObjectActive(item.gameObject, false); // Deactivate here
                }
            }

            if (_parentRoom != null)
            {
                _parentRoom.RoomEnteredEvent += ShowRoom;
            }
        }

        private void OnDisable()
        {
            if (_parentRoom != null)
            {
                _parentRoom.RoomEnteredEvent -= ShowRoom;
            }
        }

        private void ShowRoom()
        {
            foreach (GameObject obj in _miniMapObjects)
            {
                SetGameObjectActive(obj, true); // Activate here
            }
        }

        private void SetGameObjectActive(GameObject obj, bool active)
        {
            if (obj != null && obj.activeSelf != active)
            {
                obj.SetActive(active);
            }
        }
    }
}