using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
namespace MyLib
{
    /// <summary>
    /// Stream Load Room Data PerFrame A Piece
    /// </summary>
    public class RoomData : MonoBehaviour
    {
        public string partName;
        [ButtonCallFunc()]public bool RemovePart;
        public void RemovePartMethod() {
            foreach(var p in Prefabs) {
                if(p.prefab.name == partName) {
                    Prefabs.Remove(p);
                    Debug.LogError("FindPrefab: "+p.prefab.name);
                    break;
                }
            }
        }

        [System.Serializable]
        public class RoomPosRot{
            public GameObject prefab;
            public Vector3 pos;
            public Quaternion rot;
            public Vector3 scale = Vector3.one;
        }
        public List<RoomPosRot> Prefabs = new List<RoomPosRot>();

    }
}
