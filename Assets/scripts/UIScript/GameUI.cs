using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class GameUI : IUserInterface
    {
        UILabel HpLabel;
        UILabel mpLabel;
        UISlider hp;
        UISlider mp;

        UISlider exp;
        UILabel expLabel;
        UILabel level;

        UILabel hpLabel;
        void Awake()
        {

            HpLabel = GetLabel ("HPLabel");
            mpLabel = GetLabel ("MPLabel");
            hp = GetSlider ("HP");
            mp = GetSlider ("MP");
            expLabel = GetLabel("EXPLabel");
            exp = GetSlider("Exp");
            level = GetLabel("Level");

            hpLabel = GetLabel("HPNum");
            SetCallback("HPBottle", OnBottle);

            this.regEvt = new System.Collections.Generic.List<MyEvent.EventType>(){
                MyEvent.EventType.UpdateItemCoffer,
                MyEvent.EventType.UpdateMainUI,
                MyEvent.EventType.UpdatePlayerData,
            };
   
            RegEvent();
            SetCallback("Close", OnQuit);
        }
        void OnQuit(GameObject g){
            WindowMng.windowMng.ShowDialog(delegate(bool ret){
                if(ret){
                    WorldManager.worldManager.WorldChangeScene(2, false);
                }else {
                }
            }); 
        }
        void OnBottle(GameObject g){
            GameInterface_Backpack.UseItem((int)ItemData.ItemID.DRUG);
        }

        void UpdateFrame(){
            hpLabel.text = GameInterface_Backpack.GetHpNum().ToString(); 

            var me = ObjectManager.objectManager.GetMyData();
            HpLabel.text = me.GetProp(CharAttribute.CharAttributeEnum.HP).ToString ()+"/"+me.GetProp(CharAttribute.CharAttributeEnum.HP_MAX).ToString();
            mpLabel.text = me.GetProp(CharAttribute.CharAttributeEnum.MP).ToString ()+"/"+me.GetProp(CharAttribute.CharAttributeEnum.MP_MAX).ToString();

            hp.value = me.GetProp(CharAttribute.CharAttributeEnum.HP)*1.0f/me.GetProp(CharAttribute.CharAttributeEnum.HP_MAX);
            mp.value = me.GetProp(CharAttribute.CharAttributeEnum.MP)*1.0f/me.GetProp(CharAttribute.CharAttributeEnum.MP_MAX);

            var ep = me.GetProp(CharAttribute.CharAttributeEnum.EXP);
            var ma = me.GetProp(CharAttribute.CharAttributeEnum.EXP_MAX);
            exp.value = ep*1.0f/ma;
            expLabel.text = ep+"/"+ma;

            level.text = "[ff9500]等级:"+me.GetProp(CharAttribute.CharAttributeEnum.LEVEL)+"[-]";
        }

        protected override void OnEvent(MyEvent evt)
        {
            UpdateFrame();
        }
    }

}