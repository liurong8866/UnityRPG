using UnityEngine;
using System.Collections;

namespace PacketHandler
{
    public class GCPushSkillPoint  : IPacketHandler 
    {
        public override void HandlePacket(KBEngine.Packet packet) {
            MyLib.GameInterface_Skill.UpdateSkillPoint(packet.protoBody as MyLib.GCPushSkillPoint);
        }

    }

    public class GCPushLevel : IPacketHandler {
        public override void HandlePacket(KBEngine.Packet packet) {
            MyLib.GameInterface_Skill.UpdateLevel(packet.protoBody as MyLib.GCPushLevel);
        }
    }

    public class GCPushExpChange : IPacketHandler
    {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            MyLib.GameInterface_Player.UpdateExp(packet.protoBody as MyLib.GCPushExpChange);
        }
    }
    public class GCPushShortcutsInfo : IPacketHandler {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            MyLib.GameInterface_Skill.UpdateShortcutsInfo(packet.protoBody as MyLib.GCPushShortcutsInfo);
        }
    }
    public class GCPushEquipDataUpdate : IPacketHandler {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            MyLib.BackPack.backpack.EquipDataUpdate(packet.protoBody as MyLib.GCPushEquipDataUpdate);
        }
    }
}