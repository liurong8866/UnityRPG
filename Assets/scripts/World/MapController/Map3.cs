using UnityEngine;
using System.Collections;
using System;

namespace MyLib
{
    public enum WorldState
    {
        Idle,
        Connecting,
        Connected,
        Closed,
    }

    /// <summary>
    /// Idle
    /// Connecting
    /// Connected
    /// Closed
    /// </summary>
    public class Map3 : CScene
    {
        public int myId = 0;
        MainThreadLoop ml;
        RemoteClient rc;
        private WorldState _s = WorldState.Idle;

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

        public override bool IsNet
        {
            get
            { 
                return true;
            }
        }

        private string ServerIP = "127.0.0.1";

        protected override void Awake()
        {
            base.Awake();
            ml = gameObject.AddComponent<MainThreadLoop>();

            TextAsset bindata = Resources.Load("Config") as TextAsset;
            Debug.Log("nameMap " + bindata);
            if (bindata != null)
            {
                ServerIP = SimpleJSON.JSON.Parse(bindata.text).AsObject ["Server"];
            }
            Debug.LogError("ServerIP: " + ServerIP);

            state = WorldState.Connecting;
            StartCoroutine(InitConnect());
        }

        /// <summary>
        /// 协程改造的Connect和Send函数 
        /// </summary>
        /// <returns>The connect.</returns>
        IEnumerator InitConnect()
        {
            if(rc != null) {
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
                yield return null ;
            }
            Debug.LogError("StartInitData: " + lastEvt);
            if (lastEvt == RemoteClientEvent.Connected)
            {
                state = WorldState.Connected;
                yield return StartCoroutine(InitData());       
                yield return StartCoroutine(SendCommandToServer());
            } else
            {
                //StartCoroutine(RetryConnect());
            }
        }


        void SyncMyPos()
        {
            NetDateInterface.SyncPosDirHP();
        }

        IEnumerator SendCommandToServer()
        {
            Debug.LogError("SendCommandToServer");
            while (state == WorldState.Connected)
            {
                SyncMyPos();
                yield return new WaitForSeconds(0.5f);
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

            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "InitData";
            var ainfo = AvatarInfo.CreateBuilder();
            ainfo.X = (int)(pos.x * 100);
            ainfo.Z = (int)(pos.z * 100);
            ainfo.Y = (int)(pos.y * 100);
            var pinfo = ServerData.Instance.playerInfo;
            foreach (var d in pinfo.DressInfoList)
            {
                ainfo.DressInfoList.Add(d);
            }
            ainfo.Level = ObjectManager.objectManager.GetMyProp(CharAttribute.CharAttributeEnum.LEVEL);
            ainfo.HP = ObjectManager.objectManager.GetMyProp(CharAttribute.CharAttributeEnum.HP);

            cg.AvatarInfo = ainfo.Build();
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

        private RemoteClientEvent lastEvt = RemoteClientEvent.None;

        void EvtHandler(RemoteClientEvent evt)
        {
            Debug.LogError("RemoteClientEvent: " + evt);
            lastEvt = evt;
            if (lastEvt == RemoteClientEvent.Close)
            {
                WindowMng.windowMng.ShowNotifyLog("和服务器断开连接："+state);
                if (state != WorldState.Closed)
                {
                    Debug.LogError("ConnectionClosed But WorldNotClosed");
                    state = WorldState.Idle;
                    StartCoroutine(RetryConnect());
                }
            }else if(lastEvt == RemoteClientEvent.Connected) {
                WindowMng.windowMng.ShowNotifyLog("连接服务器成功："+state);
            }
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


        void MsgHandler(KBEngine.Packet packet)
        {
            var proto = packet.protoBody as GCPlayerCmd;
            Debug.Log("Map3Receive: " + proto);
            var cmds = proto.Result.Split(' ');
            if (cmds [0] == "Login")
            {
                myId = Convert.ToInt32(cmds [1]);
                ObjectManager.objectManager.RefreshMyServerId(myId);
            } else if (cmds [0] == "Add")
            {
                ObjectManager.objectManager.CreateOtherPlayer(proto.AvatarInfo);
                PlayerDataInterface.DressEquip(proto.AvatarInfo);

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
                    }
                }else {
                }

            } else if (cmds [0] == "Damage")
            {
                //SkillDamageCaculate.DoNetworkDamage(proto);
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
                var sync = NetDateInterface.GetPlayer(proto.BuffInfo.Target);
                if (sync != null)
                {
                    sync.NetworkBuff(proto);
                }
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

        protected override void  OnDestroy()
        {
            base.OnDestroy();
            QuitWorld();
        }

        public override void BroadcastMsg(CGPlayerCmd.Builder cg)
        {
            if (rc != null)
            {
                var data = KBEngine.Bundle.GetPacket(cg);
                rc.Send(data);
            }
        }
    }

}