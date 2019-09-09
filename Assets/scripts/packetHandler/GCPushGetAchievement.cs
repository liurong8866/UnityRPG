
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace PacketHandler
{
	public class GCPushGetAchievement : IPacketHandler
	{
		#region implemented abstract members of IPacketHandler

		public override void HandlePacket (KBEngine.Packet packet)
		{
			throw new System.NotImplementedException ();
		}

		#endregion



	}
	public class GCPushLoseAchievement : IPacketHandler {
		#region implemented abstract members of IPacketHandler
		
		public override void HandlePacket (KBEngine.Packet packet)
		{
			throw new System.NotImplementedException ();
		}
		
		#endregion
	}
}
