
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using MyLib;
using Google.ProtocolBuffers;

namespace KBEngine
{
    using UnityEngine; 
    using System; 
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    
    //using MessageID = System.UInt16;
    //using MessageLength = System.UInt16;
    using MessageID = System.UInt16;
    using MessageLength = System.UInt32;

    public class MessageReader
    {
        enum READ_STATE
        {
            READ_STATE_FLAG = 0,
            READ_STATE_MSGLEN = 1,
            READ_STATE_FLOWID = 2,
            READ_STATE_MODULEID = 3,
            READ_STATE_MSGID = 4,
            READ_STATE_RESPONSE_TIME = 5,
            READ_STATE_RESPONSE_FLAG =6,
            READ_STATE_BODY = 7,
        }
        
        private MessageID msgid = 0;
        private MessageLength msglen = 0;
        byte flag;
        uint flowId;
        byte moduleId;
        uint responseTime;
        short responseFlag;
        public MessageHandler msgHandle = null;
        public IMainLoop mainLoop;
        /*
         * Response Packet Format
         * 
         * 0xcc   byte
         * length int
         * flowId int
         * moduleId byte
         * messageId short
         * responseTime int
         * responseFlag byte
         * protobuffer 
         */ 
        private MessageLength expectSize = 1;
        private READ_STATE state = READ_STATE.READ_STATE_FLAG;
        private MemoryStream stream = new MemoryStream();
        
        public MessageReader()
        {
            expectSize = 1;
            state = READ_STATE.READ_STATE_FLAG;
        }

