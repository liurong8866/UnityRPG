using UnityEngine;
using System.Collections;
using Google.ProtocolBuffers;
using System.Collections.Generic;
using System;

namespace MyLib
{
	public class ServerBundle 
	{
        static System.UInt32 serverPushFlowId = 0;

		KBEngine.MemoryStream stream = new KBEngine.MemoryStream();
		public int messageLength = 0;
		public KBEngine.Message msgtype = null;
		public int moduleId;
		public int msgId;
		public System.UInt32 flowId;


		void newMessage(System.Type type) {
			Debug.Log ("ServerBundle:: 开始发送消息 Message is " + type.Name);
			var pa = Util.GetMsgID (type.Name);
            if(pa == null) {
                Debug.LogError("GetMessage Id Error, please Update NameMap.json "+type.Name);
            }
			moduleId = pa.moduleId;
			msgId = pa.messageId;
			
			msgtype = null;
		}

		uint writePB(byte[] v, int errorCode=0) {

			int bodyLength = 4 + 1 + 2+ 4 + 2 + v.Length;
			int totalLength = 1 + 4 + bodyLength;
			//checkStream (totalLength);
			Debug.Log ("ServerBundle::writePB pack data is "+bodyLength+" pb length "+v.Length+" totalLength "+totalLength);
			Debug.Log ("ServerBundle::writePB module Id msgId " + moduleId+" "+msgId);
			stream.writeUint8 (Convert.ToByte(0xcc));
			stream.writeUint32 (Convert.ToUInt32(bodyLength));
			stream.writeUint32 (Convert.ToUInt32(flowId));
			stream.writeUint8 (Convert.ToByte(moduleId));
			stream.writeUint16 (Convert.ToUInt16(msgId));
			stream.writeUint32 (Convert.ToUInt32 (123));//response time
			stream.writeUint16 (Convert.ToUInt16 (errorCode)); // no error reponse flag
			stream.writePB (v);
			
			return flowId;
		}

		uint writePB(IMessageLite pbMsg, int errorCode=0) {
			byte[] bytes;
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream()) {
				pbMsg.WriteTo (stream);
				bytes = stream.ToArray ();
			}
			return writePB (bytes, errorCode);
		}

        public static byte[] sendImmediateError(IBuilderLite build, uint flowId, int errorCode) {
            var data = build.WeakBuild ();

            var bundle = new ServerBundle ();
            bundle.newMessage (data.GetType());
            bundle.flowId = flowId;
            bundle.writePB (data, errorCode);

            return bundle.stream.getbuffer();
        }

		public static byte[] MakePacket(IBuilderLite build, uint flowId) {
			var data = build.WeakBuild ();

			var bundle = new ServerBundle ();
			bundle.newMessage (data.GetType());
			bundle.flowId = flowId;
			bundle.writePB (data);

			return bundle.stream.getbuffer();
		}
        /// <summary>
        /// Send Packet With ErrorCode
        /// </summary>
        /// <param name="build">Build.</param>
        /// <param name="flow">Flow.</param>
        /// <param name="errorCode">Error code.</param>
        public static void SendImmediateError(IBuilderLite build, uint flow, int errorCode) {
            DemoServer.demoServer.GetThread().SendPacket(build, flow, errorCode);
        }
        /// <summary>
        /// Response To Request 
        /// </summary>
        /// <param name="build">Build.</param>
        /// <param name="flow">Flow.</param>
        public static void SendImmediate(IBuilderLite build, uint flow) {
            DemoServer.demoServer.GetThread().SendPacket(build, flow);
        }
        public static void SendImmediatePush(IBuilderLite build) {
            DemoServer.demoServer.GetThread().SendPacket(build, serverPushFlowId++);
        }

	}

}