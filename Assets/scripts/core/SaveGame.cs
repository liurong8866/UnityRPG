
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
using System.IO;
using System.Collections.Generic;
using System;

namespace MyLib
{
    public class SaveGame : MonoBehaviour
    {   
        public GCLoginAccount charInfo;
        public static SaveGame saveGame;
        public JSONClass msgNameIdMap;
        public List<GameObject> characters;
        public bool InitYet = false;
        /*
        * ForMat:
        * char1 char2 char3
        * {
        *   "characters":[{charProperty1: {name:xxx},  backpack1:  skill1:},  {char2}, {char3}], 
        *   "lastSelect":0, 
        * 
        *   "accounts": [{username1:xxx, password1, level1}, {username2, password2, level2}, {username3, password3, level3}]
        * }
        */
        /*
         * CharProperty:  CharacterData 
         * backpack: BackpackDataController
         * skill: skillDataController
         */ 

        public JSONNode saveData;
        [StringProp()]
        public string
            saveDataStr;
        public List<JSONClass> otherAccounts;

        ///<summary>
        /// 选择登陆的玩家的初始位置和token等信息
        /// </summary>
        public GCSelectCharacter loginCharData;
        public GCBindingSession bindSession;

        /*
         * [
         * {"serverName" : "".
         * "ip":"",
         * "port":8000},
         * 
         * ]
         */ 
        public JSONArray serverList;

        /*
         * When LoginInit  StartGame  
         * set These two Value
         */ 

        public RolesInfo selectChar
        {
            get;
            private set;
        }

        /// <summary>
        /// 登陆时候设置选择的人物
        /// </summary>
        public void SetSelectChar(RolesInfo role)
        {
            selectChar = role;
        }

        /// <summary>
        /// 测试的时候设置人物数据
        /// </summary>
        public void TestSetRole()
        {
            var role = RolesInfo.CreateBuilder();
            role.Name = "战士";
            role.PlayerId = 103;
            role.Level = 1;
            role.Job = (Job)1;
            selectChar = role.Build();
        }

        bool DataInitFromNetworkYet = false;

        void Awake()
        {

            otherAccounts = new List<JSONClass>();
            saveGame = this;
            DontDestroyOnLoad(gameObject);

            LoadSaveFile();
            LoadMsg();
        }

        void Start()
        {
            StartCoroutine(SaveData());
        }

        IEnumerator SaveData()
        {
            while (true)
            {
                yield return new WaitForSeconds(60);
                if (ServerData.Instance != null)
                {
                    ServerData.Instance.SaveUserData();
                }
            }
        }


        //只从网络初始化一次数据
        public IEnumerator InitDataFromNetwork()
        {
            if (!DataInitFromNetworkYet)
            {
                NetDebug.netDebug.AddConsole("SaveGame:InitData");
                Log.Net("Save game Init");
                yield return StartCoroutine(BackPack.backpack.InitFromNetwork());
                yield return StartCoroutine(SkillDataController.skillDataController.InitFromNetwork());
                yield return StartCoroutine(CopyController.copyController.InitFromNetwork());

                DataInitFromNetworkYet = true;
                NetDebug.netDebug.AddConsole("SaveGame:: DataInitOver");
            }
        }

        /*
         * Load Protobuffer Message Name To ID map Json file
         */ 
        void LoadMsg()
        {
            TextAsset bindata = Resources.Load("nameMap") as TextAsset;
            Debug.Log("nameMap " + bindata.text);
            msgNameIdMap = JSON.Parse(bindata.text).AsObject;
            Debug.Log("msgList " + msgNameIdMap.ToString());
        }
        /*
         * {moduleName: {"id":xx,  "method":1, "method2":2}}
         */ 
        public string getModuleName(int moduleId)
        {
            Debug.Log("find Module Name is " + moduleId);
            foreach (KeyValuePair<string, JSONNode> m in msgNameIdMap)
            {
                var job = m.Value.AsObject;
                if (job ["id"].AsInt == moduleId)
                {
                    return m.Key;
                }
            }
            Debug.Log("name map file not found  ");
            return null;
        }

        public string getMethodName(string module, int msgId)
        {
            var msgs = msgNameIdMap [module].AsObject;
            foreach (KeyValuePair<string, JSONNode> m in msgs)
            {
                if (m.Key != "id")
                {
                    if (m.Value.AsInt == msgId)
                    {
                        return m.Key;
                    }
                }
            }
            return null;
        }

        public void AddNewAccount(string username, string password)
        {
            Log.GUI("AddNewAccount " + username + " " + password);
            var js = new JSONClass();
            js ["username"] = username;
            js ["password"] = password;
            otherAccounts.Insert(0, js);
            //currentSelect = 0;
        }

        public void SwapFirstAccount(int sel)
        {
            var sc = otherAccounts [sel];
            otherAccounts.RemoveAt(sel);
            otherAccounts.Insert(0, sc);
            //currentSelect = 0;
        }