        public void process(byte[] datas, MessageLength length, Dictionary<uint, MessageHandler> flowHandler)
        {
            //Log.Net("process receive Data " + length + " state " + state);
            MessageLength totallen = 0;
            while (length > 0 && expectSize > 0)
            {
                if (state == READ_STATE.READ_STATE_FLAG)
                {
                    if (length >= expectSize)
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                        totallen += expectSize;
                        stream.wpos += (int)expectSize;
                        length -= expectSize;
                        
                        flag = stream.readUint8();
                        stream.clear();
                        
                        state = READ_STATE.READ_STATE_MSGLEN;
                        expectSize = 4;
                    } else
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                        stream.wpos += (int)length;
                        expectSize -= length;
                        break;
                    }
                } else if (state == READ_STATE.READ_STATE_MSGLEN)
                {
                    if (length >= expectSize)
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                        totallen += expectSize;
                        stream.wpos += (int)expectSize;
                        length -= expectSize;
                        
                        msglen = stream.readUint32();
                        stream.clear();
                        
                        state = READ_STATE.READ_STATE_FLOWID;
                        expectSize = 4;
                    } else
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                        stream.wpos += (int)length;
                        expectSize -= length;
                        break;
                    }
                } else if (state == READ_STATE.READ_STATE_FLOWID)
                {
                    if (length >= expectSize)
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                        totallen += expectSize;
                        stream.wpos += (int)expectSize;
                        length -= expectSize;
                        
                        flowId = stream.readUint32();
                        stream.clear();
                        
                        state = READ_STATE.READ_STATE_MODULEID;
                        expectSize = 1;
                    } else
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                        stream.wpos += (int)length;
                        expectSize -= length;
                        break;
                    }
                    
                } else if (state == READ_STATE.READ_STATE_MODULEID)
                {
                    if (length >= expectSize)
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                        totallen += expectSize;
                        stream.wpos += (int)expectSize;
                        length -= expectSize;
                        
                        moduleId = stream.readUint8();
                        stream.clear();
                        
                        state = READ_STATE.READ_STATE_MSGID;
                        expectSize = 2;
                    } else
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                        stream.wpos += (int)length;
                        expectSize -= length;
                        break;
                    }
                } else if (state == READ_STATE.READ_STATE_MSGID)
                {
                    if (length >= expectSize)
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                        totallen += expectSize;
                        stream.wpos += (int)expectSize;
                        length -= expectSize;
                        
                        msgid = stream.readUint16();
                        stream.clear();
                        
                        state = READ_STATE.READ_STATE_RESPONSE_TIME;
                        expectSize = 4;
                    } else
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                        stream.wpos += (int)length;
                        expectSize -= length;
                        break;
                    }
                } else if (state == READ_STATE.READ_STATE_RESPONSE_TIME)
                {
                    if (length >= expectSize)
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                        totallen += expectSize;
                        stream.wpos += (int)expectSize;
                        length -= expectSize;
                        
                        responseTime = stream.readUint32();
                        stream.clear();
                        
                        state = READ_STATE.READ_STATE_RESPONSE_FLAG;
                        expectSize = 2;
                    } else
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                        stream.wpos += (int)length;
                        expectSize -= length;
                        break;
                    }
                } else if (state == READ_STATE.READ_STATE_RESPONSE_FLAG)
                {
                    if (length >= expectSize)
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                        totallen += expectSize;
                        stream.wpos += (int)expectSize;
                        length -= expectSize;
                        
                        responseFlag = stream.readInt16();
                        stream.clear();
                        
                        state = READ_STATE.READ_STATE_BODY;
                        expectSize = msglen - 4 - 1 - 2 - 4 - 2;
                    } else
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                        stream.wpos += (int)length;
                        expectSize -= length;
                        break;
                    }
                }

                /*
                 * body Can be empty
                 */ 
                if (state == READ_STATE.READ_STATE_BODY)
                {
                    if (length >= expectSize)
                    {
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
                        MessageHandler handler = null;
                        if (flowHandler == null)
                        {
                            handler = msgHandle;
                        } else if (flowHandler.ContainsKey(flowId))
                        {
                            handler = flowHandler [flowId];
                            flowHandler.Remove(flowId);

                        }else {
                            handler = msgHandle;
                        }
                        IMessageLite pbmsg = KBEngine.Message.handlePB(moduleId, msgid, stream);
                        Packet p = new Packet(flag, msglen, flowId, moduleId, msgid, responseTime, responseFlag, pbmsg);
                        var fullName = pbmsg.GetType().FullName;

                        if (fullName.Contains("Push"))
                        {
                            //Log.Net("MessageReader Handler PushMessage");
                            if (mainLoop != null)
                            {
                                mainLoop.queueInLoop(delegate
                                {
                                    var handlerName = fullName.Replace("MyLib", "PacketHandler");
                                    var tp = Type.GetType(handlerName);
                                    if (tp == null)
                                    {
                                        Debug.LogError("PushMessage noHandler " + handlerName);
                                    } else
                                    {
                                        //Debug.Log("Handler Push Message here "+handlerName);
                                        var ph = (PacketHandler.IPacketHandler)Activator.CreateInstance(tp);
                                        ph.HandlePacket(p);
                                    }

                                });
                            }
                        } else if (handler != null)
                        {
                            mainLoop.queueInLoop(()=>{
                                handler(p);
                            });

                        } else
                        {
                            //flowHandler.Remove(flowId);
                            Debug.LogError("MessageReader::process No handler for flow Message " + msgid + " " + flowId + " " + pbmsg.GetType() + " " + pbmsg);
                        }



                        stream.clear();
                        
                        state = READ_STATE.READ_STATE_FLAG;
                        expectSize = 1;
                    } else
                    {
                        Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                        stream.wpos += (int)length;
                        expectSize -= length;
                        break;
                    } 
                }
                
            }

            if (responseFlag != 0)
            {
                Debug.LogError("MessageReader:: read Error Packet " + responseFlag);
            }

            //Log.Net("current state after " + state + " msglen " + msglen + " " + length);
            //Log.Net("MessageReader::  prop  flag" + flag + "  msglen " + msglen + " flowId " + flowId + " moduleId " + moduleId + " msgid " + msgid + " responseTime " + responseTime + " responseFlag " + responseFlag + " expectSize " + expectSize);
        }
        
    }
} 
