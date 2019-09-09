using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
namespace MyLib
{
	public class GameInterface_Login
	{
		public static GameInterface_Login loginInterface = new GameInterface_Login();
		public JSONArray GetServerList() {
			return SaveGame.saveGame.serverList;
		}
		public int GetSaveAccountNum() {
			return SaveGame.saveGame.otherAccounts.Count;
		}

		public void FastRegister(){
			LoginInit.GetLogin().StartCoroutine (LoginInit.GetLogin().regAndLog());
		}

		//TODO:登陆失败则删除掉用户名 密码
		public void LoginWithUserNamePass(string name, string pass) {
			SaveGame.saveGame.AddNewAccount (name, pass);
			LoginInit.GetLogin().StartCoroutine (LoginInit.GetLogin().loginCoroutine ());
		}
		public void LoginGame ()
		{
			//LoginInit.GetLogin().StartCoroutine (LoginInit.GetLogin().loginCoroutine ());
            LoginInit.GetLogin().TryToLogin();
		}

		public void SelectAccounAndLogin (int currentSelect)
		{
			throw new NotImplementedException ();
		}

		public void RegisterAccount (string name, string pass)
		{
			LoginInit.GetLogin().startRegister (name, pass);
		}



		public GCLoginAccount GetCharInfo() {
			//return LoginInit.loginInit.GetCharInfo ();
			return SaveGame.saveGame.charInfo;
		}


		public void StartGame (RolesInfo roleInfo)
		{
			//LoginInit.loginInit.StartCoroutine(LoginInit.loginInit.StartGameCoroutine (roleInfo));
			CharSelectProgress.charSelectLogic.StartGame (roleInfo);
		}

	}

}
