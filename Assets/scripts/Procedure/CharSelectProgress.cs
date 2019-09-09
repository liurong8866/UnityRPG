using UnityEngine;
using System.Collections;



namespace MyLib {
	public class CharSelectProgress : MonoBehaviour {
		public static CharSelectProgress charSelectLogic;

		void Awake() {
			charSelectLogic = this;
		}

		void ShowCharacterPanel() {
			WindowMng.windowMng.PushView ("UI/CharSelect2", false);
			MyEventSystem.myEventSystem.PushEvent (MyEvent.EventType.UpdateSelectChar);
		}

		void Start () {
			//Clear UI
			MyEventSystem.myEventSystem.PushEvent (MyEvent.EventType.EnterScene);
			CreateUI ();

			ShowCharacterPanel ();
		}

		void CreateUI() {
			UIPanel p = NGUITools.CreateUI (false, (int)GameLayer.UICamera);
			p.tag = "UIRoot";
			var root = p.GetComponent<UIRoot> ();
			root.scalingStyle = UIRoot.Scaling.ConstrainedOnMobiles;
			root.manualWidth = 1024;
			root.manualHeight = 768;
			root.fitWidth = true;
			root.fitHeight = true;
		}

		IEnumerator CreateCoroutine(string name, int job) {
		
			Log.Net ("LoginInit::Create Char is  "+name+" "+job);
			var packet = new KBEngine.PacketHolder ();
			CGCreateCharacter.Builder charCreate = CGCreateCharacter.CreateBuilder ();
			charCreate.Username = SaveGame.saveGame.GetDefaultUserName ();
			charCreate.Password = SaveGame.saveGame.GetDefaultPassword ();
			charCreate.PlayerName = name;

			charCreate.Job = (Job)(job) ;

			yield return StartCoroutine (KBEngine.Bundle.sendSimple (this, charCreate, packet));
			if (packet.packet.responseFlag == 0) {
				GCCreateCharacter createRes = packet.packet.protoBody as GCCreateCharacter;
				Log.Net("LoginInit::CreateCoroutine: create success");

				if(SaveGame.saveGame.charInfo == null) {
					GCLoginAccount.Builder cinfo1 = GCLoginAccount.CreateBuilder();
					SaveGame.saveGame.charInfo = cinfo1.BuildPartial();
				}
				GCLoginAccount.Builder cinfo = GCLoginAccount.CreateBuilder();

				foreach(RolesInfo ri in createRes.RolesInfosList) {
					cinfo.RolesInfosList.Add(ri);
				}
				
				SaveGame.saveGame.charInfo = cinfo.BuildPartial();
				Log.Net("CharInfo set");

				Log.Net("LoginInit::CreateCoroutine : "+SaveGame.saveGame.charInfo.RolesInfosCount);

				MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.CreateSuccess);
			} else {

			}

		}

		public void CreateChar (string name, int job)
		{
			StartCoroutine (CreateCoroutine (name, job));
		}

		GCListBranchinges branches;
		
		IEnumerator ListBranchinges() {
			KBEngine.PacketHolder packet = new KBEngine.PacketHolder ();
			CGListBranchinges.Builder branch = CGListBranchinges.CreateBuilder ();
			yield return StartCoroutine (KBEngine.Bundle.sendSimple(this, branch, packet));
			if (packet.packet.responseFlag == 0) {
				branches = packet.packet.protoBody as GCListBranchinges;
				//bran.ToString();
				Debug.Log("LoginInit:: bran is "+branches);
				
				Debug.Log("LoginInit::Branch Number "+branches+" list ");
				
				foreach(Branching b in branches.BranchingList) {
					Debug.Log("Branch "+b);
				}
				
			} else {
			}
		}

		///<summary>
		/// 进入主城 或者新手村
		/// </summary>
		IEnumerator BindSession() {
			var packet = new KBEngine.PacketHolder ();
			CGBindingSession.Builder bindSession = CGBindingSession.CreateBuilder ();
			bindSession.TokenId = SaveGame.saveGame.loginCharData.TokenId;

			yield return StartCoroutine (KBEngine.Bundle.sendSimple (this, bindSession, packet));
			if (packet.packet.responseFlag == 0) {
				var bindRes = packet.packet.protoBody as GCBindingSession;
				SaveGame.saveGame.bindSession = bindRes;	
				Log.Important("返回的初始化位置 "+bindSession.ToString());
				
				//1 Main
				//2 New User One Piece Of Scene With only Level1
                //
                var getValue = CGGetKeyValue.CreateBuilder();
                getValue.Key = GameBool.FINISH_NEW;
                var packet2 = new KBEngine.PacketHolder();
                yield return StartCoroutine(KBEngine.Bundle.sendSimple(this, getValue, packet2));
                var p = packet2.packet.protoBody as GCGetKeyValue;
                if(p.Value == ""){
                    //WorldManager.worldManager.WorldChangeScene(2, false);
                    Application.LoadLevel("storyIntro");
                }else {
                    WorldManager.worldManager.WorldChangeScene(2, false);
                }

			} else {
			}
		}

		public IEnumerator StartGameCoroutine(RolesInfo role) {
		
			yield return StartCoroutine (ListBranchinges());

			KBEngine.PacketHolder packet = new KBEngine.PacketHolder ();
			CGSelectCharacter.Builder selChar = CGSelectCharacter.CreateBuilder ();
			selChar.Username = SaveGame.saveGame.GetDefaultUserName ();
			selChar.Password = SaveGame.saveGame.GetDefaultPassword ();
			selChar.PlayerId = role.PlayerId;

			Debug.Log ("LoginInit:: StartGameCoroutine BranchNumb "+branches.BranchingCount);

			selChar.Branching = branches.GetBranching (0).Line;
			yield return StartCoroutine (KBEngine.Bundle.sendSimple(this, selChar, packet));

			if (packet.packet.responseFlag == 0) {
				var selCharRes = packet.packet.protoBody as GCSelectCharacter;
				SaveGame.saveGame.loginCharData = selCharRes;

                SaveGame.saveGame.SetSelectChar(role);

				StartCoroutine(BindSession());
			} else {
			}

		}
		public void StartGame (RolesInfo roleInfo)
		{
			StartCoroutine (StartGameCoroutine(roleInfo));
		}
	}
}
