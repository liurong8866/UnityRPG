
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace PacketHandler
{
	public class GCPushChat2Client : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			Log.Net ("handle dy");
			if (packet.responseFlag == 0) {
				var data = packet.protoBody as MyLib.GCPushChat2Client;
				MyLib.Talk.talk.HandleRecvTalkPacket(data);
			}

		}
	}

}