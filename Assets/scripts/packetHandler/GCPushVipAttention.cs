
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace PacketHandler
{
	public class GCPushVipAttention : IPacketHandler
	{
		public override void HandlePacket(KBEngine.Packet packet) {
			if (packet.responseFlag == 0) {
				//ChuMeng.VipController.vipController.VipAttention(packet.protoBody as ChuMeng.GCPushVipAttention);
			}
		}
	}
}
