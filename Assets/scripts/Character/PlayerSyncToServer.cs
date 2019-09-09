using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class PlayerSyncToServer : MonoBehaviour
    {
        AvatarInfo lastInfo;
        AvatarInfo curInfo;

        public void InitData(AvatarInfo info)
        {
            lastInfo = AvatarInfo.CreateBuilder(info).Build();
            curInfo = AvatarInfo.CreateBuilder(info).Build();
        }

        public void SyncAttribute()
        {
            var me = gameObject;
            var pos = me.transform.position;
            var dir = (int)me.transform.localRotation.eulerAngles.y;
            var meAttr = me.GetComponent<NpcAttribute>();
            var intPos = NetworkUtil.ConvertPos(pos);

            var ainfo = AvatarInfo.CreateBuilder();
            var change = false;
            //Pos 和 Dir 同时同步
            if (intPos [0] != lastInfo.X || intPos [1] != lastInfo.Y || intPos [2] != lastInfo.Z || dir != lastInfo.Dir)
            {
                ainfo.X = intPos [0];
                ainfo.Y = intPos [1];
                ainfo.Z = intPos [2];

                curInfo.X = ainfo.X;
                curInfo.Y = ainfo.Y;
                curInfo.Z = ainfo.Z;

                ainfo.Dir = dir;
                curInfo.Dir = dir;
                change = true;
            }

            if (meAttr.HP != lastInfo.HP)
            {
                ainfo.HP = meAttr.HP;
                curInfo.HP = meAttr.HP;
                change = true;
            }
            var intNetSpeed = (int)(meAttr.NetSpeed * 100);
            if (intNetSpeed != lastInfo.NetSpeed)
            {
                ainfo.NetSpeed = intNetSpeed;
                curInfo.NetSpeed = intNetSpeed;
                change = true;
            }
            var intThrowSpeed = (int)(meAttr.ThrowSpeed * 100);
            if (intThrowSpeed != lastInfo.ThrowSpeed)
            {
                ainfo.ThrowSpeed = intThrowSpeed;
                curInfo.ThrowSpeed = intThrowSpeed;
                change = true;
            }
            var intJumpSpeed = (int)(meAttr.JumpForwardSpeed * 100);
            if (intJumpSpeed != lastInfo.JumpForwardSpeed)
            {
                ainfo.JumpForwardSpeed = intJumpSpeed;
                curInfo.JumpForwardSpeed = intJumpSpeed;
                change = true;
            }

            if(meAttr.userName != lastInfo.Name) {
                ainfo.Name = meAttr.userName;
                curInfo.Name = meAttr.userName;
                change = true;
            }

            if(lastInfo.Job != ServerData.Instance.playerInfo.Roles.Job) {
                ainfo.Job = ServerData.Instance.playerInfo.Roles.Job;
                curInfo.Job = ainfo.Job;
                change = true;
            }

            if (change)
            {
                lastInfo = AvatarInfo.CreateBuilder(curInfo).Build();

                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "UpdateData";
                cg.AvatarInfo = ainfo.Build();
                NetworkUtil.Broadcast(cg);
            }
        }

    }

}