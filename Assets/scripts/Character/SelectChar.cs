
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
using MyLib;
using System.Collections.Generic;

namespace MyLib
{
	[RequireComponent (typeof(ShadowComponent))]
	[RequireComponent (typeof(NpcEquipment))]
	[RequireComponent (typeof(MyAnimationEvent))]
	[RequireComponent (typeof(KBEngine.KBNetworkView))]
	public class SelectChar : MonoBehaviour
	{
		public string newName;
		public long playerId;
		public int level;
		public uint job;
		public RolesInfo roleInfo;

		void Awake ()
		{
			var body = GetComponent<Rigidbody> ();
			if (body != null) {
				body.useGravity = false;
				body.freezeRotation = true;
				body.isKinematic = true;
			}
		}

		void Start ()
		{
			if (GetComponent<Animation>().GetClip ("stand") != null) {
				GetComponent<Animation>() ["stand"].wrapMode = WrapMode.Loop;
				GetComponent<Animation>().CrossFade ("stand");
			}

		}

		void OnEnable ()
		{
			if (GetComponent<Animation>().GetClip ("stand") != null) {
				GetComponent<Animation>() ["stand"].wrapMode = WrapMode.Loop;
				GetComponent<Animation>().CrossFade ("stand");
			}
			GetComponent<ShadowComponent> ().HideShadow ();
		}
		// Update is called once per frame
		void Update ()
		{
			//Debug.Log (animation.isPlaying);
		}

		static Dictionary<long, GameObject> fakeObj = new Dictionary<long, GameObject> ();

		//ui界面构建玩家模型
		public static GameObject ConstructChar (Job job)
		{

			var udata = Util.GetUnitData (true, (int)job, 0);
			GameObject player = null;
			player = Instantiate (Resources.Load<GameObject> (udata.ModelName)) as GameObject;
			NGUITools.AddMissingComponent<NpcAttribute> (player);
			var selChar = NGUITools.AddMissingComponent<SelectChar> (player);

			selChar.GetComponent<NpcAttribute> ().SetObjUnitData (udata);
			selChar.GetComponent<NpcEquipment> ().InitDefaultEquip ();
			return player;
		}
		/*
	 * 4 Job Start From 0
	 */
		public static GameObject ConstructChar (RolesInfo roleInfo)
		{
			Log.Sys ("SelectChar::ConstructChar " + roleInfo);
			GameObject player = null;
			if (fakeObj.TryGetValue (roleInfo.PlayerId, out player)) {
				return player;
			}

			var udata = Util.GetUnitData (true, (int)roleInfo.Job, 0);
			Log.Sys ("udata " + udata + " " + udata.name + " " + udata.ModelName);
			player = Instantiate (Resources.Load<GameObject> (udata.ModelName)) as GameObject;
			NGUITools.AddMissingComponent<NpcAttribute> (player);
			var selChar = NGUITools.AddMissingComponent<SelectChar> (player);
			player.GetComponent<NpcAttribute> ().SetObjUnitData (udata);
			player.GetComponent<NpcEquipment> ().InitDefaultEquip ();


			selChar.name = roleInfo.Name;
			selChar.playerId = roleInfo.PlayerId;
			selChar.level = roleInfo.Level;
			selChar.roleInfo = roleInfo;

			return player;
		}
	}
}
