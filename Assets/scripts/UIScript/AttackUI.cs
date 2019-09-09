
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
using System;
using System.Collections.Generic;

/*
 * 右侧攻击按钮 
 * TODO:需要添加血瓶切换功能
 */

namespace MyLib
{
    /// <summary>
    /// 右下角 攻击按钮和快捷按钮的UI
    /// </summary>
    public class AttackUI : IUserInterface
    {
        void InitShortCut()
        {
            for (int i = 1; i <= 4; i++)
            {
                var shortCut = GameInterface_Skill.skillInterface.GetShortSkillData(i - 1);
                //Log.GUI("shortcut info "+i.ToString()+" "+shortCut);
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

        void Awake()
        {
            Log.GUI("Init Attack UI ");
            //SetCallback ("SelectButton", OnSelect);
            SetCallback("NormalATKButton", OnAttack);
			
            regEvt = new List<MyEvent.EventType>()
            {
                MyEvent.EventType.UpdateSkill,
                MyEvent.EventType.UpdateShortCut,
                MyEvent.EventType.UpdateMainUI,
            };
            RegEvent();
            SkillButColdTime();
        }

        void SkillButColdTime()
        {
            for (int i = 1; i <= 4; i++)
            {
                var but = GetName("SkillButton" + i.ToString());
                var sb = but.AddComponent<SkillBut>();
                sb.index = i-1;
            }
        }



        void OnAttack(GameObject g)
        {
            GameInterface.gameInterface.PlayerAttack();
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

    }
}