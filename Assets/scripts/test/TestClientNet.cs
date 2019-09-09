using UnityEngine;
using System.Collections;

public class TestClientNet : MonoBehaviour {

    [ButtonCallFunc()]public bool CloseNet;
    public void CloseNetMethod(){
        KBEngine.KBEngineApp.app.networkInterface().close();
    }
    [ButtonCallFunc()]public bool TestServer;
    public void TestServerMethod(){
        MyLib.DemoServer.demoServer.GetThread().CloseServerSocket();
    }
    [ButtonCallFunc()] public bool Save;
    public void SaveMethod(){
        MyLib.ServerData.Instance.SaveUserData();
    }

    public string file;
    [ButtonCallFunc()] public bool ReadFile;
    public void ReadFileMethod(){
        var f = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Protobuffer/"+file);
        var data = f.bytes;
        try {
            var playerInfo = MyLib.PlayerInfo.CreateBuilder().MergeFrom(data);
            Debug.Log(playerInfo.Build().ToString());
        }catch(System.Exception ex){
            Debug.LogError("PlayerInfo Load Error :"+ex);
        }

    }
    [ButtonCallFunc()]public bool Kill;
    public void KillMethod(){
        MyLib.BattleManager.battleManager.killAllMethod();
    }
    [ButtonCallFunc()]public bool Elite;
    public void EliteMethod(){
        MyLib.BattleManager.allElite = true;
    }

    [ButtonCallFunc()]public bool ignore;
    public void ignoreMethod() {
        MyLib.StoryDialog.Ignore = true;
    }
}
