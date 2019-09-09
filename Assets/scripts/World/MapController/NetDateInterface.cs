using UnityEngine;
using System.Collections;

namespace MyLib
{
    public static class NetDateInterface
    {
        public static void FastAddBuff(Affix affix, GameObject attacker, GameObject target, int skillId, int evtId, Vector3 pos)
        {
            var cg = CGPlayerCmd.CreateBuilder();
            var binfo = BuffInfo.CreateBuilder();
            binfo.BuffType = (int)affix.effectType;
            binfo.Attacker = attacker.GetComponent<KBEngine.KBNetworkView>().GetServerID();
            binfo.Target = target.GetComponent<KBEngine.KBNetworkView>().GetServerID();
            binfo.SkillId = skillId;
            binfo.EventId = evtId;
            //var pos = attacker.transform.position;
            binfo.AddAttackerPos((int)(pos.x * 100));
            binfo.AddAttackerPos((int)(pos.y * 100));
            binfo.AddAttackerPos((int)(pos.z * 100));

            cg.BuffInfo = binfo.Build();
            cg.Cmd = "Buff";
            var sc = WorldManager.worldManager.GetActive();
            sc.BroadcastMsg(cg);
        }

        /// <summary>
        ///相同的技能 Skill Configure来触发Buff 但是不要触发 Buff修改非表现属性
        /// </summary>
        /// <param name="affix">Affix.</param>
        /// <param name="attacker">Attacker.</param>
        /// <param name="target">Target.</param>
        public static void FastAddBuff(Affix affix, GameObject attacker, GameObject target, int skillId, int evtId)
        {
            var cg = CGPlayerCmd.CreateBuilder();
            var binfo = BuffInfo.CreateBuilder();
            binfo.BuffType = (int)affix.effectType;
            binfo.Attacker = attacker.GetComponent<KBEngine.KBNetworkView>().GetServerID();
            binfo.Target = target.GetComponent<KBEngine.KBNetworkView>().GetServerID();
            binfo.SkillId = skillId;
            binfo.EventId = evtId;
            var pos = attacker.transform.position;
            binfo.AddAttackerPos((int)(pos.x * 100));
            binfo.AddAttackerPos((int)(pos.y * 100));
            binfo.AddAttackerPos((int)(pos.z * 100));

            cg.BuffInfo = binfo.Build();
            cg.Cmd = "Buff";
            var sc = WorldManager.worldManager.GetActive();
            sc.BroadcastMsg(cg);
        }

        public static void FastUseSkill(int skillId, int skillLevel)
        {
            var sc = WorldManager.worldManager.GetActive();
            if (sc.IsNet)
            {
                var cg = CGPlayerCmd.CreateBuilder();
                var skInfo = SkillAction.CreateBuilder();
                skInfo.Who = ObjectManager.objectManager.GetMyServerID(); 
                skInfo.SkillId = skillId;
                skInfo.SkillLevel = skillLevel;
                cg.SkillAction = skInfo.Build();
                cg.Cmd = "Skill";
                sc.BroadcastMsg(cg);
            }
        }

        public static void FastDamage(int attackerId, int enemyId, int damage, bool isCritical)
        {
            var cg = CGPlayerCmd.CreateBuilder();
            var dinfo = DamageInfo.CreateBuilder();
            dinfo.Attacker = attackerId;
            dinfo.Enemy = enemyId;
            dinfo.Damage = damage;
            dinfo.IsCritical = isCritical;
            cg.DamageInfo = dinfo.Build();
            cg.Cmd = "Damage";
            WorldManager.worldManager.GetActive().BroadcastMsg(cg);
        }

        public static void Revive()
        {
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "Revive";
            WorldManager.worldManager.GetActive().BroadcastMsg(cg);
        }

        public static void SyncTime(int leftTime)
        {
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "SyncTime";
            cg.LeftTime = leftTime;
            WorldManager.worldManager.GetActive().BroadcastMsg(cg);
        }

        public static void GameOver()
        {
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "GameOver";
            WorldManager.worldManager.GetActive().BroadcastMsg(cg);
        }

        public static void Dead(int last)
        {
            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "Dead";
            var dinfo = DamageInfo.CreateBuilder();
            dinfo.Attacker = last;
            dinfo.Enemy = ObjectManager.objectManager.GetMyAttr().GetNetView().GetServerID();
            cg.DamageInfo = dinfo.Build();
            WorldManager.worldManager.GetActive().BroadcastMsg(cg);
        }

        public static void FastMoveAndPos()
        {
            var s = WorldManager.worldManager.GetActive();
            if (s.IsNet)
            {
                var me = ObjectManager.objectManager.GetMyPlayer();
                if (me == null)
                {
                    return;
                }
                var pos = me.transform.position;
                var dir = (int)me.transform.localRotation.eulerAngles.y;

                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "Move";
                var ainfo = AvatarInfo.CreateBuilder();
                ainfo.X = (int)(pos.x * 100);
                ainfo.Z = (int)(pos.z * 100);
                ainfo.Y = (int)(pos.y * 100);
                ainfo.Dir = dir;
                cg.AvatarInfo = ainfo.Build();

                s.BroadcastMsg(cg);
            }
        }


        public static void SyncMonster()
        {
            if (NetworkUtil.IsNetMaster())
            {
                var allNetView = ObjectManager.objectManager.GetNetView();
                foreach (var v in allNetView)
                {
                    if (v != null)
                    {
                        var ms = v.GetComponent<MonsterSyncToServer>();
                        if (ms != null)
                        {
                            ms.SyncToServer();
                        }
                    }
                }
            }
        }

        public static void SyncPosDirHP()
        {
            var me = ObjectManager.objectManager.GetMyPlayer();
            if (me == null)
            {
                return;
            }
            var pos = me.transform.position;
            var dir = (int)me.transform.localRotation.eulerAngles.y;
            var meAttr = me.GetComponent<NpcAttribute>();

            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "UpdateData";
            var ainfo = AvatarInfo.CreateBuilder();
            ainfo.X = (int)(pos.x * 100);
            ainfo.Z = (int)(pos.z * 100);
            ainfo.Y = (int)(pos.y * 100);
            ainfo.Dir = dir;
            ainfo.HP = meAttr.HP;

            cg.AvatarInfo = ainfo.Build();

            var s = WorldManager.worldManager.GetActive();
            s.BroadcastMsg(cg);
        }

        public static PlayerSync GetPlayer(int id)
        {
            var player = ObjectManager.objectManager.GetPlayer(id);
            if (player != null)
            {
                var sync = player.GetComponent<PlayerSync>();
                return sync;
            }
            return null;
        }

    }
}
