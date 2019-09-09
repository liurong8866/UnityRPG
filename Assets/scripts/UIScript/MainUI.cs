using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    ///主城中的UI 
    /// </summary>
    public class MainUI : IUserInterface
    {
        UILabel hpLabel;
        UILabel level;

        void Awake()
        {
            hpLabel = GetLabel("HPNum");
            level = GetLabel("Level");

            SetCallback("ReturnCity_Button", OnCopy);
            //SetCallback("Knapsack_Button", OnBag);
            SetCallback("Skill_Button", OnSkill);
            SetCallback("NormalATKButton", OnTalk);
            SetCallback("StoreButton", OnStore);
            SetCallback("PackButton", OnPack);
            SetCallback("GMButton", OnGM);
            SetCallback("JingShiButton", OnJingShi);
            SetCallback("WorldButton", OnWorld);
            this.regEvt = new System.Collections.Generic.List<MyEvent.EventType>()
            {
                MyEvent.EventType.UpdateItemCoffer,
                MyEvent.EventType.UpdateMainUI,
                MyEvent.EventType.UpdatePlayerData,
            };
            RegEvent();
        }

        /// <summary>
        /// 进入世界场景 
        /// </summary>
        void OnWorld()
        {
            WorldManager.worldManager.WorldChangeScene(3, false);
        }
        void OnBomb()
        {
            WorldManager.worldManager.WorldChangeScene(4, false);
        }

        void OnTank()
        {
            //WorldManager.worldManager.WorldChangeScene(5, false);
            WindowMng.windowMng.PushView("UI/SelectTankUI");
        }

        void OnJingShi()
        {
            WindowMng.windowMng.PushView("UI/ChargeUI");
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateItemCoffer);
        }

        void OnGM()
        {
            WindowMng.windowMng.PushView("UI/GMCmd");

        }

        void OnPack(GameObject g)
        {
            WindowMng.windowMng.PushView("UI/Package", true);
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateItemCoffer);
        }

        void UpdateFrame()
        {
            hpLabel.text = GameInterface_Backpack.GetHpNum().ToString(); 
            var lev = GameInterface_Player.GetLevel();
            Log.GUI("lev " + lev);
            level.text = "[ff9500]等级:" + lev + "[-]";
        }

        protected override void OnEvent(MyEvent evt)
        {
            Log.GUI("OnEvent " + evt.type);
            UpdateFrame();
        }

        void OnTalk(GameObject g)
        {
            GameInterface_Player.TalkToNpc();
        }

        void OnCopy(GameObject g)
        {
            WindowMng.windowMng.PushView("UI/CopyList", true);
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.OpenCopyUI);
        }

        void OnBag(GameObject g)
        {
        }

        void OnSkill(GameObject g)
        {
            WindowMng.windowMng.PushView("UI/SkillUI", true);
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateSkill);
        }

        void OnStore(GameObject g)
        {
            WindowMng.windowMng.PushView("UI/StoreUI", true);
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateItemCoffer);
        }



    }
}
