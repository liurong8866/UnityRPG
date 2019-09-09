
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
	//保存一些和UI显示相关的对象状态
	public class FakeObject {
		public string name; //
		public int localId; //本地对象的ID
		public bool visible;
		public string windowName; //窗口名称
		public string cameraName; //窗口摄像机名称
		
	}

	public class FakeObjSystem 
	{
		//等待游戏启动正常之后再去初始化FakeObject对象	
		static FakeObjSystem _fakeObj = null;
		public static FakeObjSystem fakeObjSystem {
			get {
				if(_fakeObj == null) {
					_fakeObj = new FakeObjSystem();
				}
				return _fakeObj;
			}
		}

		GameObject playerWindow = null;
		GameObject dialogPlayer = null;
		GameObject rotPlayer = null;
		FakeObjSystem() {
			Log.Sys ("Init Fake Object Here");
			playerWindow = GameObject.Instantiate(Resources.Load<GameObject>("UI/PlayerWindow")) as GameObject;
			//GameObject.DontDestroyOnLoad (playerWindow);

			playerWindow.SetActive (false);
			dialogPlayer = Util.FindChildRecursive (playerWindow.transform, "dialogPlayer").gameObject;

			dialogPlayer.SetActive (false);
			dialogPlayer = null;

			rotPlayer = Util.FindChildRecursive (playerWindow.transform, "rotPlayer").gameObject;
		}



		//何时删除FakeObject对象
	


		//UI显示FakeObject 
		public void OnUIShown(int localId, RolesInfo roleInfo, int job = -1) {
			Log.GUI ("UI Show Local ID"+localId);
			if (dialogPlayer != null) {
				dialogPlayer.SetActive(false);
				dialogPlayer = null;
			}

			//选择人物界面显示的人物模型
			if (roleInfo != null) {
				dialogPlayer = SelectChar.ConstructChar (roleInfo);
				dialogPlayer.SetActive (true);
				dialogPlayer.transform.parent = rotPlayer.transform;
				dialogPlayer.transform.localPosition = Vector3.zero;
				dialogPlayer.transform.localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				dialogPlayer.transform.localScale = Vector3.one;
				Util.SetLayer (dialogPlayer, GameLayer.PlayerCamera);

				playerWindow.SetActive (true);
				//创建人物界面显示的模型
			} else if (job != -1) {
				dialogPlayer = SelectChar.ConstructChar ((Job)job);
				dialogPlayer.SetActive (true);
				dialogPlayer.transform.parent = rotPlayer.transform;
				dialogPlayer.transform.localPosition = Vector3.zero;
				dialogPlayer.transform.localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				dialogPlayer.transform.localScale = Vector3.one;
				Util.SetLayer (dialogPlayer, GameLayer.PlayerCamera);

				playerWindow.SetActive (true);
				//背包UI界面显示的人物模型
			}else {
				Log.GUI ("UI Show Object " + localId);
				dialogPlayer = ObjectManager.objectManager.NewFakeObject (localId);
				dialogPlayer.SetActive (true);
				dialogPlayer.transform.parent = rotPlayer.transform;
				dialogPlayer.transform.localPosition = Vector3.zero;
				dialogPlayer.transform.localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				dialogPlayer.transform.localScale = Vector3.one;
				playerWindow.SetActive (true);
			}
		}

		//UI隐藏
		//关闭背包界面同时关闭相关的MeshView 对象
		public void OnUIHide(int localId) {
			Log.GUI ("UI Hide fake object");
			if (dialogPlayer != null) {
				dialogPlayer.SetActive (false);
				dialogPlayer = null;
			}
			playerWindow.SetActive (false);
		}


		public void SetRotate(float degree) {
		}

	}
}
