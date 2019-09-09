
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
	
	using MessageID = System.UInt16;
	using MyLib;
	
    public class Message 
    {
        public static IMessageLite handlePB(byte moduleId, System.UInt16 msgId, MemoryStream msgstream) {
			//var buf = msgstream.getbuffer ();
			IMessageLite msg =  Util.GetMsg (moduleId, msgId, msgstream.getBytString());
			return msg;
		}
    }
} 
