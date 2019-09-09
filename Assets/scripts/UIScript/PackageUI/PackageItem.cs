using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class PackageItem : IUserInterface
    {
        UILabel Name;
        EquipData equipData;
        BackpackData backpack;
        EmptyDelegate cb;
        void Awake()
        {
            SetCallback("Info", OnInfo);
            Name = GetLabel("Name");
        }
        void OnInfo(GameObject g){
            Log.GUI("OnInfo "+gameObject+" cb "+cb);
            if(cb != null) {
                cb();
            }else {
                if(equipData != null) {
                    Log.GUI("ShowDetailForEquip "+equipData.itemData.ItemName);
                }
                var win = WindowMng.windowMng.PushView("UI/DetailInfo");
                var detail = win.GetComponent<DetailInfo>();
                detail.SetEquip(equipData);
                detail.backpackData = backpack;

                MyEventSystem.PushEventStatic(MyEvent.EventType.UpdateDetailUI);
            }
        }
        public void SetButtonCB(EmptyDelegate c){
            Log.GUI("SetCb "+cb+" g "+gameObject);
            cb = c;
        }
        public void SetEquipData(EquipData equip) {
            equipData = equip;
            Name.text = string.Format("[ff9500]{0}({1}级)[-]", equip.itemData.ItemName, equipData.entry.Level); 
        }
        public void SetData(BackpackData data, int used = 0, bool showNum = true)
        {
            backpack = data;
            if(data.itemData.IsEquip()) {
                Name.text = string.Format("[ff9500]{0}({1}级)[-]", data.GetTitle(), data.entry.Level);
            }else if(data.itemData.IsGem()){
                if(showNum) {
                    Name.text = string.Format("[ff9500]{0}({1}阶)[-]\n[0098fc]数量：{2}[-]", data.GetTitle(), data.itemData.Level, data.num-used);
                }else {
                    Name.text = string.Format("[ff9500]{0}({1}阶)[-]", data.GetTitle(), data.itemData.Level);
                }
            }else {
                if(showNum) {
                    Name.text = string.Format("[ff9500]{0}[-]\n[0098fc]数量：{1}[-]", data.GetTitle(), data.num-used);
                }else {
                    Name.text = string.Format("[ff9500]{0}[-]\n", data.GetTitle());
                }
            }
        }
    }

}