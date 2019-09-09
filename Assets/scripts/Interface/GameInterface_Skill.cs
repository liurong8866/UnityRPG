using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class GameInterface_Skill
    {
        public static GameInterface_Skill skillInterface = new GameInterface_Skill();

        public List<SkillFullInfo> GetActiveSkill()
        {
            return SkillDataController.skillDataController.activeSkillData;
        }

        public List<SkillFullInfo> GetPassitiveSkill()
        {
            return SkillDataController.skillDataController.passive;
        }

        public void SkillLevelUp(int skId)
        {
            //SkillDataController.skillDataController.StartCoroutine (
            SkillDataController.skillDataController.SkillLevelUpWithSp(skId);
            //);
        }

        public void SetSkillShortCut(int skId, int index)
        {
            SkillDataController.skillDataController.StartCoroutine(SkillDataController.skillDataController.SetSkillShortCut(skId, index));
        }

        public SkillData GetSkillData(int skillId, int level)
        {
            return Util.GetSkillData(skillId, level);
        }

        public SkillData GetShortSkillData(int shortId)
        {
            var shortData = SkillDataController.skillDataController.GetShortSkillData(shortId);
            //Log.Sys("GetShortSkillData " + shortId + " d " + shortData);
            return shortData;
        }

        /// <summary>
        /// 从ShortSkillData中获取技能的位置比较可靠 
        /// </summary>
        /// <returns>The skill position.</returns>
        public static int GetSkillPos(int skillId)
        {
            for (var i = 0; i < 4; i++)
            {
                var sk = GameInterface_Skill.skillInterface.GetShortSkillData(i);
                if (sk != null)
                {
                    if (sk.Id == skillId)
                    {
                        return i + 1;
                    }
                }
            }
            return 0;
        }


        public string GetSkillDesc(SkillData sk)
        {
            var str = sk.SkillDes + "\n";
            str += string.Format("额外增加{0}武器伤害", sk.WeaponDamagePCT);
            //其它效果
            return str;
        }

        public int GetLeftSp()
        {
            return SkillDataController.skillDataController.TotalSp - SkillDataController.skillDataController.DistriSp;
        }

        public int DistriSp()
        {
            return SkillDataController.skillDataController.DistriSp;
        }


        /// <summary>
        /// 本地使用技能同时通知代理
        /// 绕过LogicCommand 本地执行不需要LogicCommand队列 
        /// </summary>
        /// <param name="skillData">Skill data.</param>
        static void UseSkill(SkillData skillData)
        {
            Log.Sys("UseSkill: " + skillData.SkillName + " lev " + skillData.Level);
            ObjectManager.objectManager.GetMyPlayer().GetComponent<MyAnimationEvent>().OnSkill(skillData);

            NetDateInterface.FastMoveAndPos();
            NetDateInterface.FastUseSkill(skillData.Id, skillData.Level);
        }

        /// <summary>
        /// UI点击使用技能 
        /// </summary>
        /// <param name="skIndex">Sk index.</param>
        public static void OnSkill(int skIndex)
        {
            var skillData = SkillDataController.skillDataController.GetShortSkillData(skIndex);
            var curState = ObjectManager.objectManager.GetMyPlayer().GetComponent<AIBase>().GetAI().state;
            //无动作技能释放 不用检测
            if (string.IsNullOrEmpty(skillData.AnimationName))
            {
            } else
            {
                //当前状态不能使用技能 或者已经在技能状态了不能连续点击
                if (curState != null)
                {
                    var ret = curState.CanChangeState(AIStateEnum.CAST_SKILL);
                    if (!ret)
                    {
                        return;
                    }
                }
            }

            if (skillData != null)
            {
                var mana = ObjectManager.objectManager.GetMyData().GetProp(CharAttribute.CharAttributeEnum.MP);
                var cost = skillData.ManaCost;
                if (cost > mana)
                {
                    WindowMng.windowMng.ShowNotifyLog("魔法不足");
                    return;
                }
                var cd = SkillDataController.skillDataController.CheckCoolDown(skIndex);
                if (!cd)
                {
                    WindowMng.windowMng.ShowNotifyLog("冷却时间未到");
                    return;
                }
                SkillDataController.skillDataController.SetCoolDown(skIndex);
                var npc = ObjectManager.objectManager.GetMyAttr();
                npc.ChangeMP(-cost);
                UseSkill(skillData);
            }
        }

        public static void UpdateSkillPoint(GCPushSkillPoint sp)
        {
            SkillDataController.skillDataController.TotalSp = sp.SkillPoint;
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateSkill);
        }

        public static void UpdateLevel(GCPushLevel lev)
        {
            var charInfo = ObjectManager.objectManager.GetMyAttr();
            charInfo.ChangeLevel(lev.Level);
        }

        public static void UpdateShortcutsInfo(GCPushShortcutsInfo inpb)
        {
            SkillDataController.skillDataController.UpdateShortcutsInfo(inpb.ShortCutInfoList);
        }

        public static bool AddSkillBuff(GameObject who, int skillId, Vector3 pos)
        {
            var skill = Util.GetSkillData(skillId, 1);
            var skillInfo = SkillLogic.GetSkillInfo(skill);
            var evt = skillInfo.eventList [0];
            var ret = who.GetComponent<BuffComponent>().AddBuff(evt.affix, pos);
            return ret;
        }
        public static void RemoveSkillBuff(GameObject who, int skillId)
        {
            var skill = Util.GetSkillData(skillId, 1);
            var skillInfo = SkillLogic.GetSkillInfo(skill);
            var evt = skillInfo.eventList [0];
            who.GetComponent<BuffComponent>().RemoveBuff(evt.affix.effectType);
        }
    }
}
