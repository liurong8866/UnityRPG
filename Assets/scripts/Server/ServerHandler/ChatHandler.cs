using UnityEngine;
using System.Collections;

using playerData = MyLib.PlayerData;
using System;

namespace ServerPacketHandler
{
    public class CGSendChat : IPacketHandler
    {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as MyLib.CGSendChat;

            var push = MyLib.GCPushChat2Client.CreateBuilder();
            push.PlayerId = 1;
            push.PlayerName = "You";
            push.PlayerLevel = 1;
            push.PlayerJob = 1;
            push.PlayerVip = 1;
            push.TargetId = 3;
            push.Channel = 0;
            push.ChatContent = inpb.Content;
            MyLib.ServerBundle.SendImmediatePush(push);
             
            var cmds = inpb.Content.Split(char.Parse(" "));

            try
            {
                if (cmds [0] == "add_gold")
                {
                    playerData.AddGold(System.Convert.ToInt32(cmds [1]));
                } else if (cmds [0] == "add_sp")
                {
                    playerData.AddSkillPoint(System.Convert.ToInt32(cmds [1]));
                } else if (cmds [0] == "add_lvl")
                {
                    playerData.AddLevel(System.Convert.ToInt32(cmds [1]));
                } else if (cmds [0] == "add_exp")
                {
                    playerData.AddExp(System.Convert.ToInt32(cmds [1]));
                } else if (cmds [0] == "pass_lev")
                {
                    playerData.PassLev(System.Convert.ToInt32(cmds [1]), Convert.ToInt32(cmds[2]));
                } else if (cmds [0] == "kill_all")
                {
                }else if(cmds[0] == "add_item") {
                    playerData.AddItemInPackage(System.Convert.ToInt32(cmds[1]), System.Convert.ToInt32(cmds[2]));
                }else if(cmds[0] == "add_forge") {
                    MyLib.GameInterface_Forge.AddForgeLevel(Convert.ToInt32(cmds[1]));
                }else if(cmds[0] == "add_jingshi") {
                    playerData.AddJingShi(System.Convert.ToInt32(cmds [1]));
                }else if(cmds[0] == "show_item") {
                }else if(cmds[0] == "helpme") {
                    playerData.HelpMe();
                }else if(cmds[0] == "pass_task") 
                {
                    var l = Convert.ToInt32(cmds[1]);
                    MyLib.GameInterface_Player.SetIntState(MyLib.GameBool.cunZhangState, l);
                }
            } catch (System.Exception e)
            {
                Log.Critical("ServerException "+e);
            }
                

        }

    }
}
