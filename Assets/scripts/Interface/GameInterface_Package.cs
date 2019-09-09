
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 背包UI获取数据接口
 * 
 * 1. 实例化一个全局背包接口对象
 * 2. 调用接口获取数据
 */ 

namespace MyLib {
	public class GameInterface_Package {
		public static GameInterface_Package playerPackage = new GameInterface_Package();

		public enum PackagePageEnum
		{
			General = 1,
			Equip,
			Fashion,
			All,
			Task,
		}

		/*
		 * 背包类型：
		 * BackpackData
		 * 
		 * PackType
		 */ 

		public BackpackData EnumItem(PackagePageEnum type, int index) {
			return BackPack.backpack.EnumItem (type, index);
		}

        public void LevelUpEquip(EquipData eqData, List<BackpackData> gems){
            var lev = CGLevelUpEquip.CreateBuilder();
            lev.EquipId = eqData.id;
            foreach(var g in gems){
                lev.AddGemId(g.id);
            }
            KBEngine.Bundle.sendImmediate(lev);
        }
        public void LevelUpGem(List<BackpackData> gems) {
            var lev = CGLevelUpGem.CreateBuilder();
            foreach(var g in gems){
                lev.AddGemId(g.id);
            }
            KBEngine.Bundle.sendImmediate(lev);
        }


        /// <summary>
        ///获得背包所有某个等级的宝石 
        /// </summary>
        /// <returns>The all lev gems.</returns>
        /// <param name="lev">Lev.</param>
        public static List<PropsConfigData>  GetAllLevGems(int lev) {
            var ret = new List<PropsConfigData>();
            foreach(var d in GameData.PropsConfig) {
                if(d.propsType == (int)ItemData.UnitTypeEnum.GEM) {
                    if(d.level == lev) {
                        ret.Add(d);
                    }
                }
            }
            return ret;
        }

        public  static int GetAllGemRate(int lev) {
            int rate = 0;

            foreach(var d in GameData.PropsConfig) {
                if(d.propsType == (int)ItemData.UnitTypeEnum.GEM) {
                    if(d.level == lev) {
                        rate += d.rate;
                    }
                }
            }
            return rate;
        }

        public static int GetRndGemForId(int lev, int id, int allRate) {
            var rnd = Random.Range(0, 100);
            if(rnd < allRate) {
                return id;
            }
            return -1;
        }

        public static int lastPossibility;
        /// <summary>
        /// 获得随机的某个宝石产物 
        /// </summary>
        /// <returns>The random gem.</returns>
        /// <param name="lev">Lev.</param>
        public static PropsConfigData GetRndGem(int lev) {
            var rnd = Random.Range(0, 100);
            lastPossibility = rnd;
            var lastRate = 0;
            foreach(var d in GameData.PropsConfig) {
                if(d.propsType == (int)ItemData.UnitTypeEnum.GEM) {
                    if(d.level == lev) {
                        lastRate += d.rate;
                        if(rnd < lastRate) {
                            return d;
                        }
                    }
                }
            }
            return null;
        }


        public static int GetAllLevGemRate(int lev) {
            var rate = 0;
            foreach(var d in GameData.PropsConfig) {
                if(d.propsType == (int)ItemData.UnitTypeEnum.GEM) {
                    if(d.level == lev) {
                        rate += d.rate;
                    }
                }
            }
            return rate;
        }

        /// <summary>
        ///只能卖出物品，暂时不支持装备
        /// ReduceItem AddGold
        /// </summary>
        public static void SellItem(BackpackData bd) {
            var sell = CGSellUserProps.CreateBuilder();
            sell.UserPropsId = bd.id;
            sell.GoodsType = (int)bd.itemData.GetGoodsType();
            KBEngine.Bundle.sendImmediate(sell);
            WindowMng.windowMng.ShowNotifyLog(bd.itemData.ItemName+"-"+bd.num);
        }

        public static void SellQuestItem(int itemId) {
            var item = BackPack.backpack.GetItemByTypeId(itemId);
            if(item != null) {
                SellItem(item);
            }
        }

	}
}
