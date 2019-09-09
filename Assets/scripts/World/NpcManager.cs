using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// 注册某张地图上所有的Npc
    ///  name --> 1Npc
    /// </summary>
    public class NpcManager : MonoBehaviour
    {
        public static NpcManager Instance;
        void Awake() {
            Instance = this;
        }
        public void TalkTo(GameObject npc) {
            Log.GUI("TalkToWho "+npc);
            npc.GetComponent<QuestNpcAI>().OnInterActive();
        }
        public QuestNpcAI GetNpc(string name) {
            return map[name].GetComponent<QuestNpcAI>();
        }

        public GameObject GetNpcObj(string name){
            return map[name];
        }

        Dictionary<string, GameObject> map = new Dictionary<string, GameObject>();
        public void RegNpc(string name, GameObject npc) {
            map[name] = npc;
        }
    }
}
