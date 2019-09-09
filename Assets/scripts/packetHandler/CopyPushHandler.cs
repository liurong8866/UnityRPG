using UnityEngine;
using System.Collections;

namespace PacketHandler
{
    public class GCPushLevelOpen : IPacketHandler {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            MyLib.CopyController.copyController.OpenLev(packet.protoBody as MyLib.GCPushLevelOpen);
        }
    }
}
