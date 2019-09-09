using UnityEngine;
using System.Collections;

namespace ServerPacketHandler {
    public abstract class IPacketHandler
    {
        public IPacketHandler ()
        {

        }
        public abstract void HandlePacket (KBEngine.Packet packet);

    }
}
