using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class NumMoney {
        public string itemId;
        public int num;
        public int money;
    }
    public class ChargeUI : IUserInterface 
    {
        GameObject Cell;
        UIGrid Grid;
        List<GameObject> Cells = new List<GameObject>();
        UILabel JingShi;
        List<NumMoney> chargeList;
        void Awake() {
            chargeList = new List<NumMoney>() {
                new NumMoney(){itemId = "item6", num=100, money=6},
                new NumMoney(){itemId = "item18", num=350, money=18},
                new NumMoney(){itemId = "item60", num=1200, money=60},
            };

            SetCallback("closeButton", Hide);
            JingShi = GetLabel("JingShi");

            Grid = GetName("Grid").GetComponent<UIGrid>();
            Cell = GetName("Cell");
            Cell.SetActive(false);

            this.regEvt = new System.Collections.Generic.List<MyEvent.EventType>(){
                MyEvent.EventType.UpdateItemCoffer, 
            };
            RegEvent();

        }

        protected override void OnEvent(MyEvent evt)
        {
            UpdateFrame();
        }

         void UpdateFrame(){
            foreach(var c in Cells){
                GameObject.Destroy(c);
            }
            Cell.SetActive(false);
            foreach(var id in chargeList ) {
                var c = GameObject.Instantiate(Cell) as GameObject;
                c.transform.parent = Cell.transform.parent;
                Util.InitGameObject(c);
                c.SetActive(true);
                var pak = c.GetComponent<ChargeItem>();
                pak.SetCharge(this, id);
                Cells.Add(c);
            }
            Grid.repositionNow = true;

            JingShi.text = "晶石: "+ServerData.Instance.playerInfo.JingShi;
        }
    }

}