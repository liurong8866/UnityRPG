using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SkillSetUI : IUserInterface
    {
        private int skId;
        UILabel skName;
        public int skillId{
            set {
                skId = value;
                var pos = GameInterface_Skill.GetSkillPos(skId);
                var sd = Util.GetSkillData(skId, 1);
                var ps = "";
                if(pos == 0){
                    ps = "未装备";
                }else if(pos == 1){
                    ps = "装备位置一";
                }else if(pos == 2) {
                    ps = "装备位置二";
                }else if(pos == 3) {
                    ps = "装备位置三";
                }else if(pos == 4){
                    ps = "装备位置四";
                }
                skName.text = string.Format("选择[ff1010]{0}[-]技能槽，当前：[10ff10]{1}[-]", sd.SkillName, ps);
            }
            private get {
                return skId;
            }
        }
        void Awake() {
            skName = GetLabel("SkillName");
            SetCallback("Close", Hide);
            SetCallback("Buy1", ()=>{
                SetSlot(1);
            });

            SetCallback("Buy2", ()=>{
                SetSlot(2);
            });

            SetCallback("Buy3", ()=>{
                SetSlot(3);
            });

            SetCallback("Buy4", ()=>{
                SetSlot(4);
            });
        }
        void SetSlot(int s){
            WindowMng.windowMng.PopView();
            GameInterface_Backpack.SetSkillPos(skillId, s);
        }
    }

}