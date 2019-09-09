
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
using SimpleJSON;
using System.Reflection;
using System;
using System.Collections.Generic;


namespace MyLib {
	//Attribute use for what
	[AttributeUsage(AttributeTargets.Field)]
	public class SaveAttribute : System.Attribute {
		public SaveAttribute() {
		}
	}
	public class CharacterData : MonoBehaviour {


		public static CharacterData characterData;

		[SaveAttribute()]
		public int Level = 1;
		[SaveAttribute()]
		public int Exp = 0;

		//public int MaxHP = 300;
		//public int MaxMP = 20;

		[SaveAttribute()]
		public int Strength = 10;
		[SaveAttribute()]
		public int Dexterity = 4;
		[SaveAttribute()]
		public int Magic = 3;
		[SaveAttribute()]
		public int Defense = 10;
		[SaveAttribute()]
		public int AttributePoint = 0;
		[SaveAttribute()]
		public int SkillPoint = 0;
		[SaveAttribute()]
		public string newName;


		void Awake() {
			characterData = this;
			DontDestroyOnLoad (gameObject);
			/*
			 * LoadData From SaveGame public Static Object
			 */ 
		}

		// Use this for initialization
		void Start () {
			LoadData ();
		}
		/*
		 * Attribute Point is Update When Player's NpcAttribuet init 
		 */
		void InitView() {


		}
		void OnLevelWasLoaded() {
			InitView ();
		}

		// Update is called once per frame
		void Update () {
		
		}
		public void LoadData() {
			//var sg = GameObject.FindObjectOfType<SaveGame> ();
		}

		public void SaveGameNow(int slot) {
			//SaveGame sg = GameObject.FindObjectOfType<SaveGame> ();
			//sg.saveData
		}



		public JSONClass Serialize() {
			var js = new JSONClass ();
			var allFields = this.GetType ().GetMembers ();
			foreach(MemberInfo m in allFields) {
				Debug.Log(m);
				var att = m.GetCustomAttributes(false);
				for(int i = 0; i < att.Length; i++) {
					Debug.Log(att[i]);
					if(att[i].GetType() == typeof(SaveAttribute)) {
						Debug.Log("GetField "+this.GetType().GetField(m.Name)+" Declar ");
						var fie = this.GetType().GetField(m.Name);
						if(fie.FieldType == typeof(string)) {
							js[m.Name] = fie.GetValue(this) as string;
						}else if(fie.FieldType == typeof(int)) {
							js[m.Name].AsInt = (int)(fie.GetValue(this));
						} else if(fie.FieldType == typeof(float)) {
							js[m.Name].AsFloat = (float)(fie.GetValue(this));
						}

						break;
					}
				}
			}

			//js.name = name;
			return js;
		}

		public void DeSerialize(JSONClass jsData) {
			/*
			var allFields = this.GetType ().GetMembers ();
			foreach (MemberInfo m in allFields) {
				var att = m.GetCustomAttributes();
			}
			*/
			foreach (KeyValuePair<string, JSONNode> n in jsData) {
				var f = this.GetType().GetField(n.Key);
				if(f.FieldType == typeof(string)) {
					f.SetValue(this, n.Value.ToString());
				}else if(f.FieldType == typeof(int)) {
					f.SetValue(this, n.Value.AsInt);
				}else if(f.FieldType == typeof(float)) {
					f.SetValue(this, n.Value.AsFloat);
				}
			}
		}
	}

}