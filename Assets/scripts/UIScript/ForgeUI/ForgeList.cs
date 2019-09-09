using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class ForgeList : IUserInterface 
    {
        GameObject Cell;
        UIGrid Grid;
        List<GameObject> Cells = new List<GameObject>();
        void Awake() {
            SetCallback("closeButton", Hide);
            Grid = GetName("Grid").GetComponent<UIGrid>();
            Cell = GetName("Cell");
            Cell.SetActive(false);

        }
        void Start() {
            UpdateFrame();
        }

        void UpdateFrame() {
            foreach(var c in Cells){
                GameObject.Destroy(c);
            }

            Cell.SetActive(false);
            var eq = GameInterface_Forge.GetForgeList();
            for(int i = 0; i < eq.Count; i++) {
                var item = eq[i];
                var c = GameObject.Instantiate(Cell) as GameObject;
                c.transform.parent = Cell.transform.parent;
                Util.InitGameObject(c);
                c.SetActive(true);
                var pak = c.GetComponent<ForgeItem>();
                pak.SetForgeItem(item);
                Cells.Add(c);
            }

            Grid.repositionNow = true;
        }
    }

}