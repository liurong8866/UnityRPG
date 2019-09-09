using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class SentryIdle : IdleState
	{
		bool birthYet = false;

		public override void EnterState ()
		{
			base.EnterState ();
			SetAttrState (CharacterState.Idle);
			aiCharacter.SetIdle ();
			Log.AI ("AttachToMaster "+GetAttr().ObjUnitData.AttachToMaster);
			if (GetAttr ().ObjUnitData.AttachToMaster) {
				GetAttr ().GetComponent<CharacterController> ().enabled = false;
				GetAttr ().GetComponent<PhysicComponent> ().enabled = false;
				Log.AI("Attach To Master ");

				GetAttr ().StartCoroutine (TraceMaster());
			}
		}

		IEnumerator Birth ()
		{
			if (CheckAni ("spawn")) {
				SetAttrState (CharacterState.Birth);
				
				PlayAni ("spawn", 2, WrapMode.Once);
				Log.AI ("spawn particle is " + GetAttr ().ObjUnitData.SpawnEffect);
				if (GetAttr ().ObjUnitData.SpawnEffect != "") {
					GameObject g = GameObject.Instantiate (Resources.Load<GameObject> (GetAttr ().ObjUnitData.SpawnEffect)) as GameObject;
					//g.transform.position = GetAttr().transform.position;
					g.transform.parent = GetAttr ().transform;
					g.transform.localPosition = Vector3.zero;
					g.transform.localRotation = Quaternion.identity;
					g.transform.localScale = Vector3.one;
				}
				yield return GetAttr ().StartCoroutine (Util.WaitForAnimation (GetAttr ().GetComponent<Animation>()));
				
				SetAttrState (CharacterState.Idle);
				aiCharacter.SetIdle ();
			}
			birthYet = true;
		}
		//TODO:后续做成全局Static函数  CommonAI 静态一次性函数
		GameObject NearestEnemy ()
		{
			return SkillLogic.FindNearestEnemy (GetAttr ().gameObject);
		}
		IEnumerator TraceMaster() {
			while (!quit && GetAttr().GetOwner() != null) {
				var transform = GetAttr ().transform;
				transform.position = Vector3.Lerp (transform.position, GetAttr ().GetOwner ().transform.position, Time.deltaTime * 3);
				transform.rotation = Quaternion.Lerp (transform.rotation, GetAttr ().GetOwner ().transform.rotation, Time.deltaTime * 3);	


				yield return null;
			}
			Log.AI ("quit Trace master ");
		}
		IEnumerator FindTarget ()
		{
			while (!quit) {
				if (CheckEvent ()) {
					yield break;
				}
				Log.AI("sentry idle find target ");
				var enemy = NearestEnemy();
				if (enemy != null)
				{
					aiCharacter.SetEnemy(enemy);
					var dir = enemy.transform.position-GetAttr().transform.position;
					var qua = Quaternion.FromToRotation(Vector3.forward, new Vector3(dir.x, 0, dir.z));
					Log.AI("sentry find target "+enemy);
					//根据攻击目标调整 攻击方向
					GetAttr().transform.localRotation = qua;
					GetSkill().SetDefaultActive();
					//发动技能攻击
					aiCharacter.ChangeState (AIStateEnum.CAST_SKILL);
					yield break;
				}


				yield return null;
			}
			Log.AI ("Idle state Logic quit ?");
		}

		public override IEnumerator RunLogic ()
		{
			if (!birthYet) {
				yield return GetAttr ().StartCoroutine (Birth ());
			}
			yield return GetAttr ().StartCoroutine (FindTarget ());
			
			Log.AI ("State Logic Over " + type);
		}
	}
}
