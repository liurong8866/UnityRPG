using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class DetailInfo : IUserInterface
    {
        private BackpackData _bd;

        public BackpackData backpackData
        {
            get
            {
                return _bd;
            }
            set
            {
                _bd = value;
                if(_bd != null) {
                    InitButton();
                }
            }
        }

        private EquipData equipData;
        UILabel Name;
        GameObject OneKey;
        GameObject Learn;
        GameObject LevelUp;
        GameObject Equip;
        GameObject Sell;

        void Awake()
        {
            Name = GetLabel("Name");
            SetCallback("closeButton", Hide);
            LevelUp = GetName("LevelUp");
            SetCallback("LevelUp", OnLevelUp);
            SetCallback("Equip", OnEquip);
            SetCallback("Sell", OnSell);
            OneKey = GetName("OneKey");
            Learn = GetName("Learn");
            Sell = GetName("Sell");
            Learn.SetActive(false);
            SetCallback("Learn", OnLearn);

            SetCallback("OneKey", OnOneKey);
            OneKey.SetActive(false);
            Equip = GetName("Equip");
            Equip.SetActive(false);

            regEvt = new System.Collections.Generic.List<MyEvent.EventType>()
            {
                MyEvent.EventType.UpdateItemCoffer,
                MyEvent.EventType.UpdateDetailUI,
            };
            RegEvent();
        }

        void OnLearn()
        {
            WindowMng.windowMng.PopView();
            var pt = backpackData.itemData.propsConfig.propsType;

            if(pt == (int)ItemData.UnitTypeEnum.SKILL_BOOK) {
                GameInterface_Backpack.LearnSkillBook(backpackData.id);
            }else if(pt == (int)ItemData.UnitTypeEnum.FORGE_GRAPH)  {
                GameInterface_Forge.LearnForgeSkill(backpackData.id);
            }
        }

        void InitButton()
        {
            OneKey.SetActive(false);
            Learn.SetActive(false);
            LevelUp.SetActive(false);
            Equip.SetActive(false);
            Sell.SetActive(false);
            Log.GUI("InitButton: "+backpackData);

            if (equipData != null)
            {
                LevelUp.SetActive(true);
            } 
            else if (backpackData != null)
            {
                if (backpackData.itemData.IsEquip())
                {
                    Equip.SetActive(true);
                } else
                {
                    var itemD = backpackData.itemData;
                    Log.GUI("PropsTYpe: "+itemD.UnitType);
                    if (backpackData.itemData.UnitType == ItemData.UnitTypeEnum.FORGE_GRAPH)
                    {
                        Learn.SetActive(true);
                        Sell.SetActive(true);
                    } else if (backpackData.itemData.UnitType == ItemData.UnitTypeEnum.GEM)
                    {
                        LevelUp.SetActive(true);
                        OneKey.SetActive(true);
                        Sell.SetActive(true);
                    } else if (backpackData.itemData.UnitType == ItemData.UnitTypeEnum.MATERIAL)
                    {
                        Sell.SetActive(true);
                    }else if(backpackData.itemData.UnitType == ItemData.UnitTypeEnum.SKILL_BOOK) {
                        Learn.SetActive(true);
                        Sell.SetActive(true);
                    }
                    else if(itemD.UnitType == ItemData.UnitTypeEnum.QUESTITEM) {
                        Sell.SetActive(false);
                        Log.GUI("PropsTYpe Quest: "+itemD.UnitType);
                    }
                    else {
                        Sell.SetActive(true);
                        Debug.LogError("Unknown Item Type "+backpackData.itemData.UnitType);
                    }
                }
            } 
            else
            {
                Debug.LogError("Data is Null ");
            }
        }

        void OnOneKey()
        {
            if (backpackData != null)
            {
                if (backpackData.entry.Count >= 2)
                {
                    GameInterface_Package.playerPackage.LevelUpGem(new List<BackpackData>(){ backpackData });
                    WindowMng.windowMng.PopView();
                } else
                {
                    Util.ShowMsg("宝石数量需要大于2个才能合成");
                }
            }
        }

        void OnLevelUp()
        {
            if (equipData != null)
            {
                var lev = equipData.entry.Level;
                var needLevel = GMDataBaseSystem.SearchIdStatic<EquipLevelData>(GameData.EquipLevel, lev);
                var myLev = ObjectManager.objectManager.GetMyAttr().Level;
                if (needLevel.level > myLev)
                {
                    WindowMng.windowMng.ShowNotifyLog(string.Format("只有自身达到{0}级,才能掌控更高级装备", needLevel.level));
                    return;
                }

                var lv = WindowMng.windowMng.PushView("UI/LevelUpEquip");
                var eq = lv.GetComponent<LevelUpEquip>();
                eq.SetEquip(equipData);
            } else
            { 


                var lv = WindowMng.windowMng.PushView("UI/LevelUpGem");
                var gem = lv.GetComponent<LevelUpGem>();
                gem.SetData(backpackData);
                MyEventSystem.PushEventStatic(MyEvent.EventType.UpdateItemCoffer);
            }
        }

        void OnEquip()
        {
            Hide(gameObject);
            GameInterface.gameInterface.PacketItemUserEquip(backpackData);
        }

        public void SetEquip(EquipData ed)
        {
            equipData = ed;
            if(equipData != null) {
                InitButton();
            }
        }

        protected override void OnEvent(MyEvent evt)
        {
            if (equipData != null)
            {
                var allEquip = BackPack.backpack.GetEquipmentData();
                foreach (var e in allEquip)
                {
                    if (e.id == equipData.id)
                    {
                        SetEquip(e);
                        break;
                    }
                }


                string baseAttr = "";
                if (equipData.itemData.Damage > 0)
                {
                    baseAttr += string.Format("[9800fc]攻击力:{0}[-]\n", equipData.itemData.Damage);
                }
                if (equipData.itemData.RealArmor > 0)
                {
                    baseAttr += string.Format("[8900cf]防御力:{0}[-]\n", equipData.itemData.RealArmor);
                }

                string initAttr = string.Format("[fc0000]天赋攻击:{0}[-]\n[fc0000]天赋防御:{1}[-]\n", 
                    equipData.entry.RndAttack,
                    equipData.entry.RndDefense);

                string extarAttr = string.Format("[fc0000]额外攻击:{0}[-]\n[fc0000]额外防御:{1}[-]\n", 
                                       equipData.entry.ExtraAttack,
                                       equipData.entry.ExtraDefense);

                Name.text = string.Format("[ff9500]{0}({1}级)[-]\n[0098fc]{2}金币[-]\n{3}{4}{5}[fcfc00]{6}[-]",
                    equipData.itemData.ItemName, 
                    equipData.entry.Level,
                    equipData.itemData.GoldCost,
                    baseAttr,
                    initAttr,
                    extarAttr,
                    equipData.itemData.Description);
            } else if (backpackData != null)
            {
                //GetName("Sell").SetActive(true);
                if(backpackData.itemData.IsEquip()) {
                    string baseAttr = "";
                    var itemData = backpackData.itemData;
                    var entry  = backpackData.packInfo.PackEntry;
                    if (itemData.Damage > 0)
                    {
                        baseAttr += string.Format("[9800fc]攻击力:{0}[-]\n", itemData.Damage);
                    }
                    if (itemData.RealArmor > 0)
                    {
                        baseAttr += string.Format("[8900cf]防御力:{0}[-]\n", itemData.RealArmor);
                    }

                    string initAttr = string.Format("[fc0000]天赋攻击:{0}[-]\n[fc0000]天赋防御:{1}[-]\n", 
                    entry.RndAttack,
                    entry.RndDefense);
                    string extarAttr = string.Format("[fc0000]额外攻击:{0}[-]\n[fc0000]额外防御:{1}[-]\n", 
                                           entry.ExtraAttack,
                                           entry.ExtraDefense);

                    var cost = "";
                    if(itemData.GoldCost > 0) {
                        cost = string.Format("[0098fc]{0}金币[-]", itemData.GoldCost);
                    }else {
                        cost = string.Format("[0098fc]{0}晶石[-]", itemData.propsConfig.JingShi);
                    }

                    Name.text = string.Format("[ff9500]{0}({1}级)[-]\n{2}\n{3}{4}{5}[fcfc00]{6}[-]",
                        itemData.ItemName, 
                        entry.Level,
                        cost, 
                        baseAttr,
                        initAttr,
                        extarAttr,
                        itemData.Description); 

                }
                else if (backpackData.itemData.IsGem())
                {
                    var cost = "";
                    var itemData = backpackData.itemData;
                    if(itemData.GoldCost > 0) {
                        cost = string.Format("[0098fc]{0}金币[-]", itemData.GoldCost);
                    }else {
                        cost = string.Format("[0098fc]{0}金币[-]", itemData.propsConfig.JingShi*100);
                    }

                    GetName("Equip").SetActive(false); 
                    Name.text = string.Format("[ff9500]{0}({1}阶)[-]\n{2}\n[0098fc]数量{3}[-]\n[fcfc00]{4}[-]",
                        backpackData.itemData.ItemName,
                        backpackData.itemData.Level,
                        cost,
                        backpackData.num,
                        backpackData.itemData.Description
                    );

                } else if (backpackData.itemData.IsProps())
                {
                    var cost = "";
                    var itemData = backpackData.itemData;
                    if(itemData.GoldCost > 0) {
                        cost = string.Format("[0098fc]{0}金币[-]", itemData.GoldCost);
                    }else {
                        cost = string.Format("[0098fc]{0}金币[-]", itemData.propsConfig.JingShi*100);
                    }

                    GetName("Equip").SetActive(false);
                    GetName("LevelUp").SetActive(false);
                    Name.text = string.Format("[ff9500]{0}[-]\n{1}\n[0098fc]数量{2}[-]\n[fcfc00]{3}[-]",
                        backpackData.itemData.ItemName,
                        cost,
                        backpackData.num,
                        backpackData.itemData.Description
                    );

                
                } else
                {
                    
                }
            }

        }

        void OnSell()
        {
            GameInterface_Package.SellItem(backpackData);
            WindowMng.windowMng.PopView();
        }

    }

}