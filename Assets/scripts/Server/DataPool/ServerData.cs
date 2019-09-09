using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace MyLib
{
    /// <summary>
    /// 服务端存储的用户数据 一个Json文件
    ///{"username":{}}
    /// </summary>
    public class ServerData
    {
        public static ServerData Instance = null;
        public PlayerInfo.Builder playerInfo;

        public PlayerInfo p2;

        public ServerData(){
            Instance = this;
        }
        public void LoadData(){
            Log.Sys("SavePath BackUp Data "+Application.persistentDataPath);
            int maxId = 3;
            //Test 0 1 2  file 
            for(int i = 0; i < maxId; i++) {
                string fpath = Path.Combine (Application.persistentDataPath, "server"+i+".json");
                var exist = File.Exists (fpath);
                FileStream fs = null;
                if (exist) {
                    fs = new FileStream (fpath, FileMode.Open);
                }

                if (fs == null) {
                    InitNewPlayerInfo();
                    break;
                }else {
                    byte[] buffer;
                    try {
                        long len = fs.Length;
                        buffer = new byte[len];
                        int count;
                        int sum = 0;
                        while ((count = fs.Read(buffer, sum, (int)(len-sum))) > 0) {
                            sum += count;
                        }
                    } finally {
                        fs.Close ();
                    }
                    try {
                        p2 = PlayerInfo.CreateBuilder().MergeFrom(buffer).Build();
                        playerInfo = PlayerInfo.CreateBuilder().MergeFrom(buffer);
                        Log.Net("InitPlayerInfo: "+p2);
                        break;
                    }catch(Exception ex){
                        WindowMng.windowMng.ShowNotifyLog("加载保存游戏数据出错 "+ex.Message);
                        //InitNewPlayerInfo();
                        if(i == maxId-1){
                            InitNewPlayerInfo();
                            break;
                        }
                    }
                }
            }

        }
        void InitNewPlayerInfo(){
            playerInfo = PlayerInfo.CreateBuilder();
            playerInfo.Exp = 0;
            playerInfo.Gold = 0;
            WindowMng.windowMng.ShowNotifyLog("初始化新的数据");
        }

        bool inSave = false;
        /// <summary>
        /// 保存玩家数据到磁盘上面
        /// 新旧文件内容会重叠在一起导致错误，需要先删除旧文件
        /// </summary>
        public void SaveUserData(){
            WindowMng.windowMng.ShowNotifyLog("正在保存数据");
            if(inSave) {
                return;
            }
            if(playerInfo == null) {
                return;
            }
            inSave = true;
            for(int i=1; i >= 0; i--){
                var nextFile = i+1;
                string fpath = Path.Combine (Application.persistentDataPath, "server"+i+".json");
                var exist = File.Exists (fpath);
                FileStream fs = null;
                if (exist) {
                    string fpath2 = Path.Combine (Application.persistentDataPath, "server"+nextFile+".json");
                    var nextExist = File.Exists(fpath2);
                    if(nextExist){
                        File.Delete(fpath2);
                    }

                    fs = new FileStream (fpath, FileMode.Open);

                    byte[] buffer;
                    try {
                        long len = fs.Length;
                        buffer = new byte[len];
                        int count;
                        int sum = 0;
                        while ((count = fs.Read(buffer, sum, (int)(len-sum))) > 0) {
                            sum += count;
                        }
                    } finally {
                        fs.Close ();
                    }

                    using (FileStream outfile = new FileStream(fpath2, FileMode.Create)) {
                        outfile.Write(buffer, 0, buffer.Length); 
                    }
                }
            }

            Log.Sys("SaveUserData");
            string fpath3 = Path.Combine (Application.persistentDataPath, "server"+0+".json");
            var exits3 = File.Exists(fpath3);
            if(exits3) {
                File.Delete(fpath3);
            }

            var msg = playerInfo.Build();
            using (FileStream outfile = new FileStream(fpath3, FileMode.Create)) {

                msg.WriteTo(outfile);
            }
            inSave = false;
        }



    }
}
