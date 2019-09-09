using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Google.ProtocolBuffers;
using SimpleJSON;
using System;

namespace MyLib
{
	public class MyCon
	{
		public Socket connect;
		public bool isClose = false;
		public bool isConnected = true;
		public List<byte[]> msgBuffer = new List<byte[]> ();

		public MyCon (Socket s)
		{
			connect = s;
		}

		public void Close ()
		{
			if (isClose) {
				return;
			}
			if (connect != null && connect.Connected && isConnected) {
				try {
					connect.Shutdown (SocketShutdown.Both);
				} catch (Exception ex) {
					Debug.LogError ("ShutDownSocket Error " + ex);
				}
			}
			if (connect != null) {
				connect.Close ();
			}
			connect = null;
			isClose = true;
			Debug.LogError ("CloseServer To Client Socket");
		}
	}

	public class ServerThread
	{
		int playerId = 100;
     
		int selectPlayerJob = 4;
		Socket socket;
		MyLib.ServerMsgReader msgReader = new ServerMsgReader ();
		System.Random random = new System.Random (1000);

		public void CloseServerSocket ()
		{
			socket.Close ();
			if (currentCon != null) {
				currentCon.Close ();
				currentCon = null;
			}
		}

		void InitListenSocket ()
		{
			Debug.LogError ("InitListenSocket " + random.Next ());
			socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.SetSocketOption (System.Net.Sockets.SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			IPEndPoint ip = new IPEndPoint (IPAddress.Parse ("127.0.0.1"), ClientApp.Instance.testPort);
			socket.Bind (ip);
			socket.Listen (1);
			//msgReader.Reset();
			msgReader = new ServerMsgReader ();
			msgReader.msgHandle = handleMsg;
		}

		public ServerThread ()
		{
			//InitListenSocket();
			//msgReader.msgHandle = handleMsg;
		}

		public void SendPacket (IBuilderLite retpb, uint flowId, int errorCode)
		{
			if (currentCon == null || currentCon.isClose) {
				return;
			}
			var bytes = ServerBundle.sendImmediateError (retpb, flowId, errorCode);
			Debug.Log ("DemoServer: Send Packet " + flowId);
			lock (currentCon.msgBuffer) {
				currentCon.msgBuffer.Add (bytes);
			}
		}

		public void SendPacket (IBuilderLite retpb, uint flowId)
		{
			if (currentCon == null || currentCon.isClose) {
				return;
			}
			var bytes = ServerBundle.MakePacket (retpb, flowId);
			Debug.Log ("DemoServer: Send Packet " + flowId);
			lock (currentCon.msgBuffer) {
				currentCon.msgBuffer.Add (bytes);
			}
		}

		void handleMsg (KBEngine.Packet packet)
		{
			var receivePkg = packet.protoBody.GetType ().FullName;
			Debug.Log ("Server Receive " + receivePkg);
			var className = receivePkg.Split (char.Parse (".")) [1];
			IBuilderLite retPb = null;
			uint flowId = packet.flowId;
			bool findHandler = false;

			if (className == "CGAutoRegisterAccount") {
				var au = GCAutoRegisterAccount.CreateBuilder ();
				au.Username = "liyong";
				retPb = au;
			} else if (className == "CGRegisterAccount") {
				var au = GCRegisterAccount.CreateBuilder ();
				retPb = au;
			} else if (className == "CGLoginAccount") {
				var au = GCLoginAccount.CreateBuilder ();
				var playerInfo = ServerData.Instance.playerInfo;
				if (playerInfo.HasRoles) {
					var role = RolesInfo.CreateBuilder ().MergeFrom (playerInfo.Roles);
					au.AddRolesInfos (role);
				}

				retPb = au;
			} else if (className == "CGSelectCharacter") {
				var inpb = packet.protoBody as CGSelectCharacter;
				if (inpb.PlayerId == 101) {
					selectPlayerJob = 4;
				} else if (inpb.PlayerId == 102) {
					selectPlayerJob = 2;
				} else {
					selectPlayerJob = 1;
				}
				var au = GCSelectCharacter.CreateBuilder ();
				au.TokenId = "12345";
				retPb = au;
			} else if (className == "CGBindingSession") {
				var au = GCBindingSession.CreateBuilder ();
				au.X = 22;
				au.Y = 1;
				au.Z = 17;
				au.Direction = 10;
				au.MapId = 0;
				au.DungeonBaseId = 0;
				au.DungeonId = 0;
				retPb = au;
			} else if (className == "CGEnterScene") {
				var inpb = packet.protoBody as CGEnterScene;
				var au = GCEnterScene.CreateBuilder ();
				au.Id = inpb.Id;
				retPb = au;
			} else if (className == "CGListBranchinges") {
				var au = GCListBranchinges.CreateBuilder ();
				var bran = Branching.CreateBuilder ();
				bran.Line = 1;
				bran.PlayerCount = 2;
				au.AddBranching (bran);
				retPb = au;
			} else if (className == "CGHeartBeat") {
            
			} 
            else if (className == "CGCopyInfo") {
				var pinfo = ServerData.Instance.playerInfo;
				if (pinfo.HasCopyInfos) {
					retPb = GCCopyInfo.CreateBuilder ().MergeFrom (pinfo.CopyInfos);
				} else {
					//First Fetch Login Info
					var au = GCCopyInfo.CreateBuilder ();
					var cin = CopyInfo.CreateBuilder ();
					cin.Id = 101;
					cin.IsPass = false;
					au.AddCopyInfo (cin);
					var msg = au.Build ();
					pinfo.CopyInfos = msg;
					retPb = GCCopyInfo.CreateBuilder ().MergeFrom (msg);
				}
			} 


            else if (className == "CGUserDressEquip") {
				PlayerData.UserDressEquip (packet);
				findHandler = true;
			} else if (className == "CGAutoRegisterAccount") {
				var au = GCAutoRegisterAccount.CreateBuilder ();
				au.Username = "liyong_" + random.Next ();
				retPb = au;
			} else if (className == "CGRegisterAccount") {
				var inpb = packet.protoBody as CGRegisterAccount;
				ServerData.Instance.playerInfo.Username = inpb.Username;

				var au = GCRegisterAccount.CreateBuilder ();
				retPb = au;
			} else if (className == "CGPlayerMove") {
				var au = GCPlayerMove.CreateBuilder ();
				retPb = au;
			} else {
				var fullName = packet.protoBody.GetType ().FullName;
                var handlerName = fullName.Replace ("MyLib", "ServerPacketHandler");

				var tp = Type.GetType (handlerName);
				if (tp == null) {
					if (ServerPacketHandler.HoldCode.staticTypeMap.ContainsKey (handlerName)) {
						tp = ServerPacketHandler.HoldCode.staticTypeMap [handlerName];
					}
				}

				if (tp == null) {
					Debug.LogError ("PushMessage noHandler " + handlerName);
				} else {
					findHandler = true;
					var ph = (ServerPacketHandler.IPacketHandler)Activator.CreateInstance (tp);
					ph.HandlePacket (packet);
				}
			}
        

			if (retPb != null) {
				SendPacket (retPb, flowId);
			} else {
				if (className != "CGHeartBeat" && !findHandler) {
					Debug.LogError ("DemoServer::not Handle Message " + className);
				}
			}
		}

		int maxId = 0;

		void SendThread (MyCon con)
		{
			int id = ++maxId;
			Debug.LogError ("SendThread Start " + id);
			while (!con.isClose && !DemoServer.demoServer.stop) {
				lock (con.msgBuffer) {
					while (!con.isClose && con.msgBuffer.Count > 0 && !DemoServer.demoServer.stop) {
						int sent = 0;
						int size = con.msgBuffer [0].Length;
						while (!con.isClose && sent < size) {
							try {
								sent += con.connect.Send (con.msgBuffer [0], sent, size - sent, SocketFlags.None);
							} catch (SocketException ex) {

								if (ex.SocketErrorCode == SocketError.WouldBlock ||
								                            ex.SocketErrorCode == SocketError.IOPending ||
								                            ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable) {
									//Thread.Sleep(100);
									Debug.LogError ("SendError Socket");
									break;
								} else {//SendThread Serious Error
									Debug.LogError ("Critical Server Error " + ex.SocketErrorCode);
									con.Close ();
									//DemoServer.demoServer.ShutDown();
									break;
								}
							}
						}
						con.msgBuffer.RemoveAt (0);
					}
				}
				Thread.Sleep (100);
			}

			Debug.LogError ("Send Finish " + id);
		}

		MyCon currentCon = null;
		//Connection Lost Then Restart New Accept
		public void run ()
		{
			InitListenSocket ();
			Debug.LogError ("Start Demo Server");
			byte[] buffer = new byte[1024];
			while (!DemoServer.demoServer.stop) {
				Debug.LogError ("Start Accept Socket");
				Socket connect = null;
				msgReader = new ServerMsgReader ();
				msgReader.msgHandle = handleMsg;
				while (!DemoServer.demoServer.stop) {
					try {
						Debug.LogError ("Server Start Accept " + random.Next ());
						connect = socket.Accept ();
						break;
					} catch (Exception ex) {
						Debug.LogError ("ServerSocket AcceptError " + ex);
						//socket.Shutdown(SocketShutdown.Both);
						try {
							socket.Close ();
						} catch (Exception ex1) {
							Debug.LogError ("Server Socket Close Error " + ex1);
						}
						Thread.Sleep (1);
						if (!DemoServer.demoServer.stop) {
							InitListenSocket ();
						}
                        
					}

				}
               
				var con = new MyCon (connect);
				if (!DemoServer.demoServer.stop) {
					if (currentCon != null) {
						lock (currentCon.msgBuffer) {
							currentCon.msgBuffer.Clear ();
						}
					}

					currentCon = con;
					var sendThread = new Thread (new ThreadStart (delegate() {
						SendThread (con);
					}));

					sendThread.Start ();
				}

				while (!con.isClose && !DemoServer.demoServer.stop) {
					int num = 0;
					try {
						num = connect.Receive (buffer);
					} catch (SocketException ex) {
						if (ex.SocketErrorCode == SocketError.WouldBlock ||
						                      ex.SocketErrorCode == SocketError.IOPending ||
						                      ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable) {
							Debug.LogError ("ReceiveSocket Error " + ex.SocketErrorCode);
							Thread.Sleep (100);

						} else {
							Debug.LogError ("Critical Server ReceiveError " + ex.SocketErrorCode);
							con.Close ();
							break;
						}
					}
					if (num > 0) {
						msgReader.process (buffer, (uint)num);
					} else {
						Debug.LogError ("Receive Buffer Length " + num);
						con.isConnected = false;
						con.Close ();
						currentCon = null;
					}
				}
				Debug.LogError ("Socket Connect Stop");
			}

			Debug.LogError ("DemoServer Stop");
			socket.Close ();
		}
	}

	public class DemoServer
	{
		public static DemoServer demoServer;
		public bool stop = false;
		ServerData serverData;
		ServerThread sth;

		public ServerThread GetThread ()
		{
			return sth;
		}

		public ServerData GetServerData ()
		{
			return serverData;
		}

		public DemoServer ()
		{
			ServerPacketHandler.HoldCode.Init();

			demoServer = this;
			serverData = new ServerData ();
			serverData.LoadData ();

			Debug.Log ("Init DemoServer");
			sth = new ServerThread ();
			var t = new Thread (new ThreadStart (sth.run));
			t.Start ();

		}

		public void ShutDown ()
		{
			Debug.LogError ("ShutDown Server");
			//serverData.SaveUserData();
			stop = true;

		}
	}
}
