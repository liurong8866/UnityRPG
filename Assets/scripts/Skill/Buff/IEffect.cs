using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib {
	public class IEffect {
		protected GameObject obj;
		public Affix affix;
		protected Affix.EffectType type;
		public bool IsDie = false;
		public GameObject attacker;
        public Vector3 attackerPos;

		protected float passTime = 0;
		GameObject unitTheme;
        protected NpcAttribute attri;

        /// <summary>
        /// 初始化Buff
        /// </summary>
        /// <param name="af">Af.</param>
        /// <param name="o">O.</param>
		public virtual void Init(Affix af, GameObject o) {
			affix = af;
			obj = o;
            attri = obj.GetComponent<NpcAttribute>();
			if (affix.UnitTheme != null) {
				var par = GameObject.Instantiate(affix.UnitTheme) as GameObject;
				//par.transform.parent = obj.transform;
                var sync = par.AddComponent<SyncPosWithTarget>();
                sync.target = obj;

				par.transform.localPosition = affix.ThemePos;
				par.transform.localRotation = Quaternion.identity;
				par.transform.localScale = Vector3.one;

				unitTheme = par;
			}
		}
        /// <summary>
        /// 激活Buff
        /// </summary>
		public virtual void OnActive() {
		}
        /// <summary>
        /// Buff状态更新 
        /// </summary>
		public virtual void OnUpdate() {
			passTime += Time.deltaTime;
			if (passTime >= affix.Duration) {
				IsDie = true;
			}
		}
		public virtual void OnDie(){
			if (unitTheme != null) {
				GameObject.Destroy(unitTheme);
			}
		}

        /// <summary>
        /// 速度修正系数
        /// </summary>
        /// <returns>The speed coff.</returns>
        public virtual float GetSpeedCoff() {
            return 1;
        }
        public virtual int GetCriticalRate() {
            return 0;
        }

		public virtual int GetArmor ()
		{
			return 0;
		}
		protected BuffComponent GetBuffCom() {
			return obj.GetComponent<BuffComponent> ();
		}
        public virtual bool CanUseSkill() {
            return true;
        }
	}
}
