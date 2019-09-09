using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    ///背包右侧UI 
    /// </summary>
    public class PackageRight : IUserInterface
    {
        List<GameObject> Cells = new List<GameObject>();
        UIGrid Grid;
        GameObject Cell;
        void Awake() {
            Grid = GetName("Grid").GetComponent<UIGrid>();
            Cell = GetName("Cell");

            this.regEvt = new System.Collections.Generic.List<MyEvent.EventType>(){
                //MyEvent.EventType.OpenItemCoffer,
                MyEvent.EventType.UpdateItemCoffer,
            };
            RegEvent();
        }

        void UpdateFrame() {
            foreach(var c in Cells){
                GameObject.Destroy(c);
            }
            Cell.SetActive(false);
            for(int i = 0; i < BackPack.MaxBackPackNumber; i++) {
                var item = GameInterface_Package.playerPackage.EnumItem(GameInterface_Package.PackagePageEnum.All, i);
                if(item != null && item.packInfo != null && item.itemData != null) {
                    Log.GUI("PackageRight is "+item.packInfo+" slot is "+item.InstanceID);
                    var c = GameObject.Instantiate(Cell) as GameObject;
                    c.transform.parent = Cell.transform.parent;
                    Util.InitGameObject(c);
                    c.SetActive(true);
                    var pak = c.GetComponent<PackageItem>();
                    pak.SetData(item);
                    Cells.Add(c);

                }

            }

            Grid.repositionNow = true;
        }
        protected override void OnEvent(MyEvent evt)
        {
            UpdateFrame();
        }
    }

}