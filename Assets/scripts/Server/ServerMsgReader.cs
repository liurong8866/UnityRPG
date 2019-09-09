using UnityEngine;
using System.Collections;
using System;
using Google.ProtocolBuffers;

namespace MyLib {
	using MessageID = System.UInt16;
	using MessageLength = System.UInt32;


	public class ServerMsgReader {
		public delegate void MessageHandler(KBEngine.Packet msg);
		enum READ_STATE
		{
			READ_STATE_FLAG = 0,
			READ_STATE_MSGLEN = 1,
			READ_STATE_FLOWID = 2,
			READ_STATE_MODULEID = 3,
			READ_STATE_MSGID = 4,
			READ_STATE_BODY = 7,
		}


		private System.UInt16 msgid = 0;
		private System.UInt32 msglen = 0;

		byte flag;
		uint flowId;
		byte moduleId;

		public MessageHandler msgHandle = null;

		private System.UInt32 expectSize = 1;
		private READ_STATE state = READ_STATE.READ_STATE_FLAG;
		private KBEngine.MemoryStream stream = new KBEngine.MemoryStream();

		public ServerMsgReader()
		{
			expectSize = 1;
			state = READ_STATE.READ_STATE_FLAG;
		}
		
        public void Reset(){
            Debug.LogError("Reset Server MsgReader");
            expectSize = 1;
            state = READ_STATE.READ_STATE_FLAG;
        }

		public void process(byte[] datas, MessageLength length) {
			Debug.Log ("process receive Data " + length+" state "+state);
			MessageLength totallen = 0;
			while (length > 0 && expectSize > 0) {
				if(state == READ_STATE.READ_STATE_FLAG) {//1
					if(length >= expectSize) {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
						totallen += expectSize;
						stream.wpos += (int)expectSize;
						length -= expectSize;
						
						flag = stream.readUint8();
						stream.clear();
						
						state = READ_STATE.READ_STATE_MSGLEN;
						expectSize = 4;
					}else {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
						stream.wpos += (int)length;
						expectSize -= length;
						break;
					}
				}else if(state == READ_STATE.READ_STATE_MSGLEN) {//4
					if(length >= expectSize) {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
						totallen += expectSize;
						stream.wpos += (int)expectSize;
						length -= expectSize;
						
						msglen = stream.readUint32();
						stream.clear();
						
						state = READ_STATE.READ_STATE_FLOWID;
						expectSize = 4;
					}else {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
						stream.wpos += (int)length;
						expectSize -= length;
						break;
					}
				}else if(state == READ_STATE.READ_STATE_FLOWID) {//4
					if(length >= expectSize) {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
						totallen += expectSize;
						stream.wpos += (int)expectSize;
						length -= expectSize;
						
						flowId = stream.readUint32();
						stream.clear();
						
						state = READ_STATE.READ_STATE_MODULEID;
						expectSize = 1;
					}else {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
						stream.wpos += (int)length;
						expectSize -= length;
						break;
					}
					
				}else if(state == READ_STATE.READ_STATE_MODULEID) {//1
					if(length >= expectSize) {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
						totallen += expectSize;
						stream.wpos += (int)expectSize;
						length -= expectSize;
						
						moduleId = stream.readUint8();
						stream.clear();
						
						state = READ_STATE.READ_STATE_MSGID;
						expectSize = 2;
					}else {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
						stream.wpos += (int)length;
						expectSize -= length;
						break;
					}
				}else if(state == READ_STATE.READ_STATE_MSGID) {//2
					if(length >= expectSize) {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
						totallen += expectSize;
						stream.wpos += (int)expectSize;
						length -= expectSize;
						
						msgid = stream.readUint16();
						stream.clear();
						
						state = READ_STATE.READ_STATE_BODY;
						expectSize = msglen-4-1-2;
					}else {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
						stream.wpos += (int)length;
						expectSize -= length;
						break;
					}
				}

				/*
				 * body Can be empty handle body immediately
				 */ 
				if(state == READ_STATE.READ_STATE_BODY) {
					if(length >= expectSize) {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
						totallen += expectSize;
						stream.wpos += (int)expectSize;
						length -= expectSize;
						/*
						 * No Handler Or PushMessage  forward To IPacketHandler 
						 * Call Who's RPC Method Or Register Many RPC Method to Handle It ?
						 * [PushHandler]
						 * void GCPushSpriteInfo(Packet packet) {
						 * }
						 * 
						 * PacketHandler namespace
						 * IPacketHandler---->GCPushSpriteInfo
						 */ 
						MyLib.ServerMsgReader.MessageHandler handler = msgHandle;

						//KBEngine.Message msg = new KBEngine.Message();
                        IMessageLite pbmsg = KBEngine.Message.handlePB(moduleId, msgid, stream);
						KBEngine.Packet p = new KBEngine.Packet (flag, msglen, flowId, moduleId, msgid, 0, 0, pbmsg);
						//var fullName = pbmsg.GetType().FullName;

                        KBEngine.KBEngineApp.app.queueInLoop(delegate() {
                            handler(p);
                        });

						stream.clear();
						
						state = READ_STATE.READ_STATE_FLAG;
						expectSize = 1;
					}else {
						Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
						stream.wpos += (int)length;
						expectSize -= length;
						break;
					} 
				}
				
			}

			
			Debug.Log("server state after "+state+" msglen "+msglen+" "+length);
			Debug.Log(" server MessageReader::  prop  flag" + flag + "  msglen " + msglen + " flowId " + flowId + " moduleId " + moduleId + " msgid " + msgid + " responseTime " + 0 + " responseFlag " + 0 + " expectSize " + expectSize);
		}
	}

}