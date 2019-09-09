using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class NetMatchScene : MonoBehaviour
    {
        public enum RoomState {
            InMatch,
            InGame,
            AllReady,
            GameOver,
        }
        public RoomState roomState{
            get;
            private set;
        }
        public void SetAllReady() {
            Log.Net("SetAllReady");
            roomState = RoomState.AllReady;
        }

        public int myId;
        MainThreadLoop ml;
        public RemoteClient rc{
            get;
            private set;
        }

        private WorldState _s = WorldState.Idle;
        private string ServerIP = "127.0.0.1";

        public RoomInfo roomInfo
        {
            get;
            private set;
        }

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

        private MatchRoom matchRoom;
        public static NetMatchScene Instance;

        void Awake()
        {
            roomState = RoomState.InMatch;
            if(Instance != null) {
                GameObject.Destroy(Instance.gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            matchRoom = gameObject.AddComponent<MatchRoom>();

            StartCoroutine(InitGameData());

            ml = gameObject.AddComponent<MainThreadLoop>();

            TextAsset bindata = Resources.Load("Config") as TextAsset;
            Debug.Log("nameMap " + bindata);
          
            ServerIP = ClientApp.Instance.remoteServerIP;
            Debug.LogError("ServerIP: " + ServerIP);

            state = WorldState.Connecting;
            StartCoroutine(InitConnect());
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
                //初始化数据
                yield return StartCoroutine(InitData());       
                yield return StartCoroutine(StartMatch());
                yield return StartCoroutine(WaitForTeamFull());
            } 
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
            Log.Net("ReceiveMsg: " + proto);
            var cmds = proto.Result.Split(' ');
            var c0 = cmds [0];
            if (c0 == "Add")
            {
                roomInfo.PlayersList.Add(proto.AvatarInfo);
                Util.ShowMsg("玩家:"+proto.AvatarInfo.Name+" 加入游戏，当前人数:"+matchRoom.GetPlayerNum());
            } else if (c0 == "Update")
            {
                foreach (var p in roomInfo.PlayersList)
                {
                    if (p.Id == proto.AvatarInfo.Id)
                    {
                        matchRoom.SyncAvatarInfo(proto.AvatarInfo, p);
                        break;
                    }
                }

            } else if (c0 == "Remove")
            {
                foreach (var p in roomInfo.PlayersList)
                {
                    if (p.Id == proto.AvatarInfo.Id)
                    {
                        roomInfo.PlayersList.Remove(p);
                        break;
                    }
                }
                Util.ShowMsg("玩家:"+proto.AvatarInfo.Name+" 离开游戏，当前人数:"+matchRoom.GetPlayerNum());
            } else if (c0 == "StartGame")
            {
                //进入Map5场景开始游戏
                //将网络状态数据保留 
                //等待所有玩家进入场景成功
                //EnterSuc 
                //然后将所有玩家状态重新刷新一遍
                Util.ShowMsg("玩家足够开始游戏："+matchRoom.GetPlayerNum());
                Log.Net("StartGame");
                roomState = RoomState.InGame;
                WorldManager.worldManager.WorldChangeScene(5, false);
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
            var data = KBEngine.Bundle.GetPacketFull(cg);
            yield return StartCoroutine(rc.SendWaitResponse(data.data, data.fid, (packet) =>
            {
                var proto = packet.protoBody as GCPlayerCmd;
                var cmds = proto.Result.Split(' ');
                myId = System.Convert.ToInt32(cmds [1]);
                ObjectManager.objectManager.RefreshMyServerId(myId);
            }));

            Log.Net("SendLogin: "+myId);
            SendUserData();
        }

        IEnumerator StartMatch()
        {
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "Match";
            var data = KBEngine.Bundle.GetPacketFull(cg);
            yield return StartCoroutine(rc.SendWaitResponse(data.data, data.fid, (packet) =>
            {
                var cmd = packet.protoBody as GCPlayerCmd;
                roomInfo = cmd.RoomInfo;
            }));
        }

        IEnumerator WaitForTeamFull()
        {
            Log.Net("WaitForTeamFull: ");
            //等待服务器同步谁是Master
            while (!matchRoom.IsMeMaster() && roomState == RoomState.InMatch)
            {
                yield return null;
            }

            while (!matchRoom.GetPlayerFull() && roomState == RoomState.InMatch)
            {
                yield return null;
            }

            if (matchRoom.IsMeMaster() && roomState == RoomState.InMatch)
            {
                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "StartGame";
                this.BroadcastMsg(cg);
            }
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
    }
}
