
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class GameInterface 
	{
		public static GameInterface gameInterface = new GameInterface();
	
        /// <summary>
        /// 和使用普通技能一样 
        /// </summary>
		public void PlayerAttack() {
            //连击3招
            var skillId = ObjectManager.objectManager.GetMyPlayer ().GetComponent<SkillInfoComponent> ().GetDefaultSkillId ();
            var skillData = Util.GetSkillData(skillId, 1);
            ObjectManager.objectManager.GetMyPlayer().GetComponent<MyAnimationEvent>().OnSkill(skillData);

            NetDateInterface.FastMoveAndPos();
            NetDateInterface.FastUseSkill(skillId, skillData.Level);
		}


		//将背包物品装备起来
		public void PacketItemUserEquip(BackpackData item) {
			//摆摊
			//验证使用规则
			//判断等级
			var myself = ObjectManager.objectManager.GetMyPlayer ();
			if (myself != null) {
				if( item.GetNeedLevel () != -1 && myself.GetComponent<NpcAttribute>().Level < item.GetNeedLevel()) {
					var evt = new MyEvent(MyEvent.EventType.DebugMessage);
					evt.strArg = "等级不够";
					MyEventSystem.myEventSystem.PushEvent(evt);
					return;
				}
			}

			BackPack.backpack.SetSlotItem (item);
			BackPack.backpack.StartCoroutine(BackPack.backpack.UseEquipForNetwork ());
		}

		

	}
}
