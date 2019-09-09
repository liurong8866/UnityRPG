using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class CityAttackUI : IUserInterface
    {
        void Awake()
        {
            Log.GUI("Init Attack UI ");
            regEvt = new List<MyEvent.EventType>()
            {
                MyEvent.EventType.UpdateSkill,
                MyEvent.EventType.UpdateShortCut,
                MyEvent.EventType.UpdateMainUI,
            };
            RegEvent();
        }

        protected override void OnEvent(MyEvent evt)
        {
            UpdateFrame();
        }
        //TODO: 初始化技能按钮UI，可以通过事件机制通知，UpdateFrame
        void UpdateFrame()
        {
            InitShortCut();
        }

        //TODO: 和SkillController协作处理玩家的技能数据
        void OnSkill(int skIndex)
        {
            Debug.Log("skillbutton is " + skIndex);
            GameInterface_Skill.OnSkill(skIndex);
        }

        void InitShortCut()
        {
            Log.GUI("Init Short Cut Icon");
            for (int i = 1; i <= 4; i++)
            {
                var shortCut = GameInterface_Skill.skillInterface.GetShortSkillData(i - 1);
                Log.GUI("shortcut info " + i.ToString() + " " + shortCut);
                var but = GetName("SkillButton" + i.ToString());
                var icon = Util.FindChildRecursive(but.transform, "Background");
                int temp = i - 1;
                UIEventListener.Get(but).onClick = delegate(GameObject g)
                {
                    OnSkill(temp);
                };
                if (shortCut != null)
                {
                    icon.gameObject.SetActive(true);
                    Util.SetIcon(icon.GetComponent<UISprite>(), shortCut.sheet, shortCut.icon);
                    var b = but.GetComponent<UIButton>();
                    b.normalSprite = shortCut.icon;
                } else
                {
                    icon.gameObject.SetActive(false);
                }
            }
        }

    }

}