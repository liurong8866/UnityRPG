using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class PackageUI : IUserInterface
    {
        UILabel goldNum;
        void Awake()
        {
            SetCallback("closeButton", Hide);
            goldNum = GetLabel("GoldNum/Label");
            regEvt = new List<MyEvent.EventType>() {
                MyEvent.EventType.UpdateItemCoffer, //DrugNum
            };
            RegEvent();
            SetCallback("Forge", OnForge);
        }

        void OnForge() {
            WindowMng.windowMng.PushView("UI/ForgeList");
        }
           
        protected override void OnEvent(MyEvent evt)
        {
            var me = ObjectManager.objectManager.GetMyData();
            goldNum.text = "金币: "+me.GetProp(CharAttribute.CharAttributeEnum.GOLD_COIN);
        }
    }

}