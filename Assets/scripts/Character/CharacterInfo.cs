using UnityEngine;
using System.Collections;
using SimpleJSON;

/*
 * Initial CharacterData From Network
 * Usage:
 * 		Show Information On CharacterBoard
 */
using System.Collections.Generic;

 
namespace MyLib
{
	/// <summary>
	/// 角色或者怪物的属性
	/// 角色的属性通过网络来初始化
	///	怪物的属性 通过本地数据库加载来初始化
    /// 角色或者怪物的网络数据
	/// </summary>
	//TODO::怪物的属性 通过本地数据库加载来初始化
	public class CharacterInfo : KBEngine.MonoBehaviour
	{
		private Dictionary<CharAttribute.CharAttributeEnum, int> propertyValue = new Dictionary<CharAttribute.CharAttributeEnum, int> ();
		//Dictionary<CharAttribute.CharAttributeEnum, bool> propertyDirty = new Dictionary<CharAttribute.CharAttributeEnum, bool>();
		public bool initYet = false;
		NpcAttribute attribute;
	
		/*
		 * Load From Json File PropertyKey
		 * 
		 * No State Code
		 * 根据从服务器加载的角色数据来初始化NpcAttribute 中的数值（后续可能需要调整这种方式）
		 */ 

		IEnumerator InitProperty() {
			Log.Net ("characterinfo   init");
			NetDebug.netDebug.AddConsole("CharacterInfo:: Init Property "+gameObject.name);
			CGGetCharacterInfo.Builder getChar = CGGetCharacterInfo.CreateBuilder ();
			getChar.PlayerId = photonView.GetServerID();

            getChar.AddParamKey((int)CharAttribute.CharAttributeEnum.LEVEL);
            getChar.AddParamKey((int)CharAttribute.CharAttributeEnum.EXP);
            getChar.AddParamKey((int)CharAttribute.CharAttributeEnum.GOLD_COIN);
            getChar.AddParamKey((int)CharAttribute.CharAttributeEnum.JING_SHI);

			NetDebug.netDebug.AddConsole ("GetChar is "+getChar.ParamKeyCount);

			KBEngine.PacketHolder packet = new KBEngine.PacketHolder ();
			yield return StartCoroutine (KBEngine.Bundle.sendSimple (this, getChar, packet));

			if (packet.packet.responseFlag == 0) {
				var info = packet.packet.protoBody as GCGetCharacterInfo;
				Debug.Log("CharacterInfo::InitProperty "+info);
				foreach(RolesAttributes att in info.AttributesList) {

					if(att.AttrKey == (int)CharAttribute.CharAttributeEnum.GOLD_COIN) {
						Log.Net("attr int "+att.BasicData.Indicate);
						Log.Net("attr key "+att.BasicData.TheInt32);
						Log.Net("attr "+att.ToString());
					}
					if(att.BasicData.Indicate == 2) {
						propertyValue[(CharAttribute.CharAttributeEnum)att.AttrKey] =(int) att.BasicData.TheInt64;
					}else {
						propertyValue[(CharAttribute.CharAttributeEnum)att.AttrKey] = att.BasicData.TheInt32;
					}
				}
			} else {
				Debug.LogError("CharacterInfo::InitProperty Error");
			}

			var m = GetProp (CharAttribute.CharAttributeEnum.GOLD_COIN);
			Log.Net (m.ToString());

			Log.Important ("PropertyValue is "+propertyValue);

            attribute.ChangeLevel(GetProp(CharAttribute.CharAttributeEnum.LEVEL));


            attribute.ChangeHP (0);
            attribute.ChangeMP (0);
            attribute.ChangeExp (0);


			Log.Important ("Init HP "+attribute.HP);
			Log.Important ("Init Property Over");
			initYet = true;
			NetDebug.netDebug.AddConsole ("Init CharacterInfo Over");
			Log.Sys ("UpdatePlayerData "+attribute.ObjUnitData.Level);
			var evt = new MyEvent (MyEvent.EventType.UpdatePlayerData);
			evt.localID = attribute.GetLocalId ();
			MyEventSystem.myEventSystem.PushEvent (evt);
		}

		public int GetProp(CharAttribute.CharAttributeEnum key) {
			int v = 1;
			if (!propertyValue.TryGetValue (key, out v)) {
			}
			return v;
		}

		//TODO:Level Up
		public void SetProp(CharAttribute.CharAttributeEnum key, int val) {
			propertyValue [key] = val;
			
		}
	

		// Use this for initialization
		IEnumerator Start ()
		{
			while (KBEngine.KBEngineApp.app == null) {
				yield return null;
			}
			attribute = GetComponent<NpcAttribute> ();
            if(photonView.IsMe) {
				StartCoroutine(InitProperty());
			}
		}
	

	}

}