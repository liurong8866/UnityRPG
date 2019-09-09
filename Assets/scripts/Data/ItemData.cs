
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public enum GoodsTypeEnum {
        Props = 0,
        Equip = 1,
    }
	public class ItemData
	{
		private IConfig config = null;
		private EquipConfigData equipConfig = null;
		public PropsConfigData propsConfig = null;

        public int RandomInitAttack() {
            if(string.IsNullOrEmpty(equipConfig.randAttack)) {
                return 0;
            }


            var dic = SimpleJSON.JSON.Parse(equipConfig.randAttack).AsArray;
            var s = dic[0].AsInt;
            var e = dic[1].AsInt;
            var ret = Random.Range(s, e+1);
            return ret;
        }

        public int RandomInitDefense() {
            if(string.IsNullOrEmpty(equipConfig.randDefense)) {
                return 0;
            }


            var dic = SimpleJSON.JSON.Parse(equipConfig.randDefense).AsArray;
            var s = dic[0].AsInt;
            var e = dic[1].AsInt;
            var ret = Random.Range(s, e+1);
            return ret;
        }

        public int GetRndAtk() {
            if(string.IsNullOrEmpty(propsConfig.attack)) {
                return 0;
            }

            var dic = SimpleJSON.JSON.Parse(propsConfig.attack).AsArray;
            var s = dic[0].AsInt;
            var e = dic[1].AsInt;
            var ret = Random.Range(s, e+1);
            return ret;
        }
        public int GetRndDef() {
            if(string.IsNullOrEmpty(propsConfig.defense)) {
                return 0;
            }
            var dic = SimpleJSON.JSON.Parse(propsConfig.defense).AsArray;
            var s = dic[0].AsInt;
            var e = dic[1].AsInt;
            var ret = Random.Range(s, e+1);
            return ret;
        }
        public int GoldCost {
            get {
                if(IsEquip()) {
                    return equipConfig.goldCost;
                }
                return propsConfig.goldCoin;
            }
        }
		public enum GoodsType
		{
			Props = 0,
			Equip = 1,
		}
        public GoodsType GetGoodsType() {
            if(IsProps()) {
                return GoodsType.Props;
            }
            return GoodsType.Equip;
        }

		//服务器上装备的位置
		public enum EquipPosition
		{
			HEAD = 1,
			TROUSERS = 2,
			SHOES = 3,
			GLOVES = 4,
			//RING = 5,
			//NECK = 6,
			BODY = 7,
			WEAPON = 8,
		}

		//普通道具的类
		public enum UnitTypeEnum
		{
			None = 0,
			POTION = 1,
			GEM = 2,
            UPGRADE = 3,
            SKILL_BOOK = 4,

			QUESTITEM = 9,
			GOLD = 12,
			MATERIAL = 13,

            FORGE_GRAPH = 16,

            POWER_DRUG = 17,
            QIPAO_DRUG = 18,
            XieZi_DRUG = 19,
		}

        public enum ItemID {
            GOLD = 4,
            DRUG = 101,
            JING_SHI = 5,
            CANG_MING_SHUI = 6,
            POWER_DRUG = 103,
        }

		//装备对应的EquipData位置



		public enum UnitEffectEnum
		{
			None,
			AddHP,
			AddMP,
		}

		public int ObjectId {
			get {
				return config.id;
			}
		}

		public UnitEffectEnum UnitEffect = UnitEffectEnum.None;


        public int triggerBuffId{
            get {
                return propsConfig.triggerBuffId;
            }
        }


		public UnitTypeEnum UnitType {
			get {
				//道具类型
				if (propsConfig != null) {
					return (UnitTypeEnum)propsConfig.propsType;
				}
				Log.Important ("Not a Props " + ItemName);
				//throw new System.NotSupportedException ("Not a Props");
                return UnitTypeEnum.None;
			}
		}

		public EquipPosition equipPosition {
			get {
				if (equipConfig != null) {
					return (EquipPosition)equipConfig.equipPosition;
				}
				Log.Important ("Not a Equip " + ItemName);
				throw new System.NotImplementedException ("Not a Equip");
			}
		}

		public int Rarity {
			get {
				return 0;
			}
		}
		/*
	 	* Potion Stack Number
	 	*/ 
		public int MaxStackSize {
			get {
				return propsConfig.maxAmount;
			}
		}

		//400HP in 4 seconds
		//explode trap
		//skeleton trap

		public int Damage {
			get {
				return equipConfig.attack;
			}
		}


		//装备模型名称
		public string ModelName {
			get {
				return equipConfig.modelId;
			}
		}

		//Binded是物品的动态信息
		public enum BindInfo
		{
			Free,
			Pick,
			Equip,
			Binded,
		}



		public int RealArmor {
			get {
				return equipConfig.defense;
			}
		}

		public string ItemName {
			get {
				if (propsConfig != null) {
					return propsConfig.name;
				}
				return equipConfig.name;
			}
		}

		public int IconSheet {
			get {
				if (propsConfig != null) {
					return propsConfig.sheet;
				}
				return equipConfig.sheet;
			}
		}

		public string IconName {
			get {
				if (propsConfig != null) {
					return propsConfig.icon;
				}
				return equipConfig.icon;
			}
		}

		public string DropMesh {
			get {
				if (propsConfig != null) {
					return propsConfig.dropIcon;
				}
				return equipConfig.dropIcon;
			}
		}

		public string Description {
			get {
				if (propsConfig != null) {
					return propsConfig.description;
				}
				return equipConfig.description;
			}
		}

        //道具等级
		public int Level {
			get {
				if (propsConfig != null) {
                    return propsConfig.level;
				}
				return equipConfig.equipLevel;
			}
		}
		/*
		 * Weapon1  Point3 
		 * Weapon2
		 */ 
		//如果是武器是否有刀鞘
		public bool HasScabbard {
			get {
				return equipConfig.hasScabbard;
			}
		}

		//装备所在职业
		public Job equipClass {
			get {
				return (Job)equipConfig.job;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns><c>true</c> if this instance is equip; otherwise, <c>false</c>.</returns>
		public bool IsEquip ()
		{
			return equipConfig != null;
		}

		public bool IsFashion ()
		{
			return IsEquip () && equipConfig.isFashion;
		}

		public bool IsTask ()
		{
			return IsProps () && UnitType == UnitTypeEnum.QUESTITEM;
		}

		public bool IsGeneral ()
		{
			return false;
		}

		public bool IsGem ()
		{
			return IsProps () && UnitType == UnitTypeEnum.GEM;
		}

		public bool IsMaterial ()
		{
			return IsProps () && UnitType == UnitTypeEnum.MATERIAL;
		}

		public bool IsProps ()
		{
			return propsConfig != null;
		}


		//根据BaseID 以及装备类型获得ItemData
		public ItemData (int goodsType, int baseId)
		{
			if (goodsType == 0) {
				propsConfig =  GMDataBaseSystem.SearchIdStatic<PropsConfigData> (GameData.PropsConfig, baseId);
				config = propsConfig;
			} else {
				equipConfig = GMDataBaseSystem.SearchIdStatic<EquipConfigData> (GameData.EquipConfig, baseId);
				config = equipConfig;
			}
			if (config == null) {
				Log.Critical ("Init ItemData Error " + goodsType + " " + baseId);
			}
		}

		public class Attr
		{
			public CharAttribute.CharAttributeEnum k;
			public int v;

			public Attr (CharAttribute.CharAttributeEnum k1, int v1)
			{
				k = k1;
				v = v1;
			}
		}

		public IEnumerable<Attr> whiteAttr {	
			get {
                /*
				foreach (CharAttribute.CharAttributeEnum e in ActionItem.WhiteAttributeEnum) {
					Log.Important ("Config is what " + config);
					//Log.Important("Field is "+e.ToString().ToLower());
					var fie = config.GetType ().GetField (e.ToString ().ToLower ());
					if (fie != null) {
						var val = (int)fie.GetValue (config);
						yield return new Attr (e, val);
					}
				}
                */
                yield break;
			}
		}

		List<Attr> allAttr ()
		{
			List<Attr> a = new List<Attr> ();
			foreach (Attr attr in whiteAttr) {
				a.Add (attr);
			}
			return a;
		}

		public bool IsPackage (GameInterface_Package.PackagePageEnum pkg)
		{
			if (pkg == GameInterface_Package.PackagePageEnum.All) {
				return true;
			}
			if (pkg == GameInterface_Package.PackagePageEnum.Equip) {
				return IsEquip ();
			}
			if (pkg == GameInterface_Package.PackagePageEnum.Fashion) {
				return IsFashion ();
			}
			if (pkg == GameInterface_Package.PackagePageEnum.Task) {
				return IsTask ();
			}

			if (pkg == GameInterface_Package.PackagePageEnum.General) {
				return IsGeneral ();
			}

			return false;
		}

		//比较装备的新旧属性
		public string CompareWhiteAttribute (ItemData curData)
		{
			string str = "";
			var myAttr = allAttr ();
			var otherAttr = curData.allAttr ();
			foreach (Attr a in myAttr) {
				var oldAtt = otherAttr.Find (delegate(Attr att) {
					return att.k == a.k;
				});
				if (a.v > 0) {
					if (oldAtt == null) {
						str += SetAttri (a);
					} else {
						str += SetAttri (new Attr (a.k, a.v - oldAtt.v));
					}
				}
			}
			return str;
		}

		string SetAttri (Attr newAtt)
		{
			var db = GMDataBaseSystem.database.GetJsonDatabase (GMDataBaseSystem.DBName.StrDictionary);

			string str = "";
			str = string.Format ("{0}+{1}\n", db.SearchForKey (newAtt.k.ToString ()), newAtt.v);
			return str;
		}
	}
}
