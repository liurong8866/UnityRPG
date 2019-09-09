using UnityEngine;
using System.Collections;
using playerData = MyLib.PlayerData;

namespace ServerPacketHandler {
    public class CGLoadSkillPanel : IPacketHandler {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            Log.Sys("LoadSkillPanelData");
            //var inpb = packet.protoBody as ChuMeng.CGLoadSkillPanel;
            var pd = MyLib.ServerData.Instance.playerInfo;
            if(pd.HasSkill){
                MyLib.ServerBundle.SendImmediate(pd.Skill.ToBuilder(), packet.flowId);
            }else {
                var ret = MyLib.GCLoadSkillPanel.CreateBuilder();
                MyLib.ServerBundle.SendImmediate(ret, packet.flowId);
            }
        }
    }

    public class CGSkillLevelUp : IPacketHandler {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as MyLib.CGSkillLevelUp;
            playerData.LevelUpSkill(inpb.SkillId);
        }
    }

    public class CGLoadShortcutsInfo : IPacketHandler {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            playerData.GetShortCuts(packet);
        }
    }
}
