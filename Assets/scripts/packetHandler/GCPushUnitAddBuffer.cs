
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
/*
 * 增加Buff给角色
 */ 
namespace PacketHandler
{
	public class GCPushUnitAddBuffer : IPacketHandler
	{
		public override void HandlePacket(KBEngine.Packet packet) {
			if (packet.responseFlag == 0) {
				var player = MyLib.ObjectManager.objectManager.GetMyPlayer();
				var objCmd = new MyLib.ObjectCommand();
				objCmd.commandID = MyLib.ObjectCommand.ENUM_OBJECT_COMMAND.OC_UPDATE_IMPACT;
				objCmd.buffInfo = packet.protoBody as MyLib.GCPushUnitAddBuffer;
				player.GetComponent<MyLib.LogicCommand>().PushCommand(objCmd);
			}
		}
	}

}