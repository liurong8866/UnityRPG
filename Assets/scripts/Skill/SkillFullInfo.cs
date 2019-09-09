using UnityEngine;
using System.Collections;

namespace MyLib
{
	/// <summary>
	/// 技能操作对象 主界面上的玩家技能对象
	/// 包含技能的位置信息
	/// ActionItem 
	/// </summary>
	public class SkillFullInfo
	{
		public SkillData skillData;
		public int level {
			get {
				return skillData.Level;
			}
		}
		public int skillId {
			get {
				return skillData.Id;
			}
		}


		public int shortSlotId {
			get {
				return shortInfo.Index;
			}
		}
        //public int Pos;

		//动态变化的技能冷却时间
		public float CoolDownTime = 0;

		//主动被动技能初始化
		//SkillInfo skillInfo = null; 
		//
		ShortCutInfo shortInfo = null;

		public void Update ()
		{
			CoolDownTime -= Time.deltaTime;
			CoolDownTime = Mathf.Max (0, CoolDownTime);
		}

        public void SetCoolDown() {
            CoolDownTime = skillData.skillConfig.FullColdTime/1000.0f;
        }
		public bool CheckCoolDown ()
		{
			return CoolDownTime <= 0;
		}


		//TODO:初始化快捷键上面的技能数据，不包含等级等数据
		public SkillFullInfo(int skId, int index) {
			var sinfo = ShortCutInfo.CreateBuilder ();
			sinfo.Index = index;
			sinfo.BaseId = skId;
			sinfo.Type = 0;
			shortInfo = sinfo.BuildPartial ();

			skillData = Util.GetSkillData (skId, 1);
		}

        public SkillFullInfo(GCPushActivateSkill p){
            skillData = Util.GetSkillData(p.SkillId, p.Level);
        }

        public void SetLevel(int lev){
            skillData = Util.GetSkillData(skillId, lev);
        }

		public SkillFullInfo(SkillInfo sk) {
			skillData = Util.GetSkillData (sk.SkillInfoId, sk.Level);
		}

		//TODO:快捷键技能的信息 需要通过ShortCutInfo和普通的SkillInfo 来初始化 获得技能的等级信息
		//TODO:根据快捷键信息 初始化技能信息 或者 使用道具药品的信息
		public SkillFullInfo(ShortCutInfo sh) {
			shortInfo = sh;
			skillData = Util.GetSkillData (sh.BaseId, 1);
            //Pos = sh.Index;
		}

		//TODO:临时接口
		public SkillFullInfo(SkillData sd) {
			skillData = sd;
		}
	}



}