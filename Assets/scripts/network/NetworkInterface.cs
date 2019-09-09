
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using Google.ProtocolBuffers;

namespace KBEngine
{
  	using UnityEngine; 
	using System; 
	using System.Net.Sockets; 
	using System.Net; 
	using System.Collections; 
	using System.Collections.Generic;
	using System.Text;
	using System.Threading;


	using MessageModuleID = System.SByte;
	using MessageID = System.UInt16;
	using MessageLength = System.UInt32;

	public delegate void MessageHandler(Packet msg);

    public class NetworkInterface 
    {
        private Socket socket_ = null;
		private MessageReader msgReader = new MessageReader();
		private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);
		private static byte[] _datas = new byte[MemoryStream.BUFFER_MAX];

		public Dictionary<uint, MessageHandler> flowHandlers = new Dictionary<uint, MessageHandler>();

        public NetworkInterface(KBEngineApp app)
        {
            msgReader.mainLoop = KBEngine.KBEngineApp.app;
        }
		
		public void reset()
		{
			if(valid())
				close();
			
			socket_ = null;
			msgReader = new MessageReader();
            msgReader.mainLoop = KBEngineApp.app;
			TimeoutObject.Set();
		}
		
		public Socket sock()
		{
			return socket_;
		}
		
		public bool valid()
		{
			return ((socket_ != null) && (socket_.Connected == true));
		}

		
		
		
		private static void connectCB(IAsyncResult asyncresult)
		{
			if(KBEngineApp.app.networkInterface().valid()) {
				KBEngineApp.app.networkInterface().sock().EndConnect(asyncresult);
            }else {
                MyLib.MyEventSystem.myEventSystem.PushEvent(MyLib.MyEvent.EventType.ReConnect);
            }
			
			TimeoutObject.Set();
		}
	    
		public bool connect(string ip, int port) 
		{
			int count = 0;
__RETRY:
			reset();
			TimeoutObject.Reset();
			
			socket_ = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
			socket_.SetSocketOption (System.Net.Sockets.SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, MemoryStream.BUFFER_MAX);
            try 
            { 
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port); 
                
				socket_.BeginConnect(endpoint, new AsyncCallback(connectCB), socket_);
				
		        if (TimeoutObject.WaitOne(10000))
		        {
                    if(valid()) {
                    }else {
                        MyLib.MyEventSystem.myEventSystem.PushEvent(MyLib.MyEvent.EventType.ReConnect);
                    }
		        }
		        else
		        {
		        	reset();
		        }
        
            } 
            catch (Exception e) 
            {
                Dbg.WARNING_MSG(e.ToString());
                
                if(count < 3)
                {
                	Dbg.WARNING_MSG("connect(" + ip + ":" + port + ") is error, try=" + (count++) + "!");
                	goto __RETRY;
           		 }
            
				return false;
            } 
			
			if(!valid())
			{
				return false;
			}
			
			return true;
		}
        
        public void close()
        {
            try{
            socket_.Shutdown(SocketShutdown.Both);
            }catch(Exception ex){
                Debug.LogError("Close Client Socket Error "+ex);
            }
            socket_.Close(0);
            socket_ = null;
        }


		void DumpHandler(Packet p) {
		}

		public void send(byte[] datas, MessageHandler handler, uint flowId) {
			if(socket_ == null || socket_.Connected == false) 
			{
				throw new ArgumentException ("invalid socket!");
			}
			
			if (datas == null || datas.Length == 0 ) 
			{
				throw new ArgumentException ("invalid datas!");
			}
			if (handler == null) {
				flowHandlers[flowId] = DumpHandler;
			} else {
				flowHandlers [flowId] = handler;
			}
			try
			{
				socket_.Send(datas);
			}
			catch (SocketException err)
			{
				if (err.ErrorCode == 10054 || err.ErrorCode == 10053)
				{
					Dbg.DEBUG_MSG(string.Format("NetworkInterface::send(): disable connect!"));
					
					if(socket_ != null && socket_.Connected)
						socket_.Close();
					
					socket_ = null;
				}
				else{
					Dbg.ERROR_MSG(string.Format("NetworkInterface::send(): socket error(" + err.ErrorCode + ")!"));
				}
			}
		}
		
		public void recv()
		{
           if(socket_ == null || socket_.Connected == false) 
			{
				throw new ArgumentException ("invalid socket!");
            }
			
            if (socket_.Poll(0, SelectMode.SelectRead))
            {
	           if(socket_ == null || socket_.Connected == false) 
				{
					Dbg.WARNING_MSG("invalid socket!");
					return;
	            }
				
				int successReceiveBytes = 0;
				
				try
				{
					successReceiveBytes = socket_.Receive(_datas, MemoryStream.BUFFER_MAX, 0);
				}
				catch (SocketException err)
				{
                    if (err.ErrorCode == 10054 || err.ErrorCode == 10053)
                    {
						Dbg.DEBUG_MSG(string.Format("NetworkInterface::recv(): disable connect!"));
						
						if(socket_ != null && socket_.Connected)
							socket_.Close();
						
						socket_ = null;
                    }
					else{
						Dbg.ERROR_MSG(string.Format("NetworkInterface::recv(): socket error(" + err.ErrorCode + ")!"));
					}
					
					return;
				}
				
				if(successReceiveBytes > 0)
				{
				//	Dbg.DEBUG_MSG(string.Format("NetworkInterface::recv(): size={0}!", successReceiveBytes));
				}
				else if(successReceiveBytes == 0)
				{
					Dbg.DEBUG_MSG(string.Format("NetworkInterface::recv(): disable connect!"));
					if(socket_ != null && socket_.Connected)
						socket_.Close();
					socket_ = null;
					
				}
				else
				{
					Dbg.ERROR_MSG(string.Format("NetworkInterface::recv(): socket error!"));
					
					if(socket_ != null && socket_.Connected)
						socket_.Close();
					socket_ = null;
					
					return;
				}
				Debug.Log("success received Data "+successReceiveBytes);
				msgReader.process(_datas, (MessageLength)successReceiveBytes, flowHandlers);
            }
		}
		
		public void process() 
		{
			if(socket_ != null && socket_.Connected)
			{
				recv();
			}
			else
			{
				System.Threading.Thread.Sleep(50);
			}
		}
	}
} 
