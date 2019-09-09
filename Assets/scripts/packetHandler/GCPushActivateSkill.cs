
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

/*
 * 例如 升到一定等级解锁新技能
 */ 
namespace PacketHandler
{
	public class GCPushActivateSkill : IPacketHandler
	{
		public override void HandlePacket(KBEngine.Packet packet) {
			if (packet.responseFlag == 0) {
				MyLib.SkillDataController.skillDataController.ActivateSkill(packet.protoBody as MyLib.GCPushActivateSkill);
			}
		}
	}

}