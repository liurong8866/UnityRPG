/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace PacketHandler
{
	
	/*
	 * Handler Push Packet 
	 */ 
	public abstract class IPacketHandler
	{
		/*
		public static IPacketHandler DefaultInstance {
			get {
				Debug.LogError("Not Implement DefaultInstance: "+);
				return null;
			}
		}
		*/

		public IPacketHandler ()
		{

		}
		public abstract void HandlePacket (KBEngine.Packet packet);

	}

}