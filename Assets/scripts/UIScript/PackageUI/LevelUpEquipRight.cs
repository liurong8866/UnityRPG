using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class LevelUpEquipRight : IUserInterface
    {
        /*
        public LevelUpEquip parent{
            private get; set;
        }
        */
        public System.Action<BackpackData> PutInGem;
        List<GameObject> Cells = new List<GameObject>();
        UIGrid Grid;
        GameObject Cell;

        void Awake()
        {
            Grid = GetName("Grid").GetComponent<UIGrid>();
            Cell = GetName("Cell");
            regEvt = new System.Collections.Generic.List<MyEvent.EventType>() {
                MyEvent.EventType.UpdateItemCoffer,
            };
            RegEvent();
        }

        List<BackpackData> gems = new List<BackpackData>();

        public void SetGems(List<BackpackData> g)
        {
            gems = g;
            UpdateFrame();
        }

        int CheckContainGem(long id)
        {
            var c = 0;
            foreach (var g in gems)
            {
                if (g.id == id)
                {
                    c++;
                }
            }
            return c;
        }

        void UpdateFrame()
        {
            foreach (var c in Cells)
            {
                GameObject.Destroy(c);
            }
            Cell.SetActive(false);
            for (int i = 0; i < BackPack.MaxBackPackNumber; i++)
            {
                var item = GameInterface_Package.playerPackage.EnumItem(GameInterface_Package.PackagePageEnum.All, i);
                var temp = item;
                if (item != null && item.itemData.IsGem())
                {
                    var count = CheckContainGem(item.id);
                    if(item.entry.Count > count)
                    {
                        var c = GameObject.Instantiate(Cell) as GameObject;
                        c.name = ""+item.id;
                        c.transform.parent = Cell.transform.parent;
                        Util.InitGameObject(c);
                        c.SetActive(true);
                        var pak = c.GetComponent<PackageItem>();
                        pak.SetData(item, count);
                        pak.SetButtonCB(delegate()
                        {
                            PutInGem(temp);
                        });
                        Cells.Add(c);
                    }
                }
            }
            StartCoroutine(WaitReset());
        }
        IEnumerator WaitReset() {
            yield return new WaitForSeconds(0.1f);
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