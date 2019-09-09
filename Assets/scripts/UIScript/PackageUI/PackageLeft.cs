using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class PackageLeft : IUserInterface 
    {
        List<GameObject> Cells = new List<GameObject>();
        UIGrid Grid;
        GameObject Cell;
        void Awake() {

            Grid = GetName("Grid").GetComponent<UIGrid>();
            Cell = GetName("Cell");

            this.regEvt = new System.Collections.Generic.List<MyEvent.EventType>(){
                MyEvent.EventType.UpdateItemCoffer,
            };
            RegEvent();
        }

        void UpdateFrame() {
            foreach(var c in Cells){
                GameObject.Destroy(c);
            }
            Cell.SetActive(false);
            var eq = BackPack.backpack.GetEquipmentData();
            for(int i = 0; i < eq.Count; i++) {
                var item = eq[i];
                if(item != null) {
                    var c = GameObject.Instantiate(Cell) as GameObject;
                    c.transform.parent = Cell.transform.parent;
                    Util.InitGameObject(c);
                    c.SetActive(true);
                    var pak = c.GetComponent<PackageItem>();
                    pak.SetEquipData(item);

                    Cells.Add(c);
                }

            }

            Grid.repositionNow = true;
        }
        protected override void OnEvent(MyEvent evt)
        {
            UpdateFrame();
        }
        // Use this for initialization
        void Start()
        {
    
        }
    
        // Update is called once per frame
        void Update()
        {
    
        }
    }

}