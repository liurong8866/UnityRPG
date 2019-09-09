using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class LevelUpEquip : IUserInterface
    {
        EquipData equip;
        public LevelUpEquipLeft left;
        public LevelUpEquipRight right;

        List<BackpackData> gems = new List<BackpackData>();

        void Awake()
        {
            SetCallback("closeButton", Hide);
            right = GetName("Right").GetComponent<LevelUpEquipRight>();
            right.PutInGem = PutInGem ;
            left = GetName("Left").GetComponent<LevelUpEquipLeft>();
            left.parent = this;
            regEvt = new List<MyEvent.EventType>(){
                MyEvent.EventType.UpdateItemCoffer,
            };
            RegEvent();
        }

        public void SetEquip(EquipData ed) {
            equip = ed;
            left.SetEquip(ed);
        }

        protected override void OnEvent(MyEvent evt)
        {
            var allEquip = BackPack.backpack.GetEquipmentData();
            foreach(var e in allEquip) {
                if(e.id == equip.id) {
                    SetEquip(e);
                    break;
                }
            }
            gems.Clear();
            left.SetGems(gems);
            right.SetGems(gems);

        }

        void Start() {
            left.SetEquip(equip);
            //left.SetGems(gems);
            right.SetGems(gems);
        }

        public void PutInGem(BackpackData data)
        {
            var equipLevel = equip.entry.Level;
            var needLevel = (equipLevel-1)*5;
            var myLev = ObjectManager.objectManager.GetMyAttr().Level;
            if(myLev < needLevel) {
                WindowMng.windowMng.ShowNotifyLog(string.Format("只有自身达到{0}级,才能掌控更高级装备", needLevel));
                return;
            }

            var lev = data.itemData.Level;
            if(lev < equipLevel) {
                WindowMng.windowMng.ShowNotifyLog("此种神兵只有使用更高阶宝石方能炼化");
                return;
            }

            if(gems.Count >= 2){
                WindowMng.windowMng.ShowNotifyLog("宝石已满，需要取下宝石才能放入新的!");
            }else {
                gems.Add(data);
                left.SetGems(gems);
                right.SetGems(gems);
            }

        }
        public void TakeOffGem(BackpackData data) 
        {
            gems.Remove(data);
            left.SetGems(gems);
            right.SetGems(gems);
        }
        public void LevelUp(){
            if(gems.Count < 2){
                WindowMng.windowMng.ShowNotifyLog("需要放入两个宝石才能升级!");
            }else {

                GameInterface_Package.playerPackage.LevelUpEquip(equip, gems);
            }
        }

    }
}
