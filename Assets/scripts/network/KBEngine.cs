
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using MyLib;
using System.Reflection;

namespace KBEngine
{
  	using UnityEngine; 
	using System; 
	using System.Collections; 
	using System.Collections.Generic;
	using System.Text;
    using System.Threading;
	using System.Text.RegularExpressions;
	
	using MessageID = System.UInt16;
	using MessageLength = System.UInt32;

	public delegate void Callback();
    public class KBEThread
    {

        KBEngineApp app_;
		public bool over = false;
		
        public KBEThread(KBEngineApp app)
        {
            this.app_ = app;
        }

        public void run()
        {
			Dbg.INFO_MSG("KBEThread::run()");
			int count = 0;
START_RUN:
			over = false;

            try
            {
                this.app_.process();
                count = 0;
            }
            catch (Exception e)
            {
                Dbg.ERROR_MSG(e.ToString());
                Dbg.INFO_MSG("KBEThread::try run:" + count);
                
                count ++;
                if(count < 10)
                	goto START_RUN;
            }
			
			over = true;
			Dbg.INFO_MSG("KBEThread::end()");
        }
    }

	public class KBEngineApp : IMainLoop
	{
		public static KBEngineApp app = null;
		public float KBE_FLT_MAX = float.MaxValue;
		private NetworkInterface networkInterface_ = null;
		
        private Thread t_ = null;
        public KBEThread kbethread = null;
        
        public string username = "kbengine";
        public string password = "123456";
		public enum LoginType {
			UUID,
			ACCOUNT,
		}
		public LoginType loginType = LoginType.UUID;
        
        private static bool loadingLocalMessages_ = false;
        
		private static bool loginappMessageImported_ = false;
		private static bool baseappMessageImported_ = false;
		private static bool entitydefImported_ = false;
		private static bool isImportServerErrorsDescr_ = false;
		
		public string ip = "127.0.0.1";
		public UInt16 port = 20013;
		
		public static string url = "http://127.0.0.1";
		
		public string baseappIP = "";
		public UInt16 baseappPort = 0;
		
		public string currserver = "loginapp";
		public string currstate = "create";
		
		public string serverVersion = "";
		public string clientVersion = "0.1.13";
		public string serverScriptVersion = "";
		public string clientScriptVersion = "0.1.0";
		
		// Reference: http://www.kbengine.org/docs/programming/clientsdkprogramming.html, client types
		public sbyte clientType = 5;
		
		// Allow synchronization role position information to the server
		public bool syncPlayer = true;
		
		public UInt64 entity_uuid = 0;
		public Int32 entity_id = 0;
		public string entity_type = "";
		public Vector3 entityLastLocalPos = new Vector3(0f, 0f, 0f);
		public Vector3 entityLastLocalDir = new Vector3(0f, 0f, 0f);
		public Vector3 entityServerPos = new Vector3(0f, 0f, 0f);
		

		public struct ServerErr
		{
			public string name;
			public string descr;
			public UInt16 id;
		}
		
		public static Dictionary<UInt16, ServerErr> serverErrs = new Dictionary<UInt16, ServerErr>(); 
		
		private System.DateTime lastticktime_ = System.DateTime.Now;
		private System.DateTime lastUpdateToServerTime_ = System.DateTime.Now;
		
		public UInt32 spaceID = 0;
		public string spaceResPath = "";
		public bool isLoadedGeometry = false;
		
		
		public bool isbreak = false;
		public List<System.Action> pendingCallbacks = new List<System.Action>();

		//public ObjectManager objectManager;

		ClientApp client;
        public KBEngineApp(ClientApp c)
        {
			client = c;
			app = this;


        	networkInterface_ = new NetworkInterface(this);
            kbethread = new KBEThread(this);
            t_ = new Thread(new ThreadStart(kbethread.run));
            t_.Start();
            
			//networkPeer = new NetworkPeer ();
            // 注册事件
        }

	
        public void destroy()
        {
        	Dbg.WARNING_MSG("KBEngine::destroy()");
        	isbreak = true;
        	
        	int i = 0;
        	while(!kbethread.over && i < 50)
        	{
        		Thread.Sleep(1);
        		i += 1;
        	}
        	
			if(t_ != null)
        		t_.Abort();

        	t_ = null;
        	
        	reset();
        }
        
        public Thread t(){
        	return t_;
        }
        
        public NetworkInterface networkInterface(){
        	return networkInterface_;
        }
        


		public void reset()
		{
			
			currserver = "loginapp";
			currstate = "create";
			serverVersion = "";
			serverScriptVersion = "";
			
			entity_uuid = 0;
			entity_id = 0;
			entity_type = "";
			
			
			lastticktime_ = System.DateTime.Now;
			lastUpdateToServerTime_ = System.DateTime.Now;
			spaceID = 0;
			spaceResPath = "";
			isLoadedGeometry = false;
			
			networkInterface_.reset();
		}
		
		public void process()
		{
			while(!isbreak)
			{
				networkInterface_.process();
			}
			
			Dbg.WARNING_MSG("KBEngine::process(): break!");
		}

		/*
		 * Connect to login Server 
		 */ 
		public bool login_loginapp()
		{
			reset();
			if(!networkInterface_.connect(ip, port))
			{
				Dbg.ERROR_MSG(string.Format("KBEngine::login_loginapp(): connect {0}:{1} is error!", ip, port));  
				return false;
			}
			
			Dbg.DEBUG_MSG(string.Format("KBEngine::login_loginapp(): connect {0}:{1} is successfylly!", ip, port));
			return true;
		}
	
		public void queueInLoop(System.Action cb) {
			lock (this) {
				pendingCallbacks.Add(cb);
			}
		}

		/* Muduo Framework
		 * 
		 * Main Thread Update 
		 * 
		 */
		public void UpdateMain() {
			lock (this) {
				foreach(var cb in pendingCallbacks) {
					cb();
				}
				pendingCallbacks.Clear();
			}

		}




        		
	}
} 
