
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
using System.Collections.Generic;
using System;
using SimpleJSON;

/// <summary>
/// 初始化技能列表 CActionItem_Skill 管理这些技能
/// </summary>
namespace MyLib
{
    public class SkillDataController : MonoBehaviour
    {
        public static SkillDataController skillDataController;

        //SkillPanel skillPanelCom;
        GameObject uiRoot;
        GameObject skillPanel;


        //右下角快捷栏 里面的 技能  还包括 使用药品的技能
        //TODO: 初始化结束之后 玩家 SkillinfoComponent 从这里获取快捷栏里面的技能  不包括普通攻击技能  普通技能的ID 根据单位的BaseSkill 确定
        //防御和闪避 也是固定的
        List<SkillFullInfo> skillSlots = new List<SkillFullInfo>();

        //CharacterData charData;
        //int distributedSkillPoint;
        int totalSkillPoint;
        //LeftSkillPoint Can Be Use
        public int TotalSp
        {
            get
            {
                return totalSkillPoint;
            }
            set
            {
                totalSkillPoint = value;
            }
        }

        public int DistriSp
        {
            get
            {
                return 0;
            }
        }

        List<SkillFullInfo> activeSkill = new List<SkillFullInfo>();
        List<SkillFullInfo> passiveSkill = new List<SkillFullInfo>();

        public List<SkillFullInfo> activeSkillData
        {
            get
            {
                return activeSkill;
            }
        }

        void Update()
        {
            foreach (SkillFullInfo s in skillSlots)
            {
                s.Update();
            }
        }

        public bool CheckCoolDown(int index)
        {
            foreach (SkillFullInfo s in skillSlots)
            {
                if (s.shortSlotId == index)
                {
                    return s.CheckCoolDown();
                }
            }
            return false;
        }
        public void SetCoolDown(int index) {
            foreach (SkillFullInfo s in skillSlots)
            {
                if (s.shortSlotId == index)
                {
                    s.SetCoolDown();
                    break;
                }
            }
        }

        public float GetCoolTime(int index) {
            foreach (SkillFullInfo s in skillSlots)
            {
                if (s.shortSlotId == index)
                {
                    return s.CoolDownTime;
                }
            }
            return 0;
        }

        public List<SkillFullInfo> passive
        {
            get
            {
                return passiveSkill;
            }
        }

	
        /// <summary>
        /// 返回实际技能的等级  快捷技能里面只有技能ID 
        /// </summary>
        /// <returns>The short skill data.</returns>
        /// <param name="index">Index.</param>
        public SkillData GetShortSkillData(int index)
        {
            //Log.Sys("GetShortSkillData "+index);
            foreach (SkillFullInfo s in skillSlots)
            {
                if (s.shortSlotId == index)
                {
                    //Log.Sys("SkillLevel: "+s.skillData.Level);
                    foreach (var sk in activeSkill)
                    {
                        if (sk.skillId == s.skillId)
                        {
                            return sk.skillData;
                        }
                    }

                    return s.skillData;
                    //return null
                }
            }
            return null;
        }

        void SetShortSkillData(int skId, int index)
        {
            foreach (SkillFullInfo s in skillSlots)
            {
                if (s.skillId == skId)
                {
                    skillSlots.Remove(s);
                    break;
                }
            }
            foreach (SkillFullInfo s in skillSlots)
            {
                if (s.shortSlotId == index)
                {
                    skillSlots.Remove(s);
                    break;
                }
            }
            var full = new SkillFullInfo(skId, index);
            skillSlots.Add(full);
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateShortCut);
        }

        public IEnumerator SetSkillShortCut(int skId, int index)
        {
            var packet = new KBEngine.PacketHolder();
            var b = CGModifyShortcutsInfo.CreateBuilder();
            b.IdAdd = true;
            var shortInfo = ShortCutInfo.CreateBuilder();
            shortInfo.Index = index;
            shortInfo.IndexId = index;
            shortInfo.BaseId = skId;
            shortInfo.Type = 0;
            b.SetShortCutInfo(shortInfo);

            yield return StartCoroutine(KBEngine.Bundle.sendSimple(this, b, packet));
            //var ret = packet.packet.protoBody as GCModifyShortcutsInfo;
            SetShortSkillData(skId, index);
        }

