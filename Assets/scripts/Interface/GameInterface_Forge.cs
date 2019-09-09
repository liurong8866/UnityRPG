using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace MyLib
{
    public static class GameInterface_Forge
    {
        public static int GetForgeLevel(int pos)
        {
            var flist = ServerData.Instance.playerInfo.ForgeLevelList;
            foreach (var f in flist)
            {
                if (f.Key == pos)
                {
                    return f.Lev;
                }
            }
            return 0;
        }

        public static void AddForgeLevel(int pos)
        {
            var flist = ServerData.Instance.playerInfo.ForgeLevelList;
            foreach (var f in flist)
            {
                if (f.Key == pos)
                {
                    f.Lev++;
                    Util.ShowMsg("打造等级提升了，等级" + f.Lev);
                    return;
                }
            }

            var flev = ForgeKV.CreateBuilder();
            flev.Key = pos;
            flev.Lev = 1;
            flist.Add(flev.Build());
            Util.ShowMsg("打造等级提升了，等级" + 1);
        }

        static int GetRandomAttack(int equipId)
        {
            var itemData = Util.GetItemData((int)GoodsTypeEnum.Equip, equipId);
            return itemData.RandomInitAttack();
        }

        static int GetRandomDefense(int equipId)
        {
            var itemData = Util.GetItemData((int)GoodsTypeEnum.Equip, equipId);
            return itemData.RandomInitDefense();
        }

        /// <summary>
        /// 检测相关的材料是否足够
        /// 扣除材料
        /// 背包增加对应的装备 
        /// 装备不能堆叠
        /// 背包空间需要足够 
        /// </summary>
        /// <param name="forgeId">Forge identifier.</param>
        public static void Forge(int forgeId)
        {
            var forgeData = GMDataBaseSystem.database.SearchId<ForgeConfigData>(GameData.ForgeConfig, forgeId);
            var needMat = JSON.Parse(forgeData.materials).AsObject;
            var isEnough = true;
            foreach (KeyValuePair<string, JSONNode> m in needMat)
            {
                var hasCount = BackPack.backpack.GetItemCount((int)GoodsTypeEnum.Props, System.Convert.ToInt32(m.Key));
                if (hasCount < m.Value.AsInt)
                {
                    isEnough = false;
                    break;
                }
            }
            var hasSlot = BackPack.backpack.CheckBackpackEmpty();

            if (isEnough && hasSlot)
            {
                foreach (KeyValuePair<string, JSONNode> m in needMat)
                {
                    var propsId = BackPack.backpack.GetItemId(System.Convert.ToInt32(m.Key));
                    PlayerData.ReduceItem(propsId, m.Value.AsInt);
                }
                PlayerData.AddEquipInPackage(forgeData.output, GetRandomAttack(forgeData.output), GetRandomDefense(forgeData.output));

            } else
            {
                if (!isEnough)
                {
                    Util.ShowMsg("材料不足不能打造");
                } else
                {
                    Util.ShowMsg("背包已满请空出位置再打造");
                }
            }
        }


        /// <summary>
        /// 根据当前打造等级获取相关配置表 
        /// </summary>
        /// <returns>The forge list.</returns>
        public static List<ForgeConfigData> GetForgeList()
        {
            List<ForgeConfigData> ret = new List<ForgeConfigData>();

            foreach (var d in GameData.ForgeConfig)
            {
                var item = Util.GetItemData((int)GoodsTypeEnum.Equip, d.output);
                var curLev = GetForgeLevel((int)item.equipPosition);
                if (d.level <= curLev)
                {
                    ret.Add(d);
                }
            }
            return ret;
        }

        public static void LearnForgeSkill(long propsId)
        {
            var backpackData = BackPack.backpack.GetItemInBackPack(propsId);
            if (backpackData != null)
            {
                Log.Sys("LearnForgeSkill "+propsId+" userData "+backpackData.itemData.propsConfig.UserData);
                var equipPos = (int)System.Convert.ToSingle(backpackData.itemData.propsConfig.UserData);

                var ret = PlayerData.ReduceItem(propsId, 1);
                if (ret)
                {
                    AddForgeLevel(equipPos);
                }
            }

        }

        /// <summary>
        /// 是否材料足够打造 
        /// </summary>
        /// <returns><c>true</c> if can forge; otherwise, <c>false</c>.</returns>
        public static bool CanForge(int forgeId) {
            var forgeData = GMDataBaseSystem.database.SearchId<ForgeConfigData>(GameData.ForgeConfig, forgeId);
            var needMat = JSON.Parse(forgeData.materials).AsObject;
            var isEnough = true;
            foreach (KeyValuePair<string, JSONNode> m in needMat)
            {
                var hasCount = BackPack.backpack.GetItemCount((int)GoodsTypeEnum.Props, System.Convert.ToInt32(m.Key));
                if (hasCount < m.Value.AsInt)
                {
                    isEnough = false;
                    break;
                }
            }
            return isEnough;
        }
    }

}