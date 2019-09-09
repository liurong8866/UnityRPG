using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Generic;

namespace MyLib
{
    public enum UDPEvent
    {
        None,
        Connected,
        Close,
    }

    public class UDPRemoteClient
    {
        UdpClient client;
        IPEndPoint endPoint;

        byte[] mTemp = new byte[8192];
        private int myId;
        Dictionary<uint, System.Action<KBEngine.Packet>> flowHandlers = new Dictionary<uint, Action<KBEngine.Packet>>();
        public bool IsClose = false;

        public UDPRemoteClient(IMainLoop loop)
        {

        
        }

        void HandleMsg(KBEngine.Packet packet)
        {
        }

        public void ConnectSuccess(uint fid)
        {
        
        }

        public IEnumerator Connect(string ip1, int port1, int userId)
        {
            client = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse(ip1), port1);
            try
            {
                client.Connect(endPoint);
            } catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            myId = userId;

            while (true)
            {
                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "UDP";
                var ainfo = AvatarInfo.CreateBuilder();
                ainfo.Id = userId;
                cg.AvatarInfo = ainfo.Build();
                var packet = KBEngine.Bundle.GetPacketFid(cg);
                Send(packet.data);
                var s = WorldManager.worldManager.GetActive();
                var ret = new bool[]{ false };
                yield return s.StartCoroutine(WaitTimeOut(packet.flowId, ret));
                if (ret [0])
                {
                } else
                {
                }
            }
        }

        IEnumerator  WaitTimeOut(uint fid, bool[] ret)
        {
            /*
            flowHandlers.Add(fid, ()=>{
                ret[0] = true;
            });
                */
            yield return null;
            /*
            var passTime = 0.0f;
            while(passTime < 3 && !ret[0]) {
                passTime += Time.deltaTime;
                yield return null;
            }
                */
        }

        //通过TCP 确认连接成功
        //超时失败则重试
        public void Send(byte[] data)
        {
            try
            {
                var asyncRet = client.BeginSend(data, data.Length, OnSend, client);
            } catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }


        void OnSend(IAsyncResult result)
        {
            var num = client.EndSend(result);
            Log.Net("SendUDP: " + num);
        }

        void Close()
        {
        }
    }
}