        public void UpdateShortcutsInfo(IList<ShortCutInfo> shortCutInfo)
        {
            skillSlots = new List<SkillFullInfo>();
            foreach (ShortCutInfo s in shortCutInfo)
            {
                var full = new SkillFullInfo(s);
                skillSlots.Add(full);
            }
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateShortCut);
        }

        /// <summary>
        /// 初始化右下角的技能快捷栏
        /// 初始化技能列表 
        /// </summary>
        /// <returns>The from network.</returns>
        public IEnumerator InitFromNetwork()
        {
            Log.Net("Init Skill slots");
            CGLoadShortcutsInfo.Builder b = CGLoadShortcutsInfo.CreateBuilder();
            KBEngine.PacketHolder p = new KBEngine.PacketHolder();
            yield return StartCoroutine(KBEngine.Bundle.sendSimple(this, b, p));
            var shortData = p.packet.protoBody as GCLoadShortcutsInfo;
            skillSlots = new List<SkillFullInfo>();
            foreach (ShortCutInfo s in shortData.ShortCutInfoList)
            {
                var full = new SkillFullInfo(s);
                skillSlots.Add(full);
            }
            Log.Net("Init Skill slots over " + skillSlots.Count);
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateShortCut);

			
            Log.Sys("LoadSkillPanel");
            //获取特定技能面板的技能点 所有面板公用这些技能点
            CGLoadSkillPanel.Builder ls = CGLoadSkillPanel.CreateBuilder();
            ls.SkillSort = SkillSort.ACTIVE_SKILL;
            KBEngine.PacketHolder p2 = new KBEngine.PacketHolder();
            yield return StartCoroutine(KBEngine.Bundle.sendSimple(this, ls, p2));

            Log.Net("FinishLoad");
            var data = p2.packet.protoBody as GCLoadSkillPanel;
            //distributedSkillPoint = data.Distributed;
            if (data.HasTotalPoint)
            {
                totalSkillPoint = data.TotalPoint;
            } else
            {
                totalSkillPoint = 0;
            }
            Dictionary<int, SkillInfo> skIdToInfo = new Dictionary<int, SkillInfo>();
            Log.GUI("SkillInfoList " + data.SkillInfosList.Count + " totalPoint " + totalSkillPoint);

            foreach (SkillInfo sk in data.SkillInfosList)
            {
                skIdToInfo [sk.SkillInfoId] = sk;
            }

            var myJob = MyLib.ObjectManager.objectManager.GetMyJob();
            Log.Sys("SkillMyJob AndList " + myJob);
            //从服务器加载对应的技能数据 两部分： 配置的职业技能 服务器加载的技能
            foreach (var s in GameData.SkillConfig)
            {
                if (s.job == myJob)
                {
                    if (skIdToInfo.ContainsKey(s.id))
                    {
                        activeSkill.Add(new SkillFullInfo(skIdToInfo [s.id]));
                    } else
                    {
                        var skInfo = SkillInfo.CreateBuilder();
                        skInfo.SkillInfoId = s.id;
                        skInfo.Level = 0;
                        skInfo.Pos = 0;
                        activeSkill.Add(new SkillFullInfo(skInfo.Build()));
                    }
                }
            }

            var learnedSkill = PlayerData.GetLearnedSkill();
            foreach (var s in learnedSkill.SkillInfosList)
            {
                var old = activeSkill.Find(item => item.skillId == s.SkillInfoId);
                if (old != null)
                {
                    //old.Pos = s.Pos;
                } else
                {
                    var skInfo = SkillInfo.CreateBuilder(s);
                    activeSkill.Add(new SkillFullInfo(skInfo.Build()));
                }
            }
            Log.Sys("MySkillCount " + activeSkill.Count);
        }



        void Awake()
        {
            skillDataController = this;
            DontDestroyOnLoad(gameObject);
	
        }

        /*
		 * 需要道具升级技能
		 * TODO:Push SP点数更新
		 */
        public void SkillLevelUpWithSp(int skillId)
        {
            var levelUp = CGSkillLevelUp.CreateBuilder();
            levelUp.SkillId = skillId;
            KBEngine.Bundle.sendImmediate(levelUp);
        }


        public void ActivateSkill(GCPushActivateSkill skill)
        {
            foreach (var a in activeSkill)
            {
                if (a.skillId == skill.SkillId)
                {
                    a.SetLevel(skill.Level);
                    break;
                }
            }
            MyEventSystem.PushEventStatic(MyEvent.EventType.UpdateSkill);
        }

    }

}