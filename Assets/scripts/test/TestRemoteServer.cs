using UnityEngine;
using System.Collections;
using System;

namespace MyLib
{
    public class TestRemoteServer : MonoBehaviour
    {
        #if UNITY_EDITOR
        public int myId ;
        MainThreadLoop ml;
        void Awake() {
            if(SaveGame.saveGame == null) {
                gameObject.AddComponent<SaveGame>();
            }
            ml = gameObject.AddComponent<MainThreadLoop>();
        }
        // Use this for initialization
        void Start()
        {
    
        }
    
        [ButtonCallFunc()]
        public bool
            Connect;

        void EvtHandler(RemoteClientEvent evt) {
        }
        RemoteClient rc;
        public void ConnectMethod()
        {
            rc = new RemoteClient(ml);
            rc.msgHandler = (KBEngine.Packet packet)=>{
                var proto = packet.protoBody as GCPlayerCmd;
                var cmds = proto.Result.Split(' ');
                if(cmds[0] == "Login") {
                    myId = Convert.ToInt32(cmds[1]);
                }
            };
            rc.evtHandler = EvtHandler;

            rc.Connect("127.0.0.1", 10001);

        }

        [ButtonCallFunc()]public bool Send;
        public void SendMethod() {
            /*
            var pk = CGLoginAccount.CreateBuilder();
            pk.Username = "liyong";
            pk.Password = "123456";
            */
            /*
            var ainfo = AvatarInfo.CreateBuilder();
            ainfo.X = 1;
            ainfo.Z = 1;
            */
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "Login";
            var data = KBEngine.Bundle.GetPacket(cg);
            rc.Send(data);
        }
        [ButtonCallFunc()]public bool InitData;
        public void InitDataMethod() {
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "InitData";
            var ainfo = AvatarInfo.CreateBuilder();
            ainfo.X = 10;
            ainfo.Z = 10;
            cg.AvatarInfo = ainfo.Build();
            var data = KBEngine.Bundle.GetPacket(cg);
            rc.Send(data);
        }
        private int p = 0;
        [ButtonCallFunc()]public bool UpdateData;
        public void UpdateDataMethod() {
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "UpdateData";
            var ainfo = AvatarInfo.CreateBuilder();
            ainfo.X = p++;
            ainfo.Z = p++;
            cg.AvatarInfo = ainfo.Build();
            var data = KBEngine.Bundle.GetPacket(cg);
            rc.Send(data);
        }
        [ButtonCallFunc()] public bool Close;
        public void CloseMethod() {
            rc.Disconnect();
        }
        #endif
    }

}