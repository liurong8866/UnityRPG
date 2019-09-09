using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SkillCell : IUserInterface 
    {
        UILabel nameLabel;
        UILabel learnOrLevel;
        GameObject buy;
        private int sId;
        void Awake() {
            nameLabel = GetLabel("Name");
            learnOrLevel = GetLabel("Label");
            buy = GetName("Buy");
            //SetCallback("Buy", OnLearn);    
            SetCallback("Setting", OnSet);
        }

        /// <summary>
        /// 设置技能的名字 
        /// </summary>
        /// <param name="skillId">Skill identifier.</param>
        /// <param name="n">N.</param>
        /// <param name="lev">Lev.</param>
        /// <param name="needLev">Need lev.</param>
        /// <param name="desc">Desc.</param>
        /// <param name="maxLev">Max lev.</param>
        public void SetSkillName(int skillId, string n, int lev, int needLev, string desc, int maxLev){
            sId = skillId;
            var pos = GameInterface_Skill.GetSkillPos(skillId);
            var ps = "";
            if(pos == 0){
                ps = "未装备";
            }else if(pos == 1){
                ps = "装备位置:一";
            }else if(pos == 2) {
                ps = "装备位置:二";
            }else if(pos == 3) {
                ps = "装备位置:三";
            }else if(pos == 4){
                ps = "装备位置:四";
            }

            if(lev <= 0){
                nameLabel.text = string.Format("[959595]{0}[-]\n[98fcfc]{3}[-]\n[959595]需要等级:{1}[-]\n[959595]{2}[-]", n, needLev, desc, ps);
                SetLearn(0);
            }else if(lev < maxLev) {
                SetLearn(1);
                nameLabel.text = string.Format("[ff9500]{0}[-]\n[98fcfc]{4}[-]\n[0098fc]等级：{1}[-]\n[73d216]下一级需要：{2}[-]\n[f85818]{3}[-]", n, lev, needLev, desc, ps);
            }else {
                SetLearn(2);
                nameLabel.text = string.Format("[ff9500]{0}[-]\n[98fcfc]{3}[-]\n[0098fc]等级：{1}满级[-]\n[f85818]{2}[-]", n, lev, desc, ps);
            }
        }

        void SetLearn(int b){
            buy.SetActive(true);
            if(b == 0) {
                learnOrLevel.text = "学习";
            }else if(b == 1) {
                learnOrLevel.text = "升级";
            }else {
                //learnOrLevel.text = "最高级";
                buy.SetActive(false);
            }
        }
                    
       
        public void SetCb(EmptyDelegate cb ){
            SetCallback("Buy", delegate(GameObject g) {
                cb();
           });
        }

        void OnSet() {
            var skillSet = WindowMng.windowMng.PushView("UI/SkillSetUI");
            var setUI = skillSet.GetComponent<SkillSetUI>();
            setUI.skillId = sId;
        }
    }

}