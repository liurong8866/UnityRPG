
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
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace MyLib
{
    public class ObjectManager : MonoBehaviour
    {
        Dictionary<int, GameObject> fakeObjects = new Dictionary<int,GameObject>();
        public static ObjectManager objectManager;
        public KBEngine.KBPlayer myPlayer;

        /*
		 * 玩家服务器Id--->玩家实体
         * 
         * View 普通的怪物等实体
         * LocalId  playerId
         * ViewId
		 */
        public Dictionary<int, KBEngine.KBPlayer> Actors = new Dictionary<int, KBEngine.KBPlayer>();
        List<KBEngine.KBNetworkView> photonViewList = new List<KBEngine.KBNetworkView>();

        public List<KBEngine.KBNetworkView> GetNetView()
        {
            return photonViewList;
        }
        /*
		 * Monster Killed By Player
		 */
        //TODO: 使用MyEventSystem 来发送事件
        public VoidDelegate killEvent;

        /// <summary>
        /// 获得玩家实体对象
        /// 获得服务器怪物实体
        /// </summary>
        public KBEngine.KBNetworkView GetPlayerOrMonsterNetView(int playerId)
        {
            foreach (KBEngine.KBNetworkView view in photonViewList)
            {
                if (view != null && view.GetServerID() == playerId)
                {
                    return view;
                }
            }
            return null;
        }

        public GameObject GetFakeObj(int localId)
        {
            GameObject g;
            if (fakeObjects.TryGetValue(localId, out g))
            {
                return g;
            }
            return null;
        }

        //对象不一定是服务器对象因此通过localId来区分
        public GameObject GetLocalPlayer(int localId)
        {
            foreach (KBEngine.KBNetworkView p in photonViewList)
            {
                if (p.GetLocalId() == localId)
                {
                    return p.gameObject;
                }
            }
            return null;
        }

        public int GetMyProp(CharAttribute.CharAttributeEnum prop)
        {
            if (myPlayer != null)
            {
                var vi = GetPlayerOrMonsterNetView(myPlayer.ID);
                if (vi != null)
                {
                    return vi.GetComponent<CharacterInfo>().GetProp(prop);
                }
            }
            return 0;
        }

        public CharacterInfo GetMyData()
        {
            if (myPlayer != null)
            {
                var vi = GetPlayerOrMonsterNetView(myPlayer.ID);
                if (vi != null)
                {
                    return vi.GetComponent<CharacterInfo>();
                }
            }
            return null;
        }

        public NpcAttribute GetMyAttr()
        {
            var myplayer = GetMyPlayer();
            if (myplayer != null)
            {
                return myplayer.GetComponent<NpcAttribute>();
            }
            return null;
        }

        public IEnumerator<GameObject> GetAllPlayer()
        {
            foreach (var p in photonViewList)
            {
                if (p.IsPlayer)
                {
                    yield return p.gameObject;
                }
            }
        }

        public GameObject GetMyPlayer()
        {
            if (myPlayer != null)
            {
                var vi = GetPlayerOrMonsterNetView(myPlayer.ID);
                if (vi != null)
                {
                    return vi.gameObject;
                }
            }
            return null;
        }

        //获得玩家自身或者其它玩家的属性数据
        public GameObject GetPlayer(int playerId)
        {
            var view = GetPlayerOrMonsterNetView(playerId);
            if (view != null)
            {
                return view.gameObject;
            }
            return null;
        }

        //登录的时候 从SelectChar中获取等级数据
        //平时从 CharacterInfo 数据源获取数据
        //因为CharacterInfo 数据的初始化是在CreateMyPlayer 之后的
        //CharacterInfo SetProps Level 这里的值也要修改
        int GetMyLevel()
        {
            return SaveGame.saveGame.selectChar.Level;
        }

        /*
		 * User Data:
		 * 		1 SaveGame  static Data Not Changed in Game
		 * 		2 CharacterData   Dynamic Data Changed in Game
		 * 		3 SaveGame InitPosition use once 
		 */

        /*
		 * SaveGame selectChar has My PlayerID
		 * But Here just Return -1 As My Id
		 */
        public int GetMyServerID()
        {
            if (myPlayer != null)
            {
                return myPlayer.ID;
            }
            return -1;
        }

        /*
		 * Local Allocated NpcID
		 */
        public int GetMyLocalId()
        {
            if (myPlayer != null)
            {
                //Debug.Log("MyLocal Id Get Error "+);
                //Log.Sys ("Local PlayerID " + myPlayer.ID);
                var view = GetPlayerOrMonsterNetView(myPlayer.ID);
                if (view != null)
                {
                    var lid = view.GetLocalId();
                    return lid;
                }
            }

            //Debug.LogError("Not Found MyPlayer "+myPlayer);
            return -1;
        }


        private Vector3 GetMyInitRot()
        {
            var dir = SaveGame.saveGame.bindSession.Direction;
            return Quaternion.Euler(new Vector3(0, dir, 0)) * Vector3.forward;
        }

        public string GetMyName()
        {
            return SaveGame.saveGame.selectChar.Name;
        }




        public int GetMyJob()
        {
            //return (int)SaveGame.saveGame.selectChar.Job;
            Log.Sys("GetMyJob: " + ServerData.Instance.playerInfo.Roles.Job);
            return (int)ServerData.Instance.playerInfo.Roles.Job;
        }

        void Awake()
        {
            objectManager = this;
            DontDestroyOnLoad(this.gameObject);
        }

        //增加一个玩家实体对象
        public void AddObject(long unitId, KBEngine.KBNetworkView view)
        {
            photonViewList.Add(view);
        }

        /*
		 * PlayerId MySelf self is -1
		 * TODO:如果WorldManager正在进入新的场景，则缓存当前的服务器推送的Player,等待彻底进入场景再初始化Player
		 * TODO:CScene 进入场景之后解开缓存的Player数据
		 */
        private void AddPlayer(int unitId, KBEngine.KBPlayer player)
        {
            if (WorldManager.worldManager.station == WorldManager.WorldStation.Enter)
            {
                Actors.Add(unitId, player);
            } else
            {
                throw new SystemException("正在切换场景，增加新的Player失败");
            }
        }




        public void DestroyByLocalId(int localId)
        {
            var keys = photonViewList.Where(f => true).ToArray();
            foreach (KBEngine.KBNetworkView v in keys)
            {
                if (v.GetLocalId() == localId)
                {
                    photonViewList.Remove(v);
                    DestroyFakeObj(v.GetLocalId());
                    GameObject.Destroy(v.gameObject);
                    break;
                }
            }
        }

        //删除Player和PhotonView
        public void DestroyPlayer(int playerID)
        {
            ///<summary>
            /// 删除自己玩家的时候需要保存玩家的位置和角度Ch
            /// </summary>
            KBEngine.KBPlayer player = null;
            if (myPlayer != null && myPlayer.ID == playerID)
            {
                player = myPlayer;
                myPlayer = null;
            }

            if (Actors.ContainsKey(playerID))
            {
                player = Actors [playerID];
            }

            //摧毁某个玩家所有的PhotonView对象  Destroy Fake object Fist Or Send Event ?
            //删除玩家控制的怪物实体
            var keys = photonViewList.Where(f => true).ToArray();
            foreach (KBEngine.KBNetworkView v in keys)
            {
                if (v == null)
                {
                    photonViewList.Remove(v);
                } else
                {
                    if (v.GetID().owner == player)
                    {
                        photonViewList.Remove(v);
                        DestroyFakeObj(v.GetLocalId());
                        GameObject.Destroy(v.gameObject);
                    }
                }
            }

            if (Actors.ContainsKey(playerID))
            {
                Actors.Remove(playerID);
            }
        }

        /// <summary>
        /// 摧毁自己玩家对象和单人副本怪物对象
        /// </summary>
        public void DestroyMySelf()
        {
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.PlayerLeaveWorld);
            //删除我自己玩家
            if (myPlayer != null)
            {
                DestroyPlayer(myPlayer.ID);
            }

        }


        /*
		 * 显示私聊人物信息
		 */
        public void ShowCharInfo(GCLoadMChatShowInfo info)
        {
        }

        //加载对应模型的基础职业骨架 FakeObject
        public GameObject NewFakeObject(int localId)
        {
            //每次显示都要初始化一下FakeObj的装备信息
            if (fakeObjects.ContainsKey(localId))
            {
                //fakeObjects [localId].GetComponent<NpcEquipment> ().InitDefaultEquip();
                fakeObjects [localId].GetComponent<NpcEquipment>().InitFakeEquip();
                return fakeObjects [localId];
            }

            var player = GetLocalPlayer(localId);
            var job = player.GetComponent<NpcAttribute>().ObjUnitData.job;
            Log.Sys("DialogPlayer is " + job.ToString());
            //var fakeObject = Instantiate (Resources.Load<GameObject> ("DialogPlayer/" + job.ToString ())) as GameObject;
            var fakeObject = SelectChar.ConstructChar(job);
            fakeObject.name = fakeObject.name + "_fake";


            fakeObject.SetActive(false);
            fakeObjects [localId] = fakeObject;
            fakeObject.GetComponent<NpcEquipment>().SetFakeObj();
            fakeObject.GetComponent<NpcEquipment>().SetLocalId(localId);
            fakeObject.GetComponent<NpcEquipment>().InitFakeEquip();

            Util.SetLayer(fakeObject, GameLayer.PlayerCamera);
            return fakeObject;
        }

        //当玩家对象被删除的时候,删除对应的玩家的FakeObj
        public void DestroyFakeObj(int localId)
        {
            var fake = GetFakeObj(localId);
            if (fake != null)
            {
                fakeObjects.Remove(localId);
                GameObject.Destroy(fake);
            }
        }

        /// <summary> 
        /// 副本内 :: 我方玩家构建流程 
        /// 
        /// 角色的初始化位置不同
        /// </summary>
        public GameObject CreateMyPlayerInCopy()
        {
            var player = CreateMyPlayerInternal();
            SetStartPointPosition(player);
            return player;
        }

        GameObject CreateMyPlayerInternal()
        {
            var kbplayer = new KBEngine.KBPlayer();
            kbplayer.ID = (int)-2;
            if (myPlayer != null)
            {
                throw new System.Exception("myPlayer not null");
            }
			
            myPlayer = kbplayer;
			
            var job = (MyLib.Job)GetMyJob();
            var udata = Util.GetUnitData(true, (int)job, GetMyLevel());
            var player = Instantiate(Resources.Load<GameObject>(udata.ModelName)) as GameObject;
			
            NetDebug.netDebug.AddConsole("Init Player tag layer transform");
            NGUITools.AddMissingComponent<NpcAttribute>(player);
			NGUITools.AddMissingComponent<PlayerAIController>(player);
            player.tag = "Player";
            player.layer = (int)GameLayer.Npc;
            player.transform.parent = transform;
			

            //设置自己玩家的View属性
            var view = player.GetComponent<KBEngine.KBNetworkView>();
            NetDebug.netDebug.AddConsole("SelectCharID " + SaveGame.saveGame.selectChar.PlayerId);
            view.SetID(new KBEngine.KBViewID(-1, kbplayer));
			
            var npcAttr = player.GetComponent<NpcAttribute>();
            NetDebug.netDebug.AddConsole("Set UnitData of Certain Job " + udata);
            player.GetComponent<NpcAttribute>().SetObjUnitData(udata);
            player.GetComponent<NpcEquipment>().InitDefaultEquip();
            player.GetComponent<NpcEquipment>().InitPlayerEquipmentFromBackPack();
            npcAttr.InitName();
			
            player.name = "player_me";
			
            ObjectManager.objectManager.AddObject(SaveGame.saveGame.selectChar.PlayerId, view);
			
            var light = GameObject.Instantiate(Resources.Load<GameObject>("light/playerLight")) as GameObject;
            light.transform.parent = player.transform;
            light.transform.localPosition = Vector3.zero;

            NetDebug.netDebug.AddConsole("LevelInit::Awake Initial kbplayer Initial KbNetowrkView " + kbplayer + " " + view);
            return player;
        }

        void SetStartPointPosition(GameObject player)
        {
            if (NetworkUtil.IsNet())
            {
                var pos = NetworkUtil.GetStartPos();
                player.transform.position = pos;
            } else
            {
                var startPoint = GameObject.Find("PlayerStart");
                player.transform.position = startPoint.transform.position;
                player.transform.forward = startPoint.transform.forward;
            }
        }

        void SetCityStartPos(GameObject player)
        {
            SetStartPointPosition(player);
        }

        ///<summary>
        /// 主城内
        /// 
        /// 我方玩家构建流程 野外 
        /// 	可能从副本退出    从BackPack 初始化装备
        /// 	也可能是刚登陆    需要等待BackPack 初始化结束 通知 穿戴装备
        /// 从副本退出则初始位置在刚才进入副本的位置
        /// 初次登陆的初始位置在登陆的属性中
        /// 
        /// 角色的初始化位置不同
        ///</summary> 
        public GameObject CreateMyPlayerInCity()
        {
            NetDebug.netDebug.AddConsole("LoginMyPlayer");
            var player = CreateMyPlayerInternal();
            SetCityStartPos(player);

            NetDebug.netDebug.AddConsole("ObjectManager Init Player Over");
            return player;
        }

        class MonsterInit
        {
            public UnitData unitData;
            public SpawnTrigger spawn;
            public GameObject spawnObj;

            public MonsterInit(UnitData ud, SpawnTrigger sp)
            {
                unitData = ud;
                spawn = sp;
            }

            public MonsterInit(UnitData ud, GameObject obj)
            {
                unitData = ud;
                spawnObj = obj;
            }
        }

        List<MonsterInit> cacheMonster = new List<MonsterInit>();
        List<MonsterInit> cacheNpc = new List<MonsterInit>();

        public void InitCache()
        {
            InitCacheMonster();
            InitCacheNpc();
        }

        void InitCacheMonster()
        {
            foreach (MonsterInit m in cacheMonster)
            {
                CreateMonster(m.unitData, m.spawn);
            }
            cacheMonster.Clear();
        }

        void InitCacheNpc()
        {
            foreach (MonsterInit m in cacheNpc)
            {
                CreateNpc(m.unitData, m.spawnObj);
            }
            cacheNpc.Clear();
        }

        /// <summary>
        /// 创建其它玩家
        /// 玩家所在场景
        /// </summary>
        /// <param name="ainfo">Ainfo.</param>
        public void CreateOtherPlayer(AvatarInfo ainfo)
        {
            if (WorldManager.worldManager.station == WorldManager.WorldStation.Enter)
            {
                Log.Sys("CreateOtherPlayer: " + ainfo);
                var oldPlayer = GetPlayer(ainfo.Id);
                if (oldPlayer != null)
                {
                    Debug.LogError("PlayerExists: " + ainfo);
                    return;
                }

                if (myPlayer != null && myPlayer.ID == ainfo.Id)
                {
                    Debug.LogError("CreateMeAgain");
                    return;
                }

                var kbplayer = new KBEngine.KBPlayer();
                kbplayer.ID = ainfo.Id;

                var udata = Util.GetUnitData(true, (int)ainfo.Job, 1);
                var player = GameObject.Instantiate(Resources.Load<GameObject>(udata.ModelName)) as GameObject;

                var attr = NGUITools.AddMissingComponent<NpcAttribute>(player);
                //状态机类似 之后可能需要修改为其它玩家状态机
				NGUITools.AddMissingComponent<OtherPlayerAI>(player);

                player.tag = "Player";
                player.layer = (int)GameLayer.Npc;

                NGUITools.AddMissingComponent<SkillInfoComponent>(player);
                player.GetComponent<NpcAttribute>().SetObjUnitData(udata);
                player.GetComponent<NpcEquipment>().InitDefaultEquip();

                player.name = "player_" + ainfo.Id;
                player.transform.parent = gameObject.transform;

                var netview = player.GetComponent<KBEngine.KBNetworkView>();
                netview.SetID(new KBEngine.KBViewID(-1, kbplayer));
                

                AddPlayer(kbplayer.ID, kbplayer);
                AddObject(netview.GetServerID(), netview);
                attr.Init();
                var sync = player.GetComponent<MyLib.PlayerSync>();
                sync.SetPositionAndDir(ainfo);
                sync.SetLevel(ainfo);
            } else
            {
                 
            }
        }

        public void RefreshMyServerId(int id)
        {
            if (myPlayer != null)
            {
                Log.Sys("RefreshMyServerId: " + id);
                myPlayer.ID = id;
                var startPos = NetworkUtil.GetStartPos();
                GetMyPlayer().transform.position = startPos;
            }
        }


        public void CreateNpc(UnitData unitData, GameObject spawn)
        {
            if (WorldManager.worldManager.station == WorldManager.WorldStation.Enter)
            {
                var Resource = Resources.Load<GameObject>(unitData.ModelName);
                GameObject g = Instantiate(Resource) as GameObject;
                if (g.GetComponent<CharacterController>() == null)
                {
                    var c = g.AddComponent<CharacterController>();
                    c.center = new Vector3(0, 0.7f, 0);
                    c.height = 1.6f;
                }

                NpcAttribute npc = NGUITools.AddMissingComponent<NpcAttribute>(g);

                var type = Type.GetType("MyLib." + unitData.AITemplate);
                var t = typeof(NGUITools);
                var m = t.GetMethod("AddMissingComponent");
                Log.AI("Monster Create Certain AI  " + unitData.AITemplate + " " + type);
                var geMethod = m.MakeGenericMethod(type);
                geMethod.Invoke(null, new object[]{ g });// as AIBase;

                g.transform.parent = transform;
                g.tag = GameTag.Npc; //Player Or Npc 
                g.layer = (int)GameLayer.IgnoreCollision;

                //Register Unique Id To Npc 
                var netView = g.GetComponent<KBEngine.KBNetworkView>();
                netView.SetID(new KBEngine.KBViewID(-1, myPlayer));
                netView.IsPlayer = false;

                npc.SetObjUnitData(unitData);
                AddObject(netView.GetServerID(), netView);

                npc.transform.position = spawn.transform.position;
                var rotY = spawn.transform.localRotation.eulerAngles.y;
                npc.transform.localRotation = Quaternion.Euler(new Vector3(0, rotY, 0)); 

                NpcManager.Instance.RegNpc(unitData.name, g);
            } else
            {
                cacheNpc.Add(new MonsterInit(unitData, spawn));
            }
        }

        public void CreateChestFromNetwork(UnitData unitData, SpawnChest spawn, EntityInfo info = null)
        {
            Log.Sys("Create Chest Unit " + unitData.name);
            var Resource = Resources.Load<GameObject>(unitData.ModelName);
            GameObject g = Instantiate(Resource) as GameObject;
            NpcAttribute npc = NGUITools.AddMissingComponent<NpcAttribute>(g);
            npc.spawnTrigger = spawn.gameObject;

            g.transform.parent = transform;
            g.tag = GameTag.Enemy;
            g.layer = (int)GameLayer.Npc;

            var type = Type.GetType("MyLib." + unitData.AITemplate);
            var t = typeof(NGUITools);
            var m = t.GetMethod("AddMissingComponent");
            Log.AI("Monster Create Certain AI  " + unitData.AITemplate + " " + type);
            var geMethod = m.MakeGenericMethod(type);
            geMethod.Invoke(null, new object[]{ g });// as AIBase;


            var netView = g.GetComponent<KBEngine.KBNetworkView>();

            //服务器返回的ViewId
            //Owner 客户端怪物 服务器怪物
            //Id ViewId
            if (info != null)
            {
                netView.SetID(new KBEngine.KBViewID(info.Id, myPlayer));
            } else
            {
                netView.SetID(new KBEngine.KBViewID(-1, myPlayer));
            }

            netView.IsPlayer = false;

            npc.SetObjUnitData(unitData);
            AddObject(netView.GetServerID(), netView);

            //不算怪物允许不去打
            if (info != null)
            {
                npc.transform.position = NetworkUtil.FloatPos(info.X, info.Y, info.Z);
            } else
            {
                npc.transform.position = spawn.transform.position;
            }


            BattleManager.battleManager.AddEnemy(npc.gameObject);
            npc.SetDeadDelegate = BattleManager.battleManager.EnemyDead;


            var sync = npc.GetComponent<MonsterSync>();
            if (sync != null)
            {
                sync.SyncAttribute(info);
            }
        }

        public GameObject GetNetworkMonster(int viewId)
        {
            foreach (var p in photonViewList)
            {
                if (p.GetViewId() == viewId)
                {
                    return p.gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Master 退出场景所有的Entities都销毁掉简单处理
        /// </summary>
        /// <param name="unitData">Unit data.</param>
        /// <param name="spawn">Spawn.</param>
        public void CreateChest(UnitData unitData, SpawnChest spawn)
        {

            if (NetworkUtil.IsNetMaster())
            {
                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "AddEntity";
                var entityInfo = EntityInfo.CreateBuilder();
                entityInfo.UnitId = unitData.ID;
                var ip = NetworkUtil.ConvertPos(spawn.transform.position);
                entityInfo.X = ip [0];
                entityInfo.Y = ip [1];
                entityInfo.Z = ip [2];
                entityInfo.SpawnId = spawn.SpawnId;
                entityInfo.HP = unitData.HP;
                cg.EntityInfo = entityInfo.Build();
                var scene = WorldManager.worldManager.GetActive();
                scene.BroadcastMsg(cg);
            } else
            {
                CreateChestFromNetwork(unitData, spawn);
            }
        }


        ///<summary>
        /// 副本内怪物构建流程
        /// 单人副本通过Scene配置来产生怪物
        /// 多人副本通过服务器推送消息产生怪物
        /// </summary>
        //TODO: 多人副本怪物产生机制
        public void CreateMonster(UnitData unitData, SpawnTrigger spawn)
        {
            if (WorldManager.worldManager.station == WorldManager.WorldStation.Enter)
            {
                Log.Sys("UnityDataIs " + unitData.ID);
                if (unitData.Config == null)
                {
                    Debug.LogError("NotFoundMonster " + unitData.ID);
                    return;
                }

                Log.Sys("Create Monster Unit " + unitData.name);
                var Resource = Resources.Load<GameObject>(unitData.ModelName);
                //本地怪兽不需要Player信息
                GameObject g = Instantiate(Resource) as GameObject;
                NpcAttribute npc = NGUITools.AddMissingComponent<NpcAttribute>(g);
                npc.spawnTrigger = spawn.gameObject;

                var type = Type.GetType("MyLib." + unitData.AITemplate);
                var t = typeof(NGUITools);
                var m = t.GetMethod("AddMissingComponent");
                Log.AI("Monster Create Certain AI  " + unitData.AITemplate + " " + type);
                var geMethod = m.MakeGenericMethod(type);
                //var petAI = 
                geMethod.Invoke(null, new object[]{ g });// as AIBase;


                g.transform.parent = transform;
                g.tag = GameTag.Enemy;
                g.layer = (int)GameLayer.Npc;
                spawn.FirstMonster = g;

                var netView = g.GetComponent<KBEngine.KBNetworkView>();
                netView.SetID(new KBEngine.KBViewID(-1, myPlayer));
                netView.IsPlayer = false;

                npc.SetObjUnitData(unitData);
                AddObject(netView.GetServerID(), netView);

			
                float angle = UnityEngine.Random.Range(0, 360);
                Vector3 v = Vector3.forward;
                v = Quaternion.Euler(new Vector3(0, angle, 0)) * v;
                float rg = UnityEngine.Random.Range(0, spawn.Radius);

                npc.transform.position = spawn.transform.position + v * rg;
                if (unitData.IsElite)
                {
                    npc.transform.localScale = new Vector3(2, 2, 2);
                }

                BattleManager.battleManager.AddEnemy(npc.gameObject);
                npc.SetDeadDelegate = BattleManager.battleManager.EnemyDead;
                //npc.Level = spawn.Level;
            } else
            {
                cacheMonster.Add(new MonsterInit(unitData, spawn));
            }
        }

        public void CreatePet(int monsterId, GameObject owner, Affix affix, Vector3 pos)
        {
            Log.Sys("Create Pet " + monsterId + " " + owner + " " + pos);
            if (owner == null)
            {
                Debug.LogError("Own NotExist Pet Not Born");
                return;
            }
            var unitData = Util.GetUnitData(false, monsterId, 1);

            var Resource = Resources.Load<GameObject>(unitData.ModelName);

            GameObject g = Instantiate(Resource) as GameObject;
            NpcAttribute npc = NGUITools.AddMissingComponent<NpcAttribute>(g);
            var type = Type.GetType("MyLib." + unitData.AITemplate);
            var t = typeof(NGUITools);
            var m = t.GetMethod("AddMissingComponent");
            Log.AI("Create Certain AI  " + unitData.AITemplate + " " + type);
            var geMethod = m.MakeGenericMethod(type);
            //var petAI = 
            geMethod.Invoke(null, new object[]{ g });// as AIBase;
            //var petAI = 
            //NGUITools.AddMissingComponent<type> (g);

            g.transform.parent = transform;
            g.tag = owner.tag;
            g.layer = (int)GameLayer.Npc;


            npc.SetOwnerId(owner.GetComponent<KBEngine.KBNetworkView>().GetLocalId());
            npc.spawnTrigger = owner.GetComponent<NpcAttribute>().spawnTrigger;

            //不可移动Buff
            //持续时间Buff
            //无敌不可被攻击Buff
            //火焰陷阱的特点 特点组合
            g.GetComponent<BuffComponent>().AddBuff(affix);

            var netView = NGUITools.AddMissingComponent<KBEngine.KBNetworkView>(g);
            netView.SetID(new KBEngine.KBViewID(-1, myPlayer));
            netView.IsPlayer = false;
            //owner.GetComponent<NpcAttribute>().AddSummon(netView.gameObject);
			
            npc.SetObjUnitData(unitData);
            AddObject(netView.GetServerID(), netView);

            npc.transform.position = pos;	

            if (unitData.IsElite)
            {
                npc.transform.localScale = new Vector3(2, 2, 2);
            }

            if (npc.tag == GameTag.Enemy)
            {
                BattleManager.battleManager.AddEnemy(npc.gameObject);
                npc.SetDeadDelegate = BattleManager.battleManager.EnemyDead;
            }
        }

        public List<GameObject> GetSummons(int localId)
        {
            var ret = new List<GameObject>();
            foreach (var g in photonViewList)
            {
                if (g.GetComponent<NpcAttribute>().OwnerId == localId)
                {
                    ret.Add(g.gameObject);
                }
            }
            return ret;
        }
    }

}