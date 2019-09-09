using UnityEngine;
using System.Collections;

namespace MyLib
{
    public static class DropGoods
    {
        public static void Drop(NpcAttribute mon)
        {
            var treasure = mon.GetDropTreasure();
            foreach(var d in treasure) {
                Log.Sys("DropTreasure " + d.Count);
                var itemData = Util.GetItemData(0, (int)d[0]);
                int num = 1;
                if (d.Count >= 3)
                {
                    num = (int)d[2];
                }

                ItemDataRef.MakeDropItem(itemData, mon.transform.position + new Vector3(0, 0.4f, 0), num);
            }
        }

        /// <summary>
        /// 掉落网络物品 
        /// </summary>
        /// <param name="mon">Mon.</param>
        public static void DropStaticGoods(NpcAttribute mon) {

            var treasure = mon.GetDropTreasure();
            foreach(var d in treasure) {
                Log.Sys("DropTreasure " + d.Count);
                var itemData = Util.GetItemData((int)ItemData.GoodsType.Props, (int)d[0]);
                int num = 1;
                if (d.Count >= 3)
                {
                    num = (int)d[2];
                }

                DropItemStatic.MakeDropItem(itemData, mon.transform.position + new Vector3(0, 0.4f, 0), num);
            }
        }
    }

}