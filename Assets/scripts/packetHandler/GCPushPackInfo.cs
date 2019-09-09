using UnityEngine;
using System.Collections;

namespace PacketHandler
{
	public class GCPushPackInfo : IPacketHandler
	{
		public override void HandlePacket(KBEngine.Packet packet) {
			if (packet.responseFlag == 0) {
				var data = packet.protoBody as MyLib.GCPushPackInfo;
				//更新背包数据 以及 UI中的图标数据 
				MyLib.BackPack.backpack.SetItemInfo(data);

				MyLib.MyEventSystem.myEventSystem.PushEvent (MyLib.MyEvent.EventType.UpdateItemCoffer);
				//ChuMeng.MyEventSystem.myEventSystem.PushEvent(ChuMeng.MyEvent.EventType.UpdateSuperTip);
			}
		}
	}
}
