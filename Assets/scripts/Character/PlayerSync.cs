
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

using System;
using KBEngine;

namespace MyLib
{

    /// <summary>
    /// 网络对象的本地代理
    /// Proxy 接受网络同步 
    /// </summary>
    public class PlayerSync : KBEngine.MonoBehaviour
    {
        //AvatarInfo lastInfo;
        AvatarInfo curInfo;
        NpcAttribute attribute;

        void Awake()
        {
            curInfo = AvatarInfo.CreateBuilder().Build();
            StartCoroutine(SyncPos());
        }

        void Start()
        {
            attribute = GetComponent<NpcAttribute>(); 
        }

        IEnumerator SyncPos()
        {
            yield return new WaitForSeconds(2);
            while (true)
            {
                SendMoveCmd();
                yield return new WaitForSeconds(1);
            }
        }


        void SendMoveCmd()
        {
            var curPos = NetworkUtil.ConvertPos(transform.position);
            var dir = (int)transform.localRotation.eulerAngles.y;
            var meAttr = gameObject.GetComponent<NpcAttribute>();
            if (curInfo.X != curPos [0] || curInfo.Y != curPos [1] || curInfo.Z != curPos [2] || curInfo.Dir != dir)
            {
                var mvTarget = new Vector3(curInfo.X / 100.0f, curInfo.Y / 100.0f + 0.2f, curInfo.Z / 100.0f);
                var cmd = new ObjectCommand();
                cmd.targetPos = mvTarget;
                cmd.dir = curInfo.Dir;
                cmd.commandID = ObjectCommand.ENUM_OBJECT_COMMAND.OC_MOVE;
                GetComponent<LogicCommand>().PushCommand(cmd);
            }

            if (curInfo.HasJumpForwardSpeed)
            {
                var intJumpSpeed = (int)(meAttr.JumpForwardSpeed * 100);
                if (intJumpSpeed != curInfo.JumpForwardSpeed)
                {
                    meAttr.JumpForwardSpeed = curInfo.JumpForwardSpeed / 100.0f;
                }
            }

        }
        /*
		 * Write Message Send To Server
		 * PlayerManagerment  PhotonView Manager 
		 */
        public void NetworkMove(AvatarInfo info)
        {
            Log.Sys("NetworkMove: " + info);
            if (info.HasX)
            {
                curInfo.X = info.X;
                curInfo.Y = info.Y;
                curInfo.Z = info.Z;
                curInfo.Dir = info.Dir;
                SendMoveCmd();
            }

            var attr = GetComponent<NpcAttribute>();
            if (info.HasHP)
            {
                GetComponent<NpcAttribute>().SetHPNet(info.HP);
            }

            if (info.HasTeamColor)
            {
                GetComponent<NpcAttribute>().SetTeamColorNet(info.TeamColor);
            }
            if (info.HasNetSpeed)
            {
                GetComponent<NpcAttribute>().NetSpeed = info.NetSpeed / 100.0f;
            }
            if (info.HasThrowSpeed)
            {
                attr.ThrowSpeed = info.ThrowSpeed / 100.0f;
            }
            if (info.HasJumpForwardSpeed)
            {
                attr.JumpForwardSpeed = info.JumpForwardSpeed / 100.0f;
                curInfo.JumpForwardSpeed = info.JumpForwardSpeed;
            }
            if(info.HasName) {
                attr.userName = info.Name;
            }
            if(info.HasJob) {
                attr.job = info.Job;
            }
        }

        public void Revive()
        {
            attribute.NetworkRevive(); 
        }

        public void NetworkAttack(SkillAction sk)
        {
            var cmd = new ObjectCommand(ObjectCommand.ENUM_OBJECT_COMMAND.OC_USE_SKILL);
            cmd.skillId = sk.SkillId;
            cmd.skillLevel = sk.SkillLevel;
            Log.GUI("Other Player Attack LogicCommand");
            gameObject.GetComponent<LogicCommand>().PushCommand(cmd);
        }

        public void SetLevel(AvatarInfo info)
        {
            GetComponent<NpcAttribute>().ChangeLevel(info.Level);
        }

        public void SetPositionAndDir(AvatarInfo info)
        {
            Vector3 vxz = new Vector3(info.X / 100.0f, info.Y / 100.0f + 0.2f, info.Z / 100.0f);
            Log.Sys("SetPosition: " + info + " vxz " + vxz + " n " + gameObject.name);
            transform.position = new Vector3(vxz.x, vxz.y, vxz.y);
            transform.rotation = Quaternion.Euler(new Vector3(0, info.Dir, 0));
            StartCoroutine(SetPos(vxz));
        }

        /// <summary>
        /// 稳定一下初始化位置 
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="p">P.</param>
        IEnumerator SetPos(Vector3 p)
        {
            var c = 0;
            while (c <= 3)
            {
                transform.position = p;
                c++;
                yield return null;
            }
        }


        /// <summary>
        /// 本地控制对象接受网络命令
        /// 本地代理接受网络命令
        /// </summary>
        /// <param name="cmd">Cmd.</param>
        public void DoNetworkDamage(GCPlayerCmd cmd)
        {
            var eid = cmd.DamageInfo.Enemy;
            var attacker = ObjectManager.objectManager.GetPlayer(cmd.DamageInfo.Attacker);
            if (attacker != null)
            {
                gameObject.GetComponent<MyAnimationEvent>().OnHit(attacker, cmd.DamageInfo.Damage, cmd.DamageInfo.IsCritical);
            }
        }

        public void NetworkBuff(GCPlayerCmd cmd)
        {
            var sk = Util.GetSkillData(cmd.BuffInfo.SkillId, 1);
            var skConfig = SkillLogic.GetSkillInfo(sk);
            var evt = skConfig.GetEvent(cmd.BuffInfo.EventId);
            if (evt != null)
            {
                var pos = cmd.BuffInfo.AttackerPosList;
                var px = pos [0] / 100.0f;
                var py = pos [1] / 100.0f;
                var pz = pos [2] / 100.0f;
                gameObject.GetComponent<BuffComponent>().AddBuff(evt.affix, new Vector3(px, py, pz));
            }
        }
    }

}