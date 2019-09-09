
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{


	/*
	 * mailbox receive other send message
	 * message dispatch   registerListener notify listener  push
	 * other checkMessage     pull
	 * 类似于Mailbox的组件 用于缓存其它实体发送给该实体的消息
	 * 包括：动画系统发送的消息  受到攻击的消息
	 */
	/// <summary>
	/// 攻击事件和被攻击事件等动画事件和外部事件触发
	/// AI或者状态机检测事件接着处理
	/// AI也向状态机中注入事件
	/// </summary>
    public class MyAnimationEvent : KBEngine.KBNetworkView
	{
		//TODO:在发射MyAnimationEvent给Player或者怪物之前 同服务器通信 GameInterface里实现网络
		public enum MsgType {
			KillNpc,
			DoSkill,
            IDLE,
            STUNNED,
            EXIT_STUNNED,
            BOMB,
            DEAD,
            JUMP,
		}
		/*
		 * Pass Message Format
		 */
		public class Message {
			public MsgType type;

			public SkillData skillData;
			public Message(MsgType t) {
				type = t;
			}
			public Message() {

			}
		}

		//召唤生物生命时间到了
		[HideInInspector]
		public bool timeToDead = false;

		[HideInInspector]
		public bool hit = false;
		[HideInInspector]
		public bool onHit = false;
		[HideInInspector]
		public GameObject attacker;
		NpcAttribute attribute;


		[HideInInspector]
		public bool KnockBack = false;
		//public GameObject WhoKnock;
        public Vector3 KnockWhoPos;

		[HideInInspector]
		List<Message> messages = new List<Message>();


		public void InsertMsg(Message msg) {
            Log.Sys("AddMessage "+msg.type);
			messages.Add (msg);
		}

        public Message NextMsg() {
            if(messages.Count > 0) {
                var ret = messages[0];
                messages.RemoveAt(0);
                return ret;
            }
            return null;
        }
        public void ClearMsg() {
            messages.Clear();
        }

		public Message CheckMsg(MsgType type) {
			Message ret = null;
			if (messages.Count > 0 && messages [0].type == type) {
				ret = messages[0];
				messages.RemoveAt(0);
			}

			return ret;
		}

        /// <summary>
        /// 角色使用某个技能 
        /// </summary>
        /// <param name="skillData">Skill data.</param>
		public void OnSkill (SkillData skillData)
		{
            var buff = GetComponent<BuffComponent>();
            if(!buff.CanUseSkill()) {
                return;
            }

			var msg = new Message (MsgType.DoSkill);
			msg.skillData = skillData;
			InsertMsg (msg);
		}

		public class DamageData {
			public GameObject Attacker;
			public int Damage;
			public SkillData.DamageType damageType;
			public bool ShowHit;
			public bool isCritical;
			public DamageData(GameObject a, int d, bool critical, SkillData.DamageType dt, bool s) {
				Attacker = a;
				Damage = d;
				damageType = dt;
				ShowHit = s;
				isCritical = critical;
			}
		}
		public List<DamageData> FetchDamage() {
			return CacheDamage;
		}
		public void ClearDamage() {
			onHit = false;
			CacheDamage.Clear ();
		}
		List<DamageData> CacheDamage;
		void Awake() {
			regEvt = new List<MyEvent.EventType> ();
		}
		void Start() {
			attribute = GetComponent<NpcAttribute>();
			CacheDamage = new List<DamageData>();
		}
		void HIT ()
		{
			hit = true;	
			MyEventSystem.myEventSystem.PushLocalEvent(photonView.GetLocalId(), MyEvent.EventType.EventTrigger);
		}


		Vector3 particlePos;
		void SetParticlePos(string pos) {
			var p = SimpleJSON.JSON.Parse(pos) as SimpleJSON.JSONArray;
			particlePos = new Vector3 (p[0].AsFloat, p[1].AsFloat, p[2].AsFloat);
		}
        void PlaySound(string sound) {
            BackgroundSound.Instance.PlayEffect(sound);
        }
		void SpawnParticle(string particle) {
			Log.Ani ("animation spawn particle "+particle);
			var evt = new MyEvent (MyEvent.EventType.SpawnParticle);
			evt.player = gameObject;
			evt.particleOffset = particlePos;
			evt.particle = particle;
			evt.boneName = attachParticleBone;
			//MyEventSystem.myEventSystem.PushLocalEvent (photonView.GetLocalId(), evt);
			MyEventSystem.myEventSystem.PushEvent (evt);

			Reset ();
		}
		void Reset() {
			particlePos = Vector3.zero;
			attachParticleBone = "";
		}
		string attachParticleBone = "";
		void AttachParticleToBone(string boneName) {
			attachParticleBone = boneName;
		}

        public int lastAttacker;
		/*
		 * When Birth is inattackable
         * Birth Add Buff UnAttackable 
		 */ 
		public void OnHit(GameObject go, int damage, bool isCritical, SkillData.DamageType damageType = SkillData.DamageType.Physic,  bool showHit = true) {
			if(attribute._characterState != CharacterState.Birth) {
                lastAttacker = go.GetComponent<NpcAttribute>().GetNetView().GetServerID();

				CacheDamage.Add(new DamageData(go, damage, isCritical, damageType, showHit));
				onHit = true;
				attacker = go;
                var evt = new MyEvent(MyEvent.EventType.OnHit);
                evt.attacker = attacker;
                MyEventSystem.myEventSystem.PushLocalEvent(photonView.GetLocalId(), evt);
			}
		}

		void ShowWeaponTrail(float duration) {
			Log.Ani ("Show Weapon Trail");
			//ShowTrail = true;
			var evt = new MyEvent (MyEvent.EventType.ShowWeaponTrail);
			evt.floatArg = duration;
			MyEventSystem.myEventSystem.PushLocalEvent (photonView.GetLocalId(), evt);
		}

		void HideWeaponTrail() {
			Log.Ani("Hide Weapon Trail");
			//HideTrail = true;
			MyEventSystem.myEventSystem.PushLocalEvent (photonView.GetLocalId (), MyEvent.EventType.HideWeaponTrail);
		}

        public void KnockBackWho(Vector3 pos) {
			Log.AI ("KnockBack Who "+pos);
			KnockBack = true;
            KnockWhoPos = pos;
		}

        public void EnterJump() {
            Log.AI("EnterJump");
            var msg = new Message();
            msg.type = MsgType.JUMP;
            InsertMsg (msg);
        }

		public bool fleeEvent = false;
		public float fleeTime = 0;

		protected override void OnEvent (MyEvent evt)
		{
			if (evt.type == MyEvent.EventType.MonsterDead) {
				fleeEvent = true;
				fleeTime = Time.time;
			} 
		}

	}

}