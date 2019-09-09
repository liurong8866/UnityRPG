using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 任务Npc 处理玩家交互事件 
    /// </summary>
    public class QuestNpcAI : AIBase 
    {
        /// <summary>
        ///地图初始化的时候配置 
        /// </summary>
        public EmptyDelegate  TalkToMe = null; 
        public void OnInterActive(){
            Log.AI("TalkToMe "+TalkToMe);
            if(TalkToMe != null) {
                TalkToMe();
            }
        }
        void Awake() {
            attribute = GetComponent<NpcAttribute> ();
            ai = new QuestNpcCharacter ();
            ai.attribute = attribute;
            ai.AddState (new QuestIdle());
        }

        void Start ()
        {
            ai.ChangeState (AIStateEnum.IDLE);
        }
    }
}