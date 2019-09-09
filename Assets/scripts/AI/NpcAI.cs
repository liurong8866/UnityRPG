
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
namespace MyLib {
    /// <summary>
    ///怪物AI 
    /// </summary>
    [RequireComponent(typeof(PhysicComponent))]
	public class NpcAI : AIBase {
		MyAnimationEvent myAnimationEvent;
		float heading;
		Vector3 targetRotation;
		void Awake() {

			attribute = GetComponent<NpcAttribute>();
			myAnimationEvent = GetComponent<MyAnimationEvent>();
			heading = Random.Range(0, 360);
			transform.eulerAngles = new Vector3(0, heading, 0);

			ai = new MonsterCharacter ();
			ai.attribute = attribute;
			ai.AddState (new MonsterIdle ());
			ai.AddState (new MonsterCombat ());
			ai.AddState (new MonsterDead ());
			ai.AddState (new MonsterFlee ());
			ai.AddState (new MonsterKnockBack ());
            ai.AddState(new MonsterSkill());
		}
		void Start() {
			ai.ChangeState (AIStateEnum.IDLE);
		}

		protected override void OnDestroy() {
            base.OnDestroy();
			if (attribute.IsDead) {
				Util.ClearMaterial (gameObject);
			}
		}

	}
}
