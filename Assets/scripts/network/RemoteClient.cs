using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Collections.Generic;

namespace MyLib
{
    public class MsgBuffer
    {
        public int position = 0;
        public byte[] buffer;

        public int Size
        {
            get
            {
                return buffer.Length - position;
            }
        }
    }

    public enum RemoteClientEvent
    {
        None,
        Connected,
        Close,
    }

    public class RemoteClient
    {
        byte[] mTemp = new byte[8192];
        KBEngine.MessageReader msgReader = new KBEngine.MessageReader();

        Socket mSocket;
        IPEndPoint endPoint;
        public bool IsClose = false;
        List<MsgBuffer> msgBuffer = new List<MsgBuffer>();
        public KBEngine.MessageHandler msgHandler;
        public System.Action<RemoteClientEvent> evtHandler;

        private Dictionary<uint, KBEngine.MessageHandler> flowHandler = new Dictionary<uint, KBEngine.MessageHandler>();

        public RemoteClient(IMainLoop loop)
        {
            msgReader.msgHandle = HandleMsg;
            msgReader.mainLoop = loop; 
        }

        /// <summary>
        /// 当消息处理器已经退出场景则关闭网络连接 
        /// </summary>
        /// <param name="packet">Packet.</param>
        void HandleMsg(KBEngine.Packet packet)
        {
            //Debug.LogError("HandlerMsg "+packet.protoBody);
            Log.Net("HandlerMsg " + packet.protoBody);

            if (msgHandler != null)
            {
                msgHandler(packet);
            } else
            {
                Close();
            }
        }

        public void Connect(string ip1, int port1)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(ip1), port1);
            try
            {
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //mSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);//NoBuff JustSend

                var result = mSocket.BeginConnect(endPoint, new AsyncCallback(OnConnectResult), null);
                var th = new Thread(CancelConnect);
                th.Start(result);
            } catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                Close();
            }
        }

        public void StartReceive()
        {
            Debug.LogError("StartReceive");
            try
            {
                mSocket.BeginReceive(mTemp, 0, mTemp.Length, SocketFlags.None, OnReceive, null);
            } catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
                Close();
            }
        }

        void OnReceive(IAsyncResult result)
        {
            int bytes = 0;
            try
            {
                if (mSocket == null || !mSocket.Connected)
                {
                } else
                {
                    bytes = mSocket.EndReceive(result);
                }
            } catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
                Close();
            }
            //Debug.LogError("OnReceive "+bytes);
            //Log.Net("OnReceive: "+bytes);
            if (bytes <= 0)
            {
                Close();
            } else
            {
                uint num = (uint)bytes;
                msgReader.process(mTemp, num, flowHandler);
                try
                {
                    mSocket.BeginReceive(mTemp, 0, mTemp.Length, SocketFlags.None, OnReceive, null);
                } catch (Exception exception)
                {
                    Debug.LogError(exception.ToString());
                    Close();
                }
            }
        }

        void OnConnectResult(IAsyncResult result)
        {
            if (mSocket == null)
            {
                return;
            }
            bool success = false;
            try
            {
                mSocket.EndConnect(result);
                success = true;

            } catch (Exception exception)
            {
                mSocket.Close();
                mSocket = null;
                Debug.LogError(exception.Message);
                success = false;
            }
            if (success)
            {
                Debug.LogError("Connect Success");

                SendEvt(RemoteClientEvent.Connected);
            } else
            {
                Close();
            }
        }

        //事件的Evt处理机制已经删除掉了
        void SendEvt(RemoteClientEvent evt)
        {
            Debug.LogError("SendEvt: " + evt);
            if (evtHandler != null)
            {
                var eh = evtHandler;
                msgReader.mainLoop.queueInLoop(() =>
                {
                    eh(evt);
                });
            } else
            {
                Close();
            }
        }

        public void Disconnect()
        {
            Close();
        }

        void Close()
        {
            if (IsClose)
            {
                return;
            }
            Debug.LogError("CloseRemoteClient");
            if (mSocket != null)
            {
                try
                {
                    if (mSocket != null && mSocket.Connected)
                    {
                        mSocket.Shutdown(SocketShutdown.Both);
                    }
                    mSocket.Close();
                } catch (Exception exception)
                {
                    Debug.LogError(exception.ToString());
                }
            }
            mSocket = null;
            IsClose = true;

            if (evtHandler != null)
            {
                SendEvt(RemoteClientEvent.Close);
            }
            evtHandler = null;
            msgHandler = null;
        }

        void CancelConnect(object obj)
        {
            var res = (IAsyncResult)obj;
            if (res != null && !res.AsyncWaitHandle.WaitOne(3000))
            {
                Debug.LogError("ConnectError");
                Close();
            } else
            {
                StartReceive();
            }
        }

        private void SendTimeOut(object data)
        {
            try
            {
                IAsyncResult ret = data as IAsyncResult;
                if (!ret.AsyncWaitHandle.WaitOne(3000))
                {
                    Debug.LogError("SendTimeOut");
                    Close();
                }
            } catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public IEnumerator SendWaitResponse(byte[] data, uint fid, KBEngine.MessageHandler handler) {
            var ret = false;
            flowHandler.Add(fid, (packet)=>{
                handler(packet);
                ret = true;
            });
            Send(data);
            while(!ret) {
                yield return null;
            }
        }

        public void Send(byte[] data)
        {
            lock (msgBuffer)
            {
                var mb = new MsgBuffer(){ position = 0, buffer = data };
                msgBuffer.Add(mb);
                if (msgBuffer.Count == 1)
                {
                    try
                    {
                        var asyncRet = mSocket.BeginSend(mb.buffer, mb.position, mb.Size, SocketFlags.None, OnSend, null);
                        ThreadPool.QueueUserWorkItem(SendTimeOut, asyncRet);
                    } catch (Exception exception)
                    {
                        Debug.LogError(exception.ToString());
                        Close();
                    }
                }
            }
        }

        void OnSend(IAsyncResult result)
        {
            int num = 0;
            try
            {
                num = mSocket.EndSend(result);
            } catch (Exception exception)
            {
                num = 0;
                Close();
                Debug.LogError(exception.ToString());
                return;
            }
            lock (msgBuffer)
            {
                if (mSocket != null && mSocket.Connected)
                {
                    var mb = msgBuffer [0];
                    MsgBuffer nextBuffer = null;
                    if (mb.Size <= num)
                    {
                        msgBuffer.RemoveAt(0);
                        if (msgBuffer.Count > 0)
                        {
                            nextBuffer = msgBuffer [0];
                        }
                    } else if (mb.Size > num)
                    {
                        mb.position += num;
                        nextBuffer = mb;
                    }
                    if (nextBuffer != null)
                    {
                        try
                        {
                            mSocket.BeginSend(nextBuffer.buffer, nextBuffer.position, nextBuffer.Size, SocketFlags.None, OnSend, null);
                        } catch (Exception exception)
                        {
                            Debug.LogError(exception.ToString());
                            Close();
                        }
                    }
                }
            }
        }



    }

}