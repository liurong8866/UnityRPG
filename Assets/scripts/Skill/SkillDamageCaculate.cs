using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SkillDamageCaculate
    {
        /// <summary>
        /// 反弹伤害无法触发HitTarget事件 
        /// </summary>
        /// <param name="attacker">Attacker.</param>
        /// <param name="WeaponDamagePCT">Weapon damage PC.</param>
        /// <param name="enemy">Enemy.</param>
        public static void DoDamage(GameObject attacker, int WeaponDamagePCT, GameObject enemy)
        {
            if (enemy.GetComponent<MyAnimationEvent>() != null)
            {
                if (enemy.GetComponent<NpcAttribute>().IsMine())
                {
                    var attribute = attacker.GetComponent<NpcAttribute>();
                    var rate = 1;
                    bool isCritical = false;

                    //在基础攻击力上面提升的比例
                    //调整基础比例
                    var damage = (int)(attribute.Damage * ( WeaponDamagePCT / 100.0f) * rate);
                    Log.Sys("calculate Damage Rate SimpleDamage " + WeaponDamagePCT);
                    NetDateInterface.FastDamage(
                        attribute.GetComponent<KBEngine.KBNetworkView>().GetServerID(),
                        enemy.GetComponent<KBEngine.KBNetworkView>().GetServerID(),
                        damage,
                        isCritical
                    );
                    enemy.GetComponent<MyAnimationEvent>().OnHit(attacker, damage, isCritical);
                }
            }
        }

        //TODO::支持单人副本和多人副本功能 取决于  是否直接通知MyAnimationEvent
        //根据技能信息和玩家信息 得到实际的 伤害  NpcAttribute  SkillFullInfo
        /*
		伤害计算过程
		1：伤害对象判定  客户端做
		2：伤害数值确定   服务端 或者客户端 
		3：伤害效果施展 例如击退  服务端 或者 客户端
		*/
        public static void DoDamage(GameObject attacker, SkillFullInfo skillData, GameObject enemy)
        {
            if (enemy.GetComponent<MyAnimationEvent>() != null)
            {
                var attribute = attacker.GetComponent<NpcAttribute>();
                //技能伤害方是我方则可以计算技能伤害否则只做技能表现
                if (enemy.GetComponent<NpcAttribute>().IsMine())
                {
                    var rd = Random.Range(0, 100);
                    var rate = 1;
                    bool isCritical = false;
                    if (rd < attribute.GetCriticalRate())
                    {
                        rate = 2;
                        isCritical = true;
                    }
                    //在基础攻击力上面提升的比例
                    var damage = (int)(attribute.Damage * (skillData.skillData.WeaponDamagePCT / 100.0f) * rate );
                    Log.Sys("calculate Damage Rate " +skillData.skillData.Id+" lev "+skillData.skillData.Level+" ra "+ skillData.skillData.WeaponDamagePCT);

                    NetDateInterface.FastDamage(
                        attribute.GetComponent<KBEngine.KBNetworkView>().GetServerID(),
                        enemy.GetComponent<KBEngine.KBNetworkView>().GetServerID(),
                        damage,
                        isCritical
                    );

                    enemy.GetComponent<MyAnimationEvent>().OnHit(attacker, damage, isCritical);
                    var hitTarget = new MyEvent(MyEvent.EventType.HitTarget);
                    hitTarget.target = enemy;
                    hitTarget.skill = skillData.skillData;
                    MyEventSystem.myEventSystem.PushLocalEvent(attribute.GetLocalId(), hitTarget);
                }
            }
        }


        /// <summary>
        /// 得到伤害计算层 
        /// </summary>
        /// <returns>The damage layer.</returns>
        public static int GetDamageLayer()
        {
            return 1 << (int)GameLayer.Npc | 1 << (int)GameLayer.IgnoreCollision2 | 1 << (int)GameLayer.Block;
        }

        public static int GetBlockerLayer() {
            return 1 << (int)GameLayer.Block;
        }
    }

}