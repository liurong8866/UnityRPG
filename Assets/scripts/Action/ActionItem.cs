
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using System.Collections.Generic;

namespace MyLib
{
	public class ActionItem
	{
		public enum OwnerType
		{
			Backpack,
			Equip,
		}

		ItemData _itemData = null;

 		//装备扩展信息
		public ItemData itemData {  //物品基础信息
			set {
				_itemData = value;
				InitItemData ();
			}
			get {
				return _itemData;
			}
		}

		//BackpackData  
		//EquipData 
		//DynamicData inherit  ActionItem

		public string GetTitle ()
		{
			return itemData.ItemName;
		}


		public virtual void InitItemData ()
		{
		}

	




	}


}