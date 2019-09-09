using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// 技能模板配置 
    /// </summary>
	public class SkillDataConfig : MonoBehaviour
	{
        /// <summary>
        ///技能层相关配置  
        /// </summary>
		[System.Serializable]
		public class EventItem {
            /// <summary>
            /// 事件类型： 技能开始，技能命中，子弹命中
            /// </summary>
			public MyEvent.EventType evt;
            /// <summary>
            /// 技能层配置对象 
            /// </summary>
			public GameObject layout;

		    /// <summary>
            /// 技能Buff
            /// </summary>
			public Affix affix;

			//DamageShape 会带着玩家一起移动  向前冲击技能需要带动玩家一起运动
			public bool attachOwner = false;
            //将粒子效果附加到玩家身上
            public bool attaches = false;

			//陷阱技能会衍生出子技能 创建另外一个技能的技能状态机
            public int childSkillId = -1;

			//电击爆炸出现在目标身上 粒子效果附着到目标身上
			public bool AttachToTarget = false;

			//设置Beam的粒子效果的GravityWell BeamTarget 的目标为Enemy的坐标  闪电类型的粒子瞬间攻击一定命中目标 目标头顶召唤闪电
			public bool SetBeamTarget = false;
			public Vector3 BeamOffset = new Vector3 (0, 1, 0);

            //出现在目标所在位置
            public bool TargetPos = false;
            //使用上次事件标记的位置 魔兽争霸3中的 血法师的 召唤火焰的技能 
            public bool UseMarkPos = false;

            public int EvtId = 0; //当前技能的事件ID编号

		}
        /// <summary>
        /// 所有技能层列表 
        /// </summary>
		public List<EventItem> eventList;


		//循环播放idle动画 按照攻击频率来 释放技能 火焰陷阱  某些技能循环播放动画 持续性的攻击技能 喷射火焰
        public bool animationLoop = false;
        //持续攻击时间长度
        public float attackDuration = 1;

        /// <summary>
        /// 初始哈每个Event事件的ID编号 
        /// 用于网络同步技能Buff
        /// </summary>
        void Awake() {
            var num = 0;
            foreach(var e in eventList) {
                e.EvtId = num++;
            }
        }

        /// <summary>
        /// 获得某个技能层 根据ID
        /// 用于网络同步
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="evtId">Evt identifier.</param>
        public EventItem GetEvent(int evtId) {
            if(evtId >= 0 && evtId < eventList.Count) {
                return eventList[evtId];
            }else {
                return null;
            }
        }
	}
}