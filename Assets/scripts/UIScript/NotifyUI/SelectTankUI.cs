using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class SelectTankUI : IUserInterface
    {
        private List<GameObject> items = new List<GameObject>();
        private UIGrid grid;
        private GameObject cell;
        private List<string> tank = new List<string>()
        {
            "天启坦克",
            "光棱坦克",
            "幻影坦克",
        };

        void Awake()
        {
            grid = GetGrid("Grid");
            cell = GetName("Cell");
            cell.SetActive(false);
            SetCallback("StartGame", OnStartGame);
        }

        void OnStartGame()
        {
            if (jobSel != 0)
            {
                WorldManager.worldManager.WorldChangeScene(6, false);
            }else {
                Util.ShowMsg("未选择坦克职业");
            }
        }

        void Start()
        {
            for (var i = 0; i < tank.Count; i++)
            {
                var n = tank [i];
                //var c = Object.Instantiate(cell) as GameObject;
                var c = NGUITools.AddChild(cell.transform.parent.gameObject, cell);
                items.Add(c);
                c.SetActive(true);
                //c.transform.parent = cell.transform.parent;

                Util.InitGameObject(c);
                IUserInterface.SetText(c, "Name", n);
                var temp = i;
                c.GetComponent<IUserInterface>().SetCallback("Info", () =>
                {
                    OnSelect(temp);
                });
            }
            grid.repositionNow = true;
        }

        private int jobSel = 0;

        void OnSelect(int i)
        {
            var job = i + 1;
            jobSel = job;
            ServerData.Instance.playerInfo.Roles.Job = (Job)job;
            Util.ShowMsg("选择了职业:"+(Job)job);
        }
    }
}