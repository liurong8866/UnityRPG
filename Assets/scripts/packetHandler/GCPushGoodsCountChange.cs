using UnityEngine;
using System.Collections;

namespace PacketHandler
{
    /// <summary>
    /// 金币数量变化
    /// </summary>
	public class GCPushGoodsCountChange : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			var pushGoods = packet.protoBody as MyLib.GCPushGoodsCountChange;
			foreach (MyLib.GoodsCountChange gc in pushGoods.GoodsCountChangeList) {
                MyLib.BackPack.backpack.UpdateGoodsCount(gc);
			}
			MyLib.MyEventSystem.myEventSystem.PushEvent (MyLib.MyEvent.EventType.UpdateItemCoffer);
		}
	}

}