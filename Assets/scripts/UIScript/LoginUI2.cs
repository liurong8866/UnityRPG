using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class LoginUI2 : IUserInterface
    {
        UIInput ip;
        UIInput port;
        UIInput sync;
        void Awake(){
            SetCallback("StartButton", OnStart);
            ip = GetInput("IPInput");
            port = GetInput("PortInput");
            SetCallback("StartServer", OnServer);
            sync = GetInput("SyncInput");
        }
        private bool serverYet = false;
        void OnServer() {
            var ca = ClientApp.Instance;
            ca.remoteServerIP = ip.value;
            ca.testPort = System.Convert.ToInt32(port.value);
            ca.syncFreq = System.Convert.ToSingle(sync.value);
            ca.StartServer();
            serverYet = true;
        }

        void OnStart(GameObject g){
            if(!serverYet) {
                OnServer();
            }
            GameInterface_Login.loginInterface.LoginGame();
        }

      
    }

}