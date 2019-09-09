
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
	using System.Collections;
	using System.Collections.Generic;
	using MyLib;

    public class Bundle 
    {
		public static List<string> sendMsg = new List<string>();
		public static List<string> recvMsg = new List<string> ();

		public static uint flowId = 1;

    	public MemoryStream stream = new MemoryStream();
		public List<MemoryStream> streamList = new List<MemoryStream>();
		public int numMessage = 0;
		public int messageLength = 0;

		public int moduleId;
		public int msgId;
		public Bundle()
		{
		}

		public void newMessage(System.Type type) {
			fini (false);

			sendMsg.Add("Bundle:: 开始发送消息 Message is " + type.Name+" "+flowId);
			if (sendMsg.Count > 30) {
				sendMsg.RemoveRange(0, sendMsg.Count-30);
			}

            Log.Net ("Bundle:: 开始发送消息 Message is " + type.Name);

			var pa = Util.GetMsgID (type.Name);
			moduleId = pa.moduleId;
			msgId = pa.messageId;
			
			numMessage += 1;
		}

		
		
		public void fini(bool issend)
		{
			if(numMessage > 0)
			{
				if(stream != null)
					streamList.Add(stream);
			}

			/*
			 * Not Send Message just swap stream
			 * Send Message clear numMessage
			 */

			if(issend)
			{
				numMessage = 0;
			}
		}

		public IEnumerator sendCoroutine(NetworkInterface networkInterface, uint fId, PacketHolder par) {
			fini (true);
			Debug.Log ("message Number " + streamList.Count);
			bool resp = false;
			if(networkInterface.valid())
			{
				for(int i=0; i<streamList.Count; i++)
				{
					stream = streamList[i];
					networkInterface.send(stream.getbuffer(), delegate(Packet p) {
						par.packet = p;
						resp = true;
					}, fId);
                    Log.Sys("SendNetworkSuc");
				}
			}
			else
			{
				Dbg.ERROR_MSG("Bundle::send: networkInterface invalid!");  
			}
			
			streamList.Clear();
			stream = new MemoryStream();

			while (!resp) {
				yield return null;
			}
		}

		void sendImm(NetworkInterface networkInterface, uint fId) {
			fini (true);
			Debug.Log ("message Number " + streamList.Count);
			//bool resp = false;
			if(networkInterface.valid())
			{
				for(int i=0; i<streamList.Count; i++)
				{
					stream = streamList[i];
					networkInterface.send(stream.getbuffer(), delegate(Packet p) {
						//par.packet = p;
						//resp = true;
					}, fId);
				}
			}
			else
			{
				Dbg.ERROR_MSG("Bundle::send: networkInterface invalid!");  
			}
			
			streamList.Clear();
			stream = new MemoryStream();

		}

		public static IEnumerator sendSimple(UnityEngine.MonoBehaviour mo, IBuilderLite build, PacketHolder par) {
		
			var bundle = new Bundle ();
			var data = build.WeakBuild ();
			Log.Net ("send Simple "+bundle+" "+data);
			/*
			 * After Build  builder Clear
			 */ 

			bundle.newMessage (data.GetType ());
			var fid = bundle.writePB (data);
			//Log.Net ("Perhaps network not ok");
			yield return mo.StartCoroutine(bundle.sendCoroutine(KBEngine.KBEngineApp.app.networkInterface(), fid, par));
			
		}

        public static Packet GetPacketFid(IBuilderLite build) {
            var p = new Packet();

            var bundle = new Bundle();
            var data = build.WeakBuild();
            Log.Net("GetPacket: "+data);
            bundle.newMessage(data.GetType());
            var fid = bundle.writePB(data);
            var buff = bundle.stream.getbuffer();
            p.flowId = fid;
            p.data = buff;
            return p;
        }

        private static uint lastFid;
        public static byte[] GetPacket(IBuilderLite build) {
            var bundle = new Bundle();
            var data = build.WeakBuild();
            Log.Net("GetPacket: "+data);
            bundle.newMessage(data.GetType());
            var fid = bundle.writePB(data);
            lastFid = fid;
            return bundle.stream.getbuffer();
        }
        public struct SendPacketInfo {
            public uint fid;
            public byte[] data;
        }
        public static SendPacketInfo GetPacketFull(IBuilderLite build) {
            var data = GetPacket(build);
            var t = new SendPacketInfo() {
                fid = lastFid,
                data = data,
            };
            return t;
        }

		public static IMessageLite sendImmediate(IBuilderLite build) {
			var bundle = new Bundle ();
			var data = build.WeakBuild ();
			bundle.newMessage (data.GetType ());
			var fid = bundle.writePB (data);
			bundle.sendImm(KBEngine.KBEngineApp.app.networkInterface(), fid);
			return data;
		}
		
		public void checkStream(int v)
		{
			if(v > stream.fillfree())
			{
				streamList.Add(stream);
				stream = new MemoryStream();
			}
	
			messageLength += v;
		}
		
		//---------------------------------------------------------------------------------
		public void writeInt8(SByte v)
		{
			checkStream(1);
			stream.writeInt8(v);
		}
	
		public void writeInt16(Int16 v)
		{
			checkStream(2);
			stream.writeInt16(v);
		}
			
		public void writeInt32(Int32 v)
		{
			checkStream(4);
			stream.writeInt32(v);
		}
	
		public void writeInt64(Int64 v)
		{
			checkStream(8);
			stream.writeInt64(v);
		}
		
		public void writeUint8(Byte v)
		{
			checkStream(1);
			stream.writeUint8(v);
		}
	
		public void writeUint16(UInt16 v)
		{
			checkStream(2);
			stream.writeUint16(v);
		}
			
		public void writeUint32(UInt32 v)
		{
			checkStream(4);
			stream.writeUint32(v);
		}
	
		public void writeUint64(UInt64 v)
		{
			checkStream(8);
			stream.writeUint64(v);
		}
		
		public void writeFloat(float v)
		{
			checkStream(4);
			stream.writeFloat(v);
		}
	
		public void writeDouble(double v)
		{
			checkStream(8);
			stream.writeDouble(v);
		}
		
		public void writeString(string v)
		{
			checkStream(v.Length + 1);
			stream.writeString(v);
		}
		
		public void writeBlob(byte[] v)
		{
			checkStream(v.Length + 4);
			stream.writeBlob(v);
		}

		/*
		 * 0xcc   int8
		 * length int32
		 * flowId int32
		 * moduleId int8
		 * messageId int16
		 * protobuffer
		 */ 
		public uint writePB(byte[] v) {
			uint fid = flowId++;
            if(fid == 0){
                fid++;
            }

			int bodyLength = 4 + 1 + 2 + v.Length;
			int totalLength = 1 + 4 + bodyLength;
			//checkStream (totalLength);
			Debug.Log ("Bundle::writePB pack data is "+bodyLength+" pb length "+v.Length+" totalLength "+totalLength);
			Debug.Log ("Bundle::writePB module Id msgId " + moduleId+" "+msgId);
			stream.writeUint8 (Convert.ToByte(0xcc));
			stream.writeUint32 (Convert.ToUInt32(bodyLength));
			stream.writeUint32 (Convert.ToUInt32(fid));
			stream.writeUint8 (Convert.ToByte(moduleId));
			stream.writeUint16 (Convert.ToUInt16(msgId));
			stream.writePB (v);

			return fid;
		}

		public uint writePB(IMessageLite pbMsg) {
			byte[] bytes;
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream()) {
				pbMsg.WriteTo (stream);
				bytes = stream.ToArray ();
			}
			return writePB (bytes);
		}

    }
} 
