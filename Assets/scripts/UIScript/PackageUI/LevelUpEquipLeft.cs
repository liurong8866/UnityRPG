using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class LevelUpEquipLeft : IUserInterface
    {
        public LevelUpEquip parent;
        EquipData equipData;

        List<GameObject> Cells = new List<GameObject>();
        UIGrid Grid;
        GameObject Cell;
        List<BackpackData> gems = new List<BackpackData>();

        public void SetGems(List<BackpackData> g)
        {
            gems = g;
            UpdateFrame();
        }

        public void SetEquip(EquipData equip)
        {
            equipData = equip;
            UpdateFrame();
        }

        UILabel Name;

        UILabel Rate;
        UITable Table;

        void Awake()
        {
            Grid = GetName("Grid").GetComponent<UIGrid>();
            Cell = GetName("Cell");
            Rate = GetLabel("Rate");
            Table = GetName("Table").GetComponent<UITable>();

            Name = GetLabel("Name");
            //SetCallback("closeButton", Hide);
            SetCallback("LevelUp", OnLevelUp);
            regEvt = new System.Collections.Generic.List<MyEvent.EventType>()
            {
                MyEvent.EventType.UpdateItemCoffer,
            };
            RegEvent();
        }

        protected override void OnEvent(MyEvent evt)
        {
            UpdateFrame();
        }

        void OnLevelUp()
        {
            parent.LevelUp();
        }

        void UpdateFrame()
        {
            var entry = equipData.entry;
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
                                  entry.RndAttack,
                                  entry.RndDefense);
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
            var levCost = GMDataBaseSystem.SearchIdStatic<EquipLevelData>(GameData.EquipLevel, equipData.entry.Level + 1);
            if (levCost != null)
            {
                Rate.text = string.Format("[ff9500]成功率:{0}%[-]\n[ff9500]金币:{1}[-]",
                    levCost.rate,
                    levCost.gold
                );
            }

            InitList();
            StartCoroutine(WaitReset());
        }

        IEnumerator WaitReset()
        {
            yield return new WaitForSeconds(0.1f);
            Table.repositionNow = true;
            yield return new WaitForSeconds(0.2f);
            Table.repositionNow = true;
        }

        void InitList()
        {
            foreach (var c in Cells)
            {
                GameObject.Destroy(c);
            }
            Cell.SetActive(false);

            for (int i = 0; i < gems.Count; i++)
            {
                var item = gems [i];
                var temp = item;
                if (item != null)
                {
                    var c = GameObject.Instantiate(Cell) as GameObject;
                    c.transform.parent = Cell.transform.parent;
                    Util.InitGameObject(c);
                    c.SetActive(true);
                    var pak = c.GetComponent<PackageItem>();
                    pak.SetData(item, 0, false);
                    pak.SetButtonCB(delegate()
                    {
                        parent.TakeOffGem(temp);
                    });
                    Cells.Add(c);

                }

            }

            Grid.repositionNow = true;
        }

       
    }

}