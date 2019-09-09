
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using Google.ProtocolBuffers;

namespace KBEngine
{
	//Response Packet Format
	public class Packet
	{
		public byte flag;
		public System.UInt32 msglen = 0;
		public uint flowId;
		public byte moduleId;
		public ushort msgid = 0;
		public uint responseTime;
		public short responseFlag;

		public IMessageLite protoBody;
        public byte[] data;


		public Packet(byte f, uint len, uint fid, byte module, ushort msg, uint restime, short resflag, IMessageLite pb) {
			//Debug.Log ("receive packet" );
			flag = f;
			msglen = len;
			flowId = fid;
			moduleId = module;
			msgid = msg;
			responseTime = restime;
			responseFlag = resflag;

			protoBody = pb;
			Debug.Log ("Packet:: readPacket "+fid);
			Debug.Log ("Packet:: readPacket " + protoBody.GetType ().FullName);
		}

		public Packet() {
		}
	}

	public class PacketHolder{
		public Packet packet;
	}

}
