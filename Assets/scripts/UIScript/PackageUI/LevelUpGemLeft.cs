using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class LevelUpGemLeft : IUserInterface
    {
        public System.Action<BackpackData> TakeOffGem;
        public System.Action LevelUp;

        List<GameObject> Cells = new List<GameObject>();
        UIGrid Grid;
        GameObject Cell;

        UILabel Rate;
        UITable Table;
        void Awake()
        {
            Grid = GetName("Grid").GetComponent<UIGrid>();
            Cell = GetName("Cell");
            Rate = GetLabel("Rate");
            Table = GetName("Table").GetComponent<UITable>();
            SetCallback("LevelUp", OnLevelUp);

            regEvt = new System.Collections.Generic.List<MyEvent.EventType>() {
                MyEvent.EventType.UpdateItemCoffer,
            };
            RegEvent();
        }

        List<BackpackData> gems = new List<BackpackData>();
        public void SetGems(List<BackpackData> g) {
            gems = g;
            UpdateFrame();
        }

        protected override void OnEvent(MyEvent evt)
        {
            UpdateFrame();
        }
        void UpdateFrame() {
            InitList();
            bool findGem = false;
            if(gems.Count > 0){
                var lev = gems[0].itemData.Level+1;
                var target = GameInterface_Package.GetAllLevGemRate(lev);
                if(target > 0) {
                    Rate.text = string.Format("[ff9500]成功率:{0}%[-]", target);
                    findGem = true;
                }
            }
            if(!findGem) {
                Rate.text = "";
            }

            StartCoroutine(WaitReset());
        }

        IEnumerator WaitReset() {
            yield return new WaitForSeconds(0.1f);
            Table.repositionNow = true;
            yield return new WaitForSeconds(0.2f);
            Table.repositionNow = true;
        }

        void InitList(){
            foreach(var c in Cells){
                GameObject.Destroy(c);
            }
            Cell.SetActive(false);

            for(int i = 0; i < gems.Count; i++) {
                var item = gems[i];
                var temp = item;
                if(item != null) {
                    var c = GameObject.Instantiate(Cell) as GameObject;
                    c.transform.parent = Cell.transform.parent;
                    Util.InitGameObject(c);
                    c.SetActive(true);
                    var pak = c.GetComponent<PackageItem>();
                    pak.SetData(item, 0, false);
                    pak.SetButtonCB(delegate(){
                        TakeOffGem(temp);
                    });
                    Cells.Add(c);

                }

            }

            Grid.repositionNow = true;
        }

        void OnLevelUp()
        {
            LevelUp();
        }
    }
}
