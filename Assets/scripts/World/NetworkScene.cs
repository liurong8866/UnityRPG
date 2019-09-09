using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// 只进行玩家的初始化 
    /// </summary>
    public class NetworkScene : MonoBehaviour
    {
        public int myId;
        MainThreadLoop ml;
        RemoteClient rc;
        private WorldState _s = WorldState.Idle;
        private string ServerIP = "127.0.0.1";

        public WorldState state
        {
            get
            {
                return _s;
            }
            set
            {
                if (_s != WorldState.Closed)
                {
                    _s = value;
                } else
                {
                    Debug.LogError("WorldHasQuit Not: " + value);
                }
            }
        }

        void Awake()
        {
            StartCoroutine(InitGameData());

            ml = gameObject.AddComponent<MainThreadLoop>();

            TextAsset bindata = Resources.Load("Config") as TextAsset;
            Debug.Log("nameMap " + bindata);
          
            ServerIP = ClientApp.Instance.remoteServerIP;
            Debug.LogError("ServerIP: " + ServerIP);

            state = WorldState.Connecting;
            //StartCoroutine(InitConnect());
        }

        IEnumerator  InitGameData()
        {
            GameInterface_Backpack.ClearDrug();
            yield return StartCoroutine(NetworkUtil.WaitForPlayer());
            var me = ObjectManager.objectManager.GetMyPlayer();
            GameInterface_Skill.AddSkillBuff(ObjectManager.objectManager.GetMyPlayer(), (int)SkillData.SkillConstId.AddMP, Vector3.zero);
            var attr = me.GetComponent<NpcAttribute>();

            var sync = CGSetProp.CreateBuilder();
            sync.Key = (int)CharAttribute.CharAttributeEnum.LEVEL;
            sync.Value = 1;
            KBEngine.Bundle.sendImmediate(sync);

            PlayerData.ResetSkillLevel();
        }


        private RemoteClientEvent lastEvt = RemoteClientEvent.None;

        void EvtHandler(RemoteClientEvent evt)
        {
            Debug.LogError("RemoteClientEvent: " + evt);
            lastEvt = evt;
            if (lastEvt == RemoteClientEvent.Close)
            {
                WindowMng.windowMng.ShowNotifyLog("和服务器断开连接：" + state);
                if (state != WorldState.Closed)
                {
                    Debug.LogError("ConnectionClosed But WorldNotClosed");
                    state = WorldState.Idle;
                    StartCoroutine(RetryConnect());
                }
            } else if (lastEvt == RemoteClientEvent.Connected)
            {
                WindowMng.windowMng.ShowNotifyLog("连接服务器成功：" + state);
            }
        }

        void MsgHandler(KBEngine.Packet packet)
        {
            var proto = packet.protoBody as GCPlayerCmd;
            Log.Net("Map4Receive: " + proto);
            var cmds = proto.Result.Split(' ');
            if (cmds [0] == "Login")
            {
                myId = Convert.ToInt32(cmds [1]);
                ObjectManager.objectManager.RefreshMyServerId(myId);
            } else if (cmds [0] == "Add")
            {
                ObjectManager.objectManager.CreateOtherPlayer(proto.AvatarInfo);
                PlayerDataInterface.DressEquip(proto.AvatarInfo);
                var player = ObjectManager.objectManager.GetPlayer(proto.AvatarInfo.Id);
                if (player != null)
                {
                    var sync = player.GetComponent<PlayerSync>();
                    if (sync != null)
                    {
                        sync.NetworkMove(proto.AvatarInfo);
                    } else
                    {
                        var sync2 = player.GetComponent<MySelfAttributeSync>();
                        sync2.NetworkAttribute(proto.AvatarInfo);
                    }
                }

            } else if (cmds [0] == "Remove")
            {
                ObjectManager.objectManager.DestroyPlayer(proto.AvatarInfo.Id); 
            } else if (cmds [0] == "Update")
            {
                var player = ObjectManager.objectManager.GetPlayer(proto.AvatarInfo.Id);
                if (player != null)
                {
                    var sync = player.GetComponent<PlayerSync>();
                    if (sync != null)
                    {
                        sync.NetworkMove(proto.AvatarInfo);
                    } else
                    {
                        var myselfAttr = player.GetComponent<MySelfAttributeSync>();
                        if (myselfAttr != null)
                        {
                            myselfAttr.NetworkAttribute(proto.AvatarInfo);
                        }
                    }
                } 

            } else if (cmds [0] == "Damage")
            {
                var dinfo = proto.DamageInfo;
                var enemy = ObjectManager.objectManager.GetPlayer(dinfo.Enemy);
                if (enemy != null)
                {
                    var sync = enemy.GetComponent<PlayerSync>();
                    if (sync != null)
                    {
                        sync.DoNetworkDamage(proto);
                    }
                }
                if (!NetworkUtil.IsMaster() && enemy != null)
                {
                    var sync = enemy.GetComponent<MonsterSync>();
                    if (sync != null)
                    {
                        sync.DoNetworkDamage(proto);
                    }
                }

            } else if (cmds [0] == "Skill")
            {
                var sk = proto.SkillAction;
                var player = ObjectManager.objectManager.GetPlayer(sk.Who);
                if (player != null)
                {
                    var sync = player.GetComponent<PlayerSync>();
                    if (sync != null)
                    {
                        sync.NetworkAttack(sk);
                    }
                }
            } else if (cmds [0] == "Buff")
            {
                var target = proto.BuffInfo.Target;
                var sync = NetDateInterface.GetPlayer(target);
                var player = ObjectManager.objectManager.GetPlayer(target);
                if (sync != null)
                {
                    sync.NetworkBuff(proto);
                }
                if (player != null && !NetworkUtil.IsNetMaster())
                {
                    var monSync = player.GetComponent<MonsterSync>();
                    if (monSync != null)
                    {
                        monSync.NetworkBuff(proto);
                    }
                }
            } else if (cmds [0] == "AddEntity")
            {
                var ety = proto.EntityInfo;
                if (ety.EType == EntityType.CHEST)
                {
                    StartCoroutine(WaitZoneInit(ety));
                } else if (ety.EType == EntityType.DROP)
                {
                    var itemData = Util.GetItemData((int)ItemData.GoodsType.Props, (int)ety.ItemId);
                    var itemNum = ety.ItemNum;
                    var pos = NetworkUtil.FloatPos(ety.X, ety.Y, ety.Z); 
                    DropItemStatic.MakeDropItemFromNet(itemData, pos, itemNum, ety);
                }

            } else if (cmds [0] == "UpdateEntity")
            {
                var ety = proto.EntityInfo;
                var mon = ObjectManager.objectManager.GetPlayer(ety.Id);
                Log.Net("UpdateEntityHP: " + ety.Id + " hp " + ety.HasHP + " h " + ety.HP);
                if (!NetworkUtil.IsMaster() && mon != null)
                {
                    var sync = mon.GetComponent<MonsterSync>();
                    if (sync != null)
                    {
                        sync.SyncAttribute(proto);
                    }
                }
            } else if (cmds [0] == "RemoveEntity")
            {
                var ety = proto.EntityInfo;
                var mon = ObjectManager.objectManager.GetPlayer(ety.Id);
                if (!NetworkUtil.IsMaster() && mon != null)
                {
                    var netView = mon.GetComponent<KBEngine.KBNetworkView>();
                    if (netView != null)
                    {
                        ObjectManager.objectManager.DestroyByLocalId(netView.GetLocalId());
                    }
                }
            } else if (cmds [0] == "Pick")
            {
                if (!NetworkUtil.IsMaster())
                {
                    var action = proto.PickAction;
                    var ety = ObjectManager.objectManager.GetPlayer(action.Id);
                    var who = ObjectManager.objectManager.GetPlayer(action.Who);
                    if (ety != null)
                    {
                        var item = ety.GetComponent<DropItemStatic>();
                        if (item != null)
                        {
                            item.PickItemFromNetwork(who);
                        }
                    }
                }
            } else if (cmds [0] == "Revive")
            {
                var player = ObjectManager.objectManager.GetPlayer(proto.AvatarInfo.Id);
                if (player != null)
                {
                    var sync = player.GetComponent<PlayerSync>();
                    if (sync != null)
                    {
                        sync.Revive();
                    }
                }
            } else if (cmds [0] == "Dead")
            {
                var dinfo = proto.DamageInfo;
                ScoreManager.Instance.NetAddScore(dinfo.Attacker, dinfo.Enemy);
            } else if (cmds [0] == "SyncTime")
            {
                if (!NetworkUtil.IsNetMaster())
                {
                    ScoreManager.Instance.NetSyncTime(proto.LeftTime);
                }
            } else if (cmds [0] == "GameOver")
            {
                if (!NetworkUtil.IsNetMaster())
                {
                    ScoreManager.Instance.NetworkGameOver();
                }
            } else if (cmds [0] == "AllReady")
            {
                Util.ShowMsg("所有客户端准备完成");
                //当所有客户端准备好之后 服务器推送Entity给所有客户端
                NetMatchScene.Instance.SetAllReady();

                //更新IsMaster 这样才能生成Entity
                var player = ObjectManager.objectManager.GetMyPlayer();
                var myselfAttr = player.GetComponent<MySelfAttributeSync>();
                var matchRoom = NetMatchScene.Instance.GetComponent<MatchRoom>();
                if (myselfAttr != null)
                {
                    myselfAttr.NetworkAttribute(matchRoom.GetMyInfo());
                }
            }
        }

        IEnumerator WaitZoneInit(EntityInfo ety)
        {
            var zone = BattleManager.battleManager.GetZone().GetComponent<ZoneEntityManager>();
            var unitData = Util.GetUnitData(false, ety.UnitId, 0);
            var spawnChest = zone.GetSpawnChest(ety.SpawnId);
            while (spawnChest == null)
            {
                yield return null;
                spawnChest = zone.GetSpawnChest(ety.SpawnId);
            }
            ObjectManager.objectManager.CreateChestFromNetwork(unitData, spawnChest, ety);
        }

        //设置发送Ready
        //当所有的Ready之后Master 会发送一个Go状态
        public void InitMap()
        {
            rc = NetMatchScene.Instance.rc;
            StartCoroutine(SendReady());
        }

        IEnumerator SendReady()
        {
            while (ObjectManager.objectManager.GetMyPlayer() == null)
            {
                yield return null;
            }
            //等待NetworkLoadZone 初始化完成
            yield return new WaitForSeconds(0.2f);
            rc.evtHandler = EvtHandler;
            rc.msgHandler = MsgHandler;
            state = WorldState.Connected;
            myId = NetMatchScene.Instance.myId;
            ObjectManager.objectManager.RefreshMyServerId(myId);



            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "Ready";
            var data = KBEngine.Bundle.GetPacket(cg);
            rc.Send(data);
            SendUserData();
            yield return StartCoroutine(SendCommandToServer());
        }

        IEnumerator InitConnect()
        {
            if (rc != null)
            {
                rc = null;
                yield return new WaitForSeconds(2);
            }

            //玩家自己模型尚未初始化准备完毕则不要连接服务器放置Logic之后玩家的ID没有设置
            while (ObjectManager.objectManager.GetMyPlayer() == null)
            {
                yield return null;
            }
            //重新构建新的连接
            rc = new RemoteClient(ml);
            rc.evtHandler = EvtHandler;
            rc.msgHandler = MsgHandler;

            rc.Connect(ServerIP, 10001);
            while (lastEvt == RemoteClientEvent.None && state == WorldState.Connecting)
            {
                yield return null;
            }
            Debug.LogError("StartInitData: " + lastEvt);
            if (lastEvt == RemoteClientEvent.Connected)
            {
                state = WorldState.Connected;
                yield return StartCoroutine(InitData());       
                yield return StartCoroutine(SendCommandToServer());
            } 
        
        }

        void SyncMyPos()
        {
            var me = ObjectManager.objectManager.GetMyPlayer();
            if (me != null)
            {
                var sync = me.GetComponent<PlayerSyncToServer>();
                sync.SyncAttribute();
            }
            //NetDateInterface.SyncPosDirHP();
            NetDateInterface.SyncMonster();
        }

        /// <summary>
        /// 周期性的同步属性状态到服务器上面 Diff属性
        /// lastAvatarInfo 比较后的属性 
        /// </summary>
        /// <returns>The command to server.</returns>
        IEnumerator SendCommandToServer()
        {
            Debug.LogError("SendCommandToServer");
            while (state == WorldState.Connected)
            {
                SyncMyPos();
                yield return new WaitForSeconds(ClientApp.Instance.syncFreq);
            }
        }

        void SendUserData()
        {
            Debug.Log("SendUserData");
            if (state != WorldState.Connected)
            {
                return;
            }
            if (rc == null)
            {
                return;
            }

            var me = ObjectManager.objectManager.GetMyPlayer();
            var pos = me.transform.position;
            var dir = (int)me.transform.localRotation.eulerAngles.y;

            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "InitData";
            var ainfo = AvatarInfo.CreateBuilder();
            ainfo.X = (int)(pos.x * 100);
            ainfo.Z = (int)(pos.z * 100);
            ainfo.Y = (int)(pos.y * 100);
            ainfo.Dir = dir;

            var pinfo = ServerData.Instance.playerInfo;
            foreach (var d in pinfo.DressInfoList)
            {
                ainfo.DressInfoList.Add(d);
            }
            ainfo.Level = ObjectManager.objectManager.GetMyProp(CharAttribute.CharAttributeEnum.LEVEL);
            ainfo.HP = ObjectManager.objectManager.GetMyProp(CharAttribute.CharAttributeEnum.HP);
            ainfo.Job = ServerData.Instance.playerInfo.Roles.Job;

            cg.AvatarInfo = ainfo.Build();
            var sync = me.GetComponent<PlayerSyncToServer>();
            sync.InitData(cg.AvatarInfo);

            var data = KBEngine.Bundle.GetPacket(cg);
            rc.Send(data);
        }

        IEnumerator InitData()
        {
            Debug.LogError("InitData");
            if (rc == null)
            {
                yield break;
            }
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "Login";
            var data = KBEngine.Bundle.GetPacket(cg);
            rc.Send(data);
            Debug.Log("SendLogin");

            while (myId == 0 && state == WorldState.Connected)
            {
                yield return new WaitForSeconds(1);
            }
            SendUserData();
        }

        /// <summary>
        /// 断开连接之后重新连接 
        /// </summary>
        /// <returns>The connect.</returns>
        IEnumerator RetryConnect()
        {
            yield return new WaitForSeconds(4);
            Debug.LogError("RetryConnect");
            //重试是否连接重置用户ID
            lastEvt = RemoteClientEvent.None;
            myId = 0;
            state = WorldState.Connecting;
            if (state == WorldState.Connecting)
            {
                StartCoroutine(InitConnect());
            }
        }


        void QuitWorld()
        {
            Debug.LogError("QuitWorld");
            state = WorldState.Closed;
            if (rc != null)
            {
                rc.evtHandler = null;
                rc.msgHandler = null;
                rc.Disconnect();
                rc = null;
            }
        }

        void  OnDestroy()
        {
            if (NetMatchScene.Instance != null)
            {
                UnityEngine.Object.Destroy(NetMatchScene.Instance.gameObject);
            }
            QuitWorld();
        }

        public void BroadcastMsg(CGPlayerCmd.Builder cg)
        {
            Log.Net("BroadcastMsg: " + cg);
            if (rc != null)
            {
                var data = KBEngine.Bundle.GetPacket(cg);
                rc.Send(data);
            }
        }
    }
}
