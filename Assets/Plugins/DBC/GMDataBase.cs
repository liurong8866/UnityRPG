/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


using SimpleJSON;
namespace MyLib
{
	public class CDataBase {
		SimpleJSON.JSONArray database = null;
		List<IConfig> configData;

		public CDataBase(List<IConfig> conf) {
			configData = conf;
		}

		public CDataBase(SimpleJSON.JSONArray arr) {
			database = arr;
		}


		public IConfig SearchIdByConf(int id) {

			foreach(IConfig p in configData) {
				if(p.id == id) {
					return p;
				}
			}
			return null;
		}


		public string SearchForKey(string key) {
			//Log.Important ("SearchForKey " + key);
			foreach (JSONNode n in database) {
				var obj = n as JSONClass;
				//Log.Important("Char Diction "+obj["key"].Value);

				if(obj["key"].Value == key) {
					return obj["value"];
				}
			}
			return null;
		}
	}

	public class GMDataBaseSystem
	{
		public enum DBName{
			PropsConfig,
			StrDictionary,
			EquipConfig,
			ChapterConfig,
			MonsterFightConfig,
		}

		public static GMDataBaseSystem database = new GMDataBaseSystem();
		Dictionary<DBName, CDataBase> databaseMap;

		List<string> dbName = new List<string>() {
			"",
			"StrDictionary",
			"",
		};

        public T SearchId<T>(List<T> data, int id) where T : IConfig {
            return GMDataBaseSystem.SearchIdStatic<T>(data, id);
        }

		public static T SearchIdStatic<T>(List<T> data, int id) where T : IConfig {
			var d = data.Cast<IConfig> ();
			foreach (IConfig conf in d) {
				if(conf.id == id) {
					return conf as T;
				}
			}
			return null;
		}

		//从配置代码中获取数据
		public IConfig SearchIdByConf(DBName db, int id) {
			CDataBase database = null;
			if (databaseMap.TryGetValue (db, out database)) {
				database = databaseMap [db];
			} else {
				var ch = new List<IConfig>();
				if(db == DBName.PropsConfig) {
					foreach(PropsConfigData p in GameData.PropsConfig) {
						ch.Add(p);
					}
				}else if(db == DBName.EquipConfig) {
					foreach(EquipConfigData p in GameData.EquipConfig) {
						ch.Add(p);
					}
				}else if(db == DBName.ChapterConfig) {
					foreach(ChapterConfigData p in GameData.ChapterConfig) {
						ch.Add(p);
					}
				}
				database = new CDataBase(ch);
			}
		
			return database.SearchIdByConf(id);
		}


		GMDataBaseSystem() {
			databaseMap = new Dictionary<DBName, CDataBase> ();
		}

		public CDataBase GetDataBase(List<IConfig> conf) {
			CDataBase database = new CDataBase(conf);
			return database;
		}


		public CDataBase GetJsonDatabase(DBName db) {
			CDataBase database;
			if (databaseMap.TryGetValue (db, out database)) {
				
			} else {
				var binData = Resources.Load(dbName[(int)db]) as TextAsset;
				//Log.Important("binData "+binData);
				var arr = SimpleJSON.JSON.Parse(binData.text) as SimpleJSON.JSONArray;
				//Log.Important("Load database "+arr.ToString());
				database = new CDataBase(arr);
				databaseMap[db] = database;
			}
			return database;
		}

	}

}