        public Util.Pair GetMsgID(string msgName)
        {
            foreach (KeyValuePair<string, JSONNode> m in msgNameIdMap)
            {
                if (m.Value [msgName] != null)
                {
                    int a = m.Value ["id"].AsInt;
                    int b = m.Value [msgName].AsInt;
                    return new Util.Pair(a, b);     
                }
            }
            return null;
        }

        /// <summary>
        /// 加载save.json 文件保存的用户信息
        /// </summary>
        void LoadSaveFile()
        {
            string fpath = Path.Combine(Application.persistentDataPath, "save.json");
            Debug.Log("savepath " + fpath);
            Debug.Log("log path is " + Application.dataPath);
            var exist = File.Exists(fpath);
            FileStream fs = null;
            //Test No Account

            if (exist)
            {
                fs = new FileStream(fpath, FileMode.Open);
            }


            if (fs == null)
            {
                //saveData = null;
                saveData = new JSONClass();
            } else
            {
                byte[] buffer;
                try
                {
                    long len = fs.Length;
                    buffer = new byte[len];
                    int count;
                    int sum = 0;
                    while ((count = fs.Read(buffer, sum, (int)(len-sum))) > 0)
                    {
                        sum += count;
                    }
                } finally
                {
                    fs.Close();
                }

                saveData = JSON.Parse(System.Text.Encoding.UTF8.GetString(buffer));
            }

            if (saveData ["accounts"] != null)
            {
                foreach (JSONClass jc in saveData["accounts"].AsArray)
                {
                    otherAccounts.Add(jc);
                }
            }
        }

        public GameObject EffectMainNode = null;

        /// <summary>
        /// 初始化各个子模块
        /// </summary>
        public void InitData()
        {
			CScene.InitStatic();
            var g = gameObject;
            if (NotifyUIManager.Instance == null)
            {
                g.AddComponent<NotifyUIManager>();
            }
            if (BackPack.backpack == null)
            {
                var back = new GameObject("backpack");
                back.transform.parent = transform;
                back.AddComponent<BackPack>();
            }
            if (SkillDataController.skillDataController == null)
            {
                var skill = new GameObject("skill");
                skill.transform.parent = transform;
                skill.AddComponent<SkillDataController>();
            }
            

            if (CopyController.copyController == null)
            {
                var copy = new GameObject();
                copy.AddComponent<CopyController>();
                copy.transform.parent = g.transform;
            }

            if (WorldManager.worldManager == null)
            {
                var world = new GameObject("WorldManager");
                world.transform.parent = g.transform;
                world.AddComponent<WorldManager>();
            }

            if (ObjectManager.objectManager == null)
            {
                var go = new GameObject("ObjectManager");
                go.AddComponent<ObjectManager>();
            }


            if (CursorManager.cursorManager == null)
            {
                var t = CursorManager.cursorManager;
                t.transform.parent = g.transform;
            }

            characters = new List<GameObject>();
            if (saveData == null)
            {
                saveData = new JSONClass();
                saveData ["characters"] = new JSONArray();
                saveData ["lastSelect"].AsInt = -1;
            }
            /*
            if (MyEventSystem.myEventSystem == null) {
                var evt = new GameObject();
                evt.name = "EventSystem";
                evt.AddComponent<MyEventSystem>();
            }
            MyEventSystem.myEventSystem.transform.parent = g.transform;
            */
            MyEventSystem.myEventSystem.InitEventHandler();

            /*
            if (TeamController.teamController == null)
            {
                var tempObj = new GameObject("TeamController");
                tempObj.AddComponent<TeamController>();
                tempObj.transform.parent = transform;
            }
            */
            if (BuffManager.buffManager == null)
            {
                var tempObj = new GameObject("BuffManager");
                tempObj.AddComponent<BuffManager>();
                tempObj.transform.parent = transform;
            }
            var eff = new GameObject("EffectMainNode");
            eff.transform.parent = transform;
            EffectMainNode = eff;
        
            InitYet = true;
        }

        /*
         * Download File From HttpServer
         * Init ServerList
         * 
         * Test::Load From LocalFile
         * 
         */ 
        public void InitServerList()
        {
            var binData = Resources.Load("server") as TextAsset;
            serverList = JSON.Parse(binData.text).AsArray;
            Debug.Log("SaveGame::server " + serverList);
        }

        public void SaveFile()
        {
            //saveData ["username"] = username;
            //saveData ["password"] = password;

            var ja = new JSONArray();
            saveData ["accounts"] = ja;
            foreach (JSONClass jc in otherAccounts)
            {
                ja.Add(jc);
            }

            string fpath = Path.Combine(Application.persistentDataPath, "save.json");
            Debug.Log("save path " + fpath);
            using (StreamWriter outfile = new StreamWriter(fpath))
            {
                outfile.Write(saveData.ToString());
            }
        }

        static int sid = 0;
        // Update is called once per frame
        void Update()
        {
            //NetDebug.netDebug.AddConsole ("SaveGameUpdate "+sid++ );
        }

        public string GetDefaultUserName()
        {
            return otherAccounts [0] ["username"];
        }

        public string GetDefaultPassword()
        {
            return otherAccounts [0] ["password"];
        }

    }
}
