using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class SkillUI : IUserInterface
    {
        UILabel gNum;
        GameObject cell;
        List<GameObject> cells;
        UIGrid grid;
        void Awake()
        {
            gNum = GetLabel("gNum");
            cell = GetName("Cell");
            cell.SetActive(false);
            grid = GetGrid("Grid");
            cells = new List<GameObject>();

            SetCallback("Close", Hide);
            regEvt = new System.Collections.Generic.List<MyEvent.EventType>(){
                MyEvent.EventType.UpdateSkill,
                MyEvent.EventType.UpdateShortCut,
            };
            RegEvent();
        }

        protected override void OnEvent(MyEvent evt)
        {
            UpdateFrame();
        }

        void UpdateFrame()
        {
            foreach(var c in cells){
                GameObject.Destroy(c);
            }
            cells.Clear();

            gNum.text = "技能点："+GameInterface_Skill.skillInterface.GetLeftSp();
            var skillList = GameInterface_Skill.skillInterface.GetActiveSkill();
            for(int i = 0; i < skillList.Count; i++){
                var data = skillList[i];
                var nc = GameObject.Instantiate(cell) as GameObject;
                nc.SetActive(true);
                nc.transform.parent = cell.transform.parent;
                Util.InitGameObject(nc);

                var c = nc.GetComponent<SkillCell>();
                var nextLevSkill = Util.GetSkillData(data.skillId, data.level+1);
                c.SetSkillName(data.skillId, data.skillData.SkillName, data.level, nextLevSkill.LevelRequired, data.skillData.SkillDes, data.skillData.MaxLevel);

                c.SetCb(delegate() {
                    GameInterface_Skill.skillInterface.SkillLevelUp(data.skillId);
                });
                cells.Add(nc);
            }

            grid.repositionNow = true;
        }
    }

}