using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyLib
{
    public class ScoreUI : IUserInterface
    {
        UIGrid grid;
        GameObject cell;
        private List<GameObject> cells = new List<GameObject>();

        void Awake()
        {
            grid = GetGrid("Grid");
            cell = GetName("Cell");
            cell.SetActive(false);
        }

        public void SetData(Dictionary<int, ScoreData> score)
        {
            while (cells.Count < score.Count)
            {
                var g = Object.Instantiate(cell) as GameObject;
                g.transform.parent = cell.transform.parent;
                Util.InitGameObject(g);
                cells.Add(g);
                g.SetActive(false);
            }
            foreach (var c in cells)
            {
                c.SetActive(false);
            }
            var keys = score.Keys.ToList();
            keys.Sort((a, b) =>
            {
                var ad = score [a];
                var bd = score [b];
                if (ad.killed > bd.killed)
                {
                    return -1;    
                }
                if (ad.killed < bd.killed)
                {
                    return 1;
                }
                return 0;
            });

            for (var i = 0; i < keys.Count; i++)
            {
                var k = keys [i];
                var player = ObjectManager.objectManager.GetPlayer(k);
                if (player != null)
                {
                    var g = cells [i];
                    g.SetActive(true);
                    IUserInterface.SetText(g, "Name", player.GetComponent<NpcAttribute>().userName+": "+score [k].killed + "");
                }
            }
            StartCoroutine(WaitReset());
        }

        IEnumerator WaitReset()
        {
            yield return new WaitForSeconds(0.1f);
            grid.repositionNow = true;
        }
    }
